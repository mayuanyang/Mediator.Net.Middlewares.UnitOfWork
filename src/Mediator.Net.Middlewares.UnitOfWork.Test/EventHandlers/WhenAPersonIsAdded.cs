using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Middlewares.UnitOfWork.Test.Database;
using Mediator.Net.Middlewares.UnitOfWork.Test.Events;

namespace Mediator.Net.Middlewares.UnitOfWork.Test.EventHandlers
{
    class WhenAPersonIsAdded : IEventHandler<PersonAndCarAddedEvent>
    {
        private readonly MyDbContext _db;

        public WhenAPersonIsAdded(MyDbContext db)
        {
            _db = db;
        }
        public async Task Handle(IReceiveContext<PersonAndCarAddedEvent> context)
        {
            CommittableTransaction tx;
            if (context.TryGetService(out tx))
            {
                if (_db.Database.Connection.State != ConnectionState.Open)
                {
                    _db.Database.Connection.Open();
                }
                _db.Database.Connection.EnlistTransaction(tx);
            }

            var per = new Person { Id = context.Message.PersonId, FirstName = context.Message.FirstName };
            _db.Persons.Add(per);
            await _db.SaveChangesAsync();
        }
    }
}
