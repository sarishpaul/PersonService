using Microsoft.EntityFrameworkCore;
using PersonService.DBContexts;
using PersonService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace PersonService.Repository
{
    public class EventRepository : IEventRepository
    {
        public readonly PersonServiceContext _dbContext;

        public EventRepository(PersonServiceContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Event GetEventById(int EventId)
        {
            return _dbContext.Events.Find(EventId);
        }


        public IEnumerable<Event> GetEvents()
        {
            return _dbContext.Events.ToList();
        }

        public Event InsertEvent(Event evt, bool isPersonExists)
        {
            if (isPersonExists)
            {
                _dbContext.Events.Attach(evt);
                _dbContext.Entry(evt).State = EntityState.Added;
            }
            else
            {
                _dbContext.Events.Add(evt);
            }

            Save();
            return evt;
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void UpdateEvent(Event evt)
        {
            throw new NotImplementedException();
        }
    }
}
