using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.EventHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.EventHandlers.Hotels
{
    [TestClass]
    public class When_Handling_HotelUpdatedEvent : With_EventHandler<HotelUpdatedEvent, Hotel,IHotelRepository>
    {
        private readonly int _hotelId = 42;
        private readonly Guid _hotelAggregatedId = Guid.NewGuid();

        private Hotel _hotel;

        protected override IMessageHandler<HotelUpdatedEvent> Given()
        {
            _hotel = new Hotel()
                         {
                             Name = "Test Beach Hotel",
                             Description = "A nice hotel situated right at Test Beach",
                             ResortName = "Test Beach",
                             Image = "http://test.com/test.jpg",
                             Latitude = 12.0f,
                             Longitude = 32.4f
                         };

            A.CallTo(() => IdentityMapperFake.GetModelId<Hotel>(_hotelAggregatedId)).Returns(_hotelId);
            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(_hotel);
            
            return new HotelEventHandlers(ObjectContextFake,RepositoryFake, IdentityMapperFake);
        }

        protected override HotelUpdatedEvent When()
        {
            return new HotelUpdatedEvent()
                       {
                           HotelAggregateId = _hotelAggregatedId,
                           Name = "Testy Beach Resort",
                           Description = "A nice resort situated right at Testy Beach",
                           ImageUrl = "http://test.com/testar.jpg",
                       };
        }

        [TestMethod]
        public void Then_Hotel_Name_Is_Updated()
        {
            Assert.AreEqual("Testy Beach Resort", _hotel.Name);
        }

        [TestMethod]
        public void Then_Hotel_Description_Is_Updated()
        {
            Assert.AreEqual("A nice resort situated right at Testy Beach", _hotel.Description);
        }

        [TestMethod]
        public void Then_Hotel_Image_Is_Updated()
        {
            Assert.AreEqual("http://test.com/testar.jpg", _hotel.Image);
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}