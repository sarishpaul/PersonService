
using Moq;
using PersonService.Models;
using PersonService.Repository;
using Xunit;
using System;
using PersonService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace PersonService.Test
{
    public class PersonController_UnitTests
    {        
        public Mock<IPersonRepository> mock = new Mock<IPersonRepository>();

        [Fact]
        public void GetPersonById_ShouldRetuen_SamePerson()
        {
            Person person = new Person() { Name = "Sarish", DOB = DateTime.Parse("2000-10-10") };
            mock.Setup(p => p.GetPersonById(1)).Returns(person);
            PersonController emp = new PersonController(mock.Object);
            var result = emp.Get(1);
            var okResult = result as OkObjectResult;
            Assert.Equal(person, okResult.Value);
        }       
    }
}
