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
            var table_db = new Mock<TableStorageUser>();
            var que_db = new Mock<QueStorage>();
            var service = new UserService(table_db.Object, que_db.Object);
        }



        [Test]
        public void Test1()
        {
            var table_db = new Mock<TableStorageUser>();
            var que_db = new Mock<QueStorage>();
            var service = new UserService(table_db.Object, que_db.Object);


            var user = new UserDAL { FirstName = "Gentle", LastName = "Possel", UserId = "12345" };
            var result = service.GetEntity("12345", "GentlePossel");

            Assert.AreEqual(user.FirstName, result.Result.FirstName);
            Assert.AreEqual(user.LastName, result.Result.LastName);
            Assert.NotNull(result);

            Assert.Pass();
        }
    }
}