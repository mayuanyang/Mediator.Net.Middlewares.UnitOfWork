using System;
using Mediator.Net.Contracts;

namespace Mediator.Net.Middlewares.UnitOfWork.Test.Events
{
    class PersonAndCarAddedEvent : IEvent
    {
   
        public Guid PersonId { get; }
        public string FirstName { get; }
        public Guid CarId { get; }
        public string CarName { get; }
        public bool ShouldThrow { get; }

        public PersonAndCarAddedEvent(Guid personId, string firstName, Guid carId, string carName, bool shouldThrow)
        {
            PersonId = personId;
            FirstName = firstName;
            CarId = carId;
            CarName = carName;
            ShouldThrow = shouldThrow;
        }
    }
}
