using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonService.Models;
using PersonService.Repository;

namespace PersonService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {

        private readonly IEventRepository _eventRepository;
        private readonly IPersonRepository _personRepository;

        public EventController(IEventRepository eventRepository, IPersonRepository personRepository)
        {
            _eventRepository = eventRepository;
            _personRepository = personRepository;
        }
        // GET: api/Event
        [HttpGet]
        public IActionResult Get()
        {

            var Events = _eventRepository.GetEvents();
            return new OkObjectResult(Events);
        }

        // GET: api/Event/5
        [HttpGet("{id}", Name = "GetById")]
        public IActionResult Get(int id)
        {
            var evt = _eventRepository.GetEventById(id);
            return new OkObjectResult(evt);
        }

        // POST: api/Event
        [HttpPost]
        public IActionResult Post([FromBody] Event evt)
        {

            using (var scope = new TransactionScope())
            {
                Person person = _personRepository.GetPersonById(evt.Person.Id);
                bool isPersonExists = false;
                if(person != null) { isPersonExists = true; }
                _eventRepository.InsertEvent(evt, isPersonExists);
                scope.Complete();
                return new CreatedAtActionResult(nameof(Get), nameof(PersonController), new { id = evt.Id }, evt);
            }
        }

        // PUT: api/Event/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Event evt)
        {
            if (evt != null)
            {
                using (var scope = new TransactionScope())
                {
                    _eventRepository.UpdateEvent(evt);
                    scope.Complete();
                    return new OkResult();
                }
            }
            return new NoContentResult();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
