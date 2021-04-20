using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.MessageQueue
{
    public abstract class MessageQueueListener : IListener
    {
        private string queueName;
        protected IMessageQueue messageQueue;
        public abstract QueueDestination QueueDestination { get; set; }
     
        public MessageQueueListener(string queueName)
        {
            this.queueName = queueName;
        }

        public MessageQueueListener(string queueName, IMessageQueue messageQueue)
        {
            this.queueName = queueName;
            this.messageQueue = messageQueue;
        }
        public virtual void StartListener()
        {
            if (this.messageQueue == null) throw new ArgumentNullException("MessageQueue is null. Initialize it in derived class constructor");
            messageQueue.CreateDurableConsumer(queueName, QueueDestination);
            messageQueue.OnMessageReceived += new MessageReceivedDelegate(subscriber_OnMessageReceived);
        }

        public abstract void subscriber_OnMessageReceived(string message, string jmxGroupId, object sender, ulong deliverytag, IDictionary<string, object> headers);

        public virtual void StopListener()
        {
            messageQueue.Stop();
        }
    }
}
