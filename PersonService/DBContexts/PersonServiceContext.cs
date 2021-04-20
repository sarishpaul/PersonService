using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonService.Models;

namespace PersonService.DBContexts
{
    public class PersonServiceContext :  DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Event> Events { get; set; }
        public PersonServiceContext(DbContextOptions<PersonServiceContext> options) : base(options)
        { }
        public PersonServiceContext() { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasData(
                new Person
                {
                    Id = 1,
                    Name = "Sarish Paul",
                    DOB = Convert.ToDateTime("1980-01-19"), 
                },
                new Person
                {
                    Id = 2,
                    Name = "Person 2",
                    DOB = Convert.ToDateTime("1998-11-01"),
                }
            );
            modelBuilder.Entity<Event>()
               .HasOne(z => z.Person);
        }
       
    }
}
