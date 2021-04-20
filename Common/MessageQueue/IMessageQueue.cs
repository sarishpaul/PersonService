using Common.Enums;
using System;
using System.Collections.Generic;

namespace Common.MessageQueue
{
    
    public delegate void MessageReceivedDelegate(string message, string jmxGroupId, object sender, ulong deliverytag, IDictionary<string,object> headers);
    public interface IMessageQueue : IDisposable
    {
        void SendMessage(string message, string destination, string messageType, string routeKey = "");
        void SendMessage(string message, string destination, QueueDestination queueDestination, string messageType);

        void SendMessage(string message, string destination, QueueDestination queueDestination, DestinationType destinationType, string messageType);

        void SendMessage(string message, string destination, QueueDestination queueDestination, DestinationType destinationType, bool persistent, string messageType);

        MessageReceivedDelegate OnMessageReceived { get; set; }
        void CreateDurableConsumer(string name, QueueDestination queueDestination);
      
        void Stop();

        string JmxGroupId { get; set; }

        string ServiceName { get; set; }
        ulong DeliveryTag { get; set; }

        void Acknowledge(bool closeConnection = true);

        void Acknowledge(ulong deliveryTag);
        void Acknowledge(object obj, ulong deliveryTag);
        void Nack(object obj, ulong deliveryTag);
        void Close();
        void AckMe(object sender, ulong delivery);
        void NackMe(object sender, ulong delivery);
    }
}

