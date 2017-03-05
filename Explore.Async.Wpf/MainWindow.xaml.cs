using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;

namespace Explore.Async.Wpf
{
    public static class PlotTaskExtensions
    {
        private static double counter;
        private static readonly TaskCompletionSource<bool> barrier = new TaskCompletionSource<bool>();
        private static readonly List<Task> taskList = new List<Task>();

        public static async Task TrackTask(this Task task, RectangleBarSeries series, string title = null)
        {
            await barrier.Task;

            var p0 = DateTime.Now;

            taskList.Add(task);
            await task;

            var pc = DateTime.Now;

            series.Items.Add(new RectangleBarItem(p0.ToOADate(), counter, pc.ToOADate(), counter+1){Title = title});

            counter = counter + 1.25;
        }

        public static void Ignore(this Task task)
        {
            
        }

        public static List<Task> AllTasks { get { return taskList; } }

        public static void Unblock()
        {
            barrier.SetResult(true);
        }
    }

    public class PlotViewModel : INotifyPropertyChanged
    {
        private PlotModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public PlotModel Model
        {
            get { return model; }
            set
            {
                model = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Creates a model showing normal distributions.
        /// </summary>
        /// <returns>A PlotModel.</returns>
        public async void CreateDiagram()
        {
            // Create Series
            var s1 = new RectangleBarSeries { Title = "Individual", };
            var s2 = new RectangleBarSeries { Title = "Composite WhenAll" };
            var s3 = new RectangleBarSeries { Title = "Composite WhenAny" };

            // Create Tasks
            var simpleTask1 = Task.Delay(500).TrackTask(s1);
            var simpleTask2 = Task.Delay(250).TrackTask(s1);
            var simpleTask3 = Task.Delay(700).TrackTask(s1);

            var continuation1 = simpleTask1
                .ContinueWith(t => Task.Delay(400).TrackTask(s1).Wait())
                .TrackTask(s2, "first continuation");

            var continuation2 = continuation1
                .ContinueWith(t => Task.Delay(256).TrackTask(s1).Wait())
                .TrackTask(s2, "second continuation");
            
            Task.WhenAny(simpleTask1, simpleTask2, continuation1, continuation2)
                .TrackTask(s3, "When Any")
                .Ignore();

            Task.WhenAll(simpleTask1, simpleTask2, continuation1, continuation2)
                .TrackTask(s2, "When All")
                .Ignore();

            Task.WhenAll(simpleTask2, simpleTask3)
                .TrackTask(s2, $"When {nameof(simpleTask2)} and {nameof(simpleTask3)}")
                .Ignore();

            PlotTaskExtensions.Unblock();

            // Ensure all tasks completed and build the model.
            await Task.WhenAll(PlotTaskExtensions.AllTasks);
            this.Model = CreateModelFromSeries(s1, s2, s3);
        }

        private PlotModel CreateModelFromSeries(params Series[] series)
        {
            var model = new PlotModel { LegendPlacement = LegendPlacement.Outside };

            foreach (var serie in series)
            {
                model.Series.Add(serie);
            }

            return model;
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            var plotVm =new PlotViewModel();
            this.DataContext = plotVm;
            plotVm.CreateDiagram();
        }
    }
}
