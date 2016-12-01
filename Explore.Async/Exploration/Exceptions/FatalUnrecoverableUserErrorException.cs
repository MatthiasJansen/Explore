#region imports

using System;

#endregion

namespace Explore.Async.Exploration.Exceptions
{
    public class FatalUnrecoverableUserErrorException : ApplicationException
    {
        public FatalUnrecoverableUserErrorException()
            : base("You did it all wrong!")
        {
        }
    }
}