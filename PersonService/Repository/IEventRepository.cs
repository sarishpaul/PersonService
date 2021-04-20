using PersonService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonService.Repository
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetEvents();
        Event GetEventById(int EventId);
        Event InsertEvent(Event evt, bool isPersonExists);
        void UpdateEvent(Event evt);
        void Save();
    }
}
