#region imports

using System;

#endregion

namespace Explore.Async.Exploration.Exceptions
{
    public class CriticalUnrecoverableUserErrorException : ApplicationException
    {
        public CriticalUnrecoverableUserErrorException()
            : base("Confess to your administrator what you just did.")
        {
        }
    }
}