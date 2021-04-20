using PersonService.DBContexts;
using PersonService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonService.Repository
{
    public class PersonRepository: IPersonRepository
    {
        public readonly PersonServiceContext _dbContext;
        public PersonRepository(PersonServiceContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Get all persons
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/Person
        ///     {        
        ///       "Name": "Mike",
        ///       "DOB": "2020-12-10"
        ///     }
        /// </remarks>
        /// <returns></returns>
        public IEnumerable<Person> GetPersons()
        {
            return _dbContext.Persons.ToList();
        }
        public Person GetPersonById(int PersonId)
        {
            return _dbContext.Persons.Find(PersonId);
        }
        public Person GetPersonByNameAndDob(string name, DateTime dob)
        {           
            return _dbContext.Persons.FirstOrDefault(p => p.Name.ToLower()==name.ToLower() && p.DOB.Date == dob.Date);
        }
        public Person InsertPerson(Person person)
        {
            _dbContext.Persons.Add(person);
            Save();
            return person;
        }
        public void UpdatePerson(Person Person)
        {
            throw new NotImplementedException();
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
      
    }
}
