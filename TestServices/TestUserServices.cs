using BuyMyHouse.Domain;
using BuyMyHouse.Infrastructure;
using BuyMyHouse.Services;
using Moq;
using NUnit.Framework;

namespace TestServices
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var db = new Mock<TableStorageUser>();
            var service = new UserService(db.Object);
        }



        [Test]
        public void Test1()
        {
            // arrange
            var db = new Mock<TableStorageUser>();
            var service = new UserService(db.Object);


            // act
            var user = new UserDAL { FirstName = "Gentle", LastName = "Possel", UserId = "12345" };
      //      service.
            var result = service.GetEntity("12345", "GentlePossel");

            // assert
            Assert.AreEqual(user.FirstName, result.Result.FirstName);
            Assert.AreEqual(user.LastName, result.Result.LastName);
            Assert.NotNull(result);

            Assert.Pass();
        }
    }
}