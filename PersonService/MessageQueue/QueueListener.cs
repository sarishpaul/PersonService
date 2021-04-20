using Common.Enums;
using Common.MessageQueue;
using PersonService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonService.MessageQueue
{
    public class QueueListener: MessageQueueListener
    {
        private string queueName;
        public override QueueDestination QueueDestination { get; set; }
        public bool IsProcessing { get; protected set; }

        protected bool IsListenerRunning;
        private IPersonRepository personRepository;
        private IEventRepository eventRepository;
        public QueueListener(string queueName, IMessageQueue queue, IPersonRepository personRepository, IEventRepository eventRepository): base(queueName, queue)
        {
            this.queueName = queueName;
            QueueDestination = QueueDestination.Queue;
            this.personRepository = personRepository;
            this.eventRepository = eventRepository;
        }
        public override void StartListener()
        {
            base.StartListener();
            IsListenerRunning = true;
        }

        public override void StopListener()
        {

            IsProcessing = false;
            IsListenerRunning = false;
        }

        public override void subscriber_OnMessageReceived(string message, string jmxGroupId, object sender, ulong deliverytag, IDictionary<string,object> headers)
        {
            IsProcessing = true;          
            try
            {
                var main = new MessageProcessor(base.messageQueue, personRepository, eventRepository);
                main.ProcessMessage(message, headers);
              
            }
            catch (Exception ex)
            {
               
            }

            messageQueue.AckMe(sender, deliverytag);
            if (!IsListenerRunning)
            {
                base.StopListener();
                IsProcessing = false;
            }

        }
    }
}
