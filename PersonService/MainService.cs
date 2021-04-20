using Common.MessageQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersonService.DBContexts;
using PersonService.MessageQueue;
using PersonService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Common.DependencyInjection.Utilities;

namespace PersonService
{
    public class MainService : IHostedService
    {
        QueueListener listener;
        IHostApplicationLifetime appLifetime;
        private readonly string queueName;
        private IMessageQueue messageQueue;
        private IPersonRepository _personRepository;
        private IEventRepository _eventRepository;
        private readonly PersonServiceContext _personServiceContext;

        public MainService(IHostApplicationLifetime appLifetime, IServiceProvider serviceProvider)
        {
            queueName = "EventQueue";
            this.appLifetime = appLifetime;
            messageQueue = DependencyInjection.GetService<IMessageQueue>();
            _personServiceContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PersonServiceContext>();
            _personRepository = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IPersonRepository>();
            _eventRepository = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IEventRepository>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.appLifetime.ApplicationStarted.Register(OnStarted);
            this.appLifetime.ApplicationStopping.Register(OnStopping);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        private void OnStarted()
        {
            StartService();
        }

        private void OnStopping()
        {
            StopService();
        }

        private void StartService()
        {
            listener = new QueueListener(queueName, messageQueue, _personRepository,_eventRepository);
            listener.StartListener();
        }
        private void StopService()
        {
            if (listener != null)
                listener.StopListener();
        }
        
    }
}
