using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Enums
{
    public enum QueueDestination
    {
        Queue = 1,
        Topic = 2
    }

    public enum DestinationType
    {
        None = 0,
        Exchange = 1,
        Queue = 2,
    }
}
