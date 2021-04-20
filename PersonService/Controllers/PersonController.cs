using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonService.Repository;
using PersonService.Models;
using static Common.DependencyInjection.Utilities;
using Common.MessageQueue;
using Common.Utilities;

namespace PersonService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;

        public PersonController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }
        // GET: api/Person
        [HttpGet]
        public IActionResult Get()
        {
            var Persons = _personRepository.GetPersons();
            return new OkObjectResult(Persons);
        }

        // GET: api/Person/5
        [HttpGet("{id}", Name = "GetByPersonId")]
        public IActionResult Get(int id)
        {
            var Person = _personRepository.GetPersonById(id);
            return new OkObjectResult(Person);
        }
              

        // POST: api/Person
        [HttpPost]
        public IActionResult Post([FromBody] Person person)
        {
            using (var scope = new TransactionScope())
            {
                Person p = _personRepository.InsertPerson(person);
                scope.Complete();
                var ss = DependencyInjection.GetService<IMessageQueue>();
                ss.SendMessage(Utils.SerializeObject<Person>(person), "PersonQueue", "Person");
                return new CreatedAtActionResult(nameof(Get), nameof(PersonController), new { id = person.Id }, person);
            }
        }

        // PUT: api/Person/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Person person)
        {
            if (person != null)
            {
                using (var scope = new TransactionScope())
                {
                    _personRepository.UpdatePerson(person);
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
