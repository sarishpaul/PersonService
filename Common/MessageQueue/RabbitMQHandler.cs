using System;
using System.Collections.Generic;
using System.Text;
using Common.Enums;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Util;

namespace Common.MessageQueue
{
    public class RabbitMQHandler : IMessageQueue
    {

        public MessageReceivedDelegate OnMessageReceived { get; set; }
        public ulong DeliveryTag { get; set; }

        public string ServiceName { get; set; }

        private readonly string ipAddress;
        private readonly int port;
        private readonly string username;
        private readonly string password;
        private IConnection connection;
        private IModel channel;
        private IModel ichannel;
        private EventingBasicConsumer consumer;

        private const string ConsistentHashExchange = "x-consistent-hash";
        private const string TopicExchangeType = "fanout";
        private const string GroupSuffix = ".Group";
        private const string AlterNameExchange = "alternate-exchange";
        private const string DefaultRouteKey = "5";

        public RabbitMQHandler(string ipAddress, int port, string username, string password)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.username = username;
            this.password = password;
        }

        public RabbitMQHandler(string ipAddress, int port, string username, string password, string serviceName)
            : this(ipAddress, port, username, password)
        {
            ServiceName = serviceName;
        }

        public string JmxGroupId { get; set; }

        public void Acknowledge(bool closeConnection = true)
        {

        }


        public void Acknowledge(ulong deliveryTag)
        {
            if (ichannel != null && ichannel.IsOpen)
            {
                ichannel.BasicAck(deliveryTag, false);
                ichannel.Dispose();
                ichannel = null;
            }
        }

        public void Acknowledge(object obj, ulong deliveryTag)
        {
            var imodel = (IModel)obj;
            if (imodel != null && imodel.IsOpen)
            {
                imodel.BasicAck(deliveryTag, false);
                imodel.Close();
                imodel.Dispose();
            }

        }

        public void Nack(object obj, ulong deliveryTag)
        {
            var imodel = (IModel)obj;
            if (imodel != null && imodel.IsOpen)
            {
                imodel.BasicNack(deliveryTag, false, true);
                imodel.Close();
                imodel.Dispose();
            }
        }

