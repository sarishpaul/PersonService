using System;
using System.Collections.Generic;
using System.Text;

namespace Common.MessageQueue
{
    public interface IListener
    {
        void StartListener();
        void StopListener();
    }
}
