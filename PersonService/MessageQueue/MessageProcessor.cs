using Common.MessageQueue;
using Common.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PersonService.Controllers;
using PersonService.Models;
using PersonService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PersonService.MessageQueue
{
    public class MessageProcessor
    {
        private IMessageQueue messageQueue;
        private IPersonRepository _personRepository;
        private IEventRepository _eventRepository;
        public MessageProcessor(IMessageQueue messageQueue, IPersonRepository personRepository, IEventRepository eventRepository)
        {
            this.messageQueue = messageQueue;
            this._personRepository = personRepository;
            this._eventRepository = eventRepository;
        }

        public void ProcessMessage(string message, IDictionary<string, object> headers)
        {

            try
            {
                if (headers != null && headers.ContainsKey("RequestType"))
                {
                    switch (Encoding.UTF8.GetString((byte[])headers["RequestType"]))
                    {
                        case "Event":
                            using (var scope = new TransactionScope())
                            {
                                EventPayload evt = Utils.DeSerializeObject<EventPayload>(message);
                                DateTime.TryParse(evt.PersonDOB, out DateTime dtDOB);
                                Person person = FindPerson(evt.PersonName, dtDOB);
                                bool isPersonExists = true;
                                if (person == null)
                                {
                                    person = new Person() { DOB = dtDOB, Name = evt.PersonName };
                                    _personRepository.InsertPerson(person);
                                    isPersonExists = false;
                                }
                                Event ev = new Event() { Name = evt.Name, TimeStamp = evt.TimeStamp, Person = person };
                                _eventRepository.InsertEvent(ev, isPersonExists);
                            }
                            break;
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        private Person FindPerson(string personname, DateTime persondob)
        {
            Person p = _personRepository.GetPersonByNameAndDob(personname, persondob);
            if (p != null)
            {
                return p;
            }
            return null;
        } 
    }
}