        public void CreateExchangeQueue(string name, QueueDestination queueDestination)
        {
            try
            {
                if (Connect())
                {
                    if (queueDestination == QueueDestination.Queue)
                    {
                        var routeKey = DefaultRouteKey;
                        var arguments = new Dictionary<string, object>();
                        arguments.Add(AlterNameExchange, name);

                        channel.ExchangeDeclare(exchange: name, type: "direct", durable: true, autoDelete: false, arguments: null);

                        var exchangeName = name + GroupSuffix;
                        channel.ExchangeDeclare(exchange: exchangeName, type: ConsistentHashExchange, durable: true, autoDelete: false, arguments: arguments);
                        if (!string.IsNullOrEmpty(JmxGroupId))
                        {
                            name = name + JmxGroupId;
                        }

                        arguments.Clear();
                        arguments.Add("x-queue-mode", "lazy");
                        channel.QueueDeclare(queue: name, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                        channel.QueueBind(queue: name, exchange: exchangeName, routingKey: routeKey, arguments: null);
                    }
                    else
                    {
                        string exchangeName = name;
                        var arguments = new Dictionary<string, object>();
                        arguments.Add(AlterNameExchange, name);
                        try
                        {
                            if (exchangeName.EndsWith(".DISPLAY") || exchangeName.EndsWith(".INJECTOR"))
                                exchangeName = exchangeName.Substring(0, exchangeName.LastIndexOf('.'));
                        }
                        catch
                        {
                        }
                        arguments.Clear();
                        arguments.Add("x-queue-mode", "lazy");
                        channel.ExchangeDeclare(exchange: exchangeName, type: TopicExchangeType, durable: true, autoDelete: false, arguments: null);
                        channel.QueueDeclare(queue: name, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                        channel.QueueBind(queue: name, exchange: exchangeName, routingKey: name, arguments: null);
                    }


                    channel.BasicQos(0, 1, false);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void CreateDurableConsumer(string name, QueueDestination queueDestination)
        {
           try
            {
                if (Connect())
                {
                    CreateExchangeQueue(name, queueDestination);
                    consumer = new EventingBasicConsumer(channel);
                    consumer.Received += OnMessage;

                    if (!string.IsNullOrEmpty(JmxGroupId))
                    {
                        name = name + JmxGroupId;
                    }
                    channel.BasicConsume(queue: name, consumer: consumer);

                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error while creating Consumer", ex);
            }
        }

        public void AckMe(object sender, ulong delivery)
        {
            var basicConsumer = (EventingBasicConsumer)sender;
            basicConsumer.Model.BasicAck(delivery, false);
        }

        public void NackMe(object sender, ulong delivery)
        {
            var basicConsumer = (EventingBasicConsumer)sender;
            basicConsumer.Model.BasicNack(delivery, false, true);
        }

        private void OnMessage(object sender, BasicDeliverEventArgs e)
        {
            if (consumer.IsRunning)
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                if (this.OnMessageReceived != null)
                {
                    OnMessageReceived(message, e.RoutingKey, sender, e.DeliveryTag, e.BasicProperties.Headers);
                }
            }
        }

      


     

        public void SendMessage(string message, string destination, QueueDestination queueDestination, DestinationType destinationType, bool persistent, string messageType)
        {
            if (destinationType == DestinationType.Exchange)
            {
                SendMessageToExchange(message, destination, string.Empty, queueDestination, messageType);
            }
            else if (destinationType == DestinationType.Queue)
            {
                SendMessageToQueue(message, destination, messageType, persistent);
            }
        }

        public void SendMessage(string message, string destination, QueueDestination queueDestination, DestinationType destinationType, string messageType)
        {
            SendMessage(message, destination, queueDestination, destinationType, true,messageType);
        }
        public void SendMessage(string message, string destination, string messageType, string routeKey = "")
        {
            SendMessageToExchange(message, destination, routeKey, QueueDestination.Queue, messageType);
        }

        public void SendMessage(string message, string destination, QueueDestination queueDestination, string messageType)
        {
            try
            {
                SendMessageToExchange(message, destination, string.Empty, queueDestination, messageType);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while sending the message to Queue", ex);
            }
        }

        private void SendMessageToQueue(string message, string destination, string messageType, bool persistent = true)
        {
            if (Connect())
            {
                using (var model = connection.CreateModel())
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    var properties = model.CreateBasicProperties();
                    properties.Persistent = persistent;
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("RequestType", messageType);
                    var arguments = new Dictionary<string, object>();
                    arguments.Add("x-queue-mode", "lazy");
                    model.QueueDeclare(queue: destination, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                    model.BasicPublish(exchange: "", routingKey: destination, basicProperties: properties, body: body);
                }
            }
        }

        private void SendMessageToExchange(string message, string destination, string routeKey, QueueDestination queueDestination, string messageType)
        {
            if (Connect())
            {
                using (var model = connection.CreateModel())
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    var properties = model.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("RequestType", messageType);
                    string exchange = "";
                    properties.Persistent = true;

                    if (queueDestination == QueueDestination.Queue)
                    {
                        exchange = destination + GroupSuffix;                      
                        model.BasicPublish(exchange: exchange, routingKey: routeKey, basicProperties: properties, body: body);                
                    }
                    else
                    {
                        model.BasicPublish(exchange: destination, routingKey: destination, basicProperties: properties, body: body);
                    }
                }
            }
        }

      

        object connectLock = new object();
        public bool Connect()
        {
            lock (connectLock)
            {
                if (connection != null && connection.IsOpen)
                    return true;
                var factory = new ConnectionFactory();
                factory.HostName = ipAddress;
                factory.Port = AmqpTcpEndpoint.UseDefaultPort;
                factory.UserName = username;
                factory.Password = password;

                connection = factory.CreateConnection(ServiceName);
                channel = connection.CreateModel();
                return connection.IsOpen;
            }
        }

        public void Close()
        {
            if (channel != null && channel.IsOpen)
                channel.Close();
            if (connection != null && connection.IsOpen)
                connection.Close();

        }

        public void Stop()
        {
            if (connection != null && connection.IsOpen)
            {
                connection.Close();
            }
        }


        public void Dispose()
        {
            if (channel != null)
                channel.Dispose();
            if (connection != null)
                connection.Dispose();

        }

      
    }
}
