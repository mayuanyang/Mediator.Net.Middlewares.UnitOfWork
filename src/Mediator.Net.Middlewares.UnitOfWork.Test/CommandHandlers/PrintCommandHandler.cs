using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Middlewares.UnitOfWork.Test.Commands;
using Mediator.Net.Middlewares.UnitOfWork.Test.Database;

namespace Mediator.Net.Middlewares.UnitOfWork.Test.CommandHandlers
{
    class PrintCommandHandler : ICommandHandler<PrintCommand>
    {
        private readonly MyDbContext _db;

        public PrintCommandHandler(MyDbContext db)
        {
            _db = db;
        }
        public async Task Handle(ReceiveContext<PrintCommand> context)
        {
            CommittableTransaction tx;
            if (context.TryGetService(out tx))
            {
                if (_db.Database.Connection.State != ConnectionState.Open)
                {
                     await _db.Database.Connection.OpenAsync();
                }
                   
                _db.Database.Connection.EnlistTransaction(tx);
            }

            var first = await _db.Persons.SingleAsync(x => x.Id == 1);
            first.FirstName = "new name";
            await _db.SaveChangesAsync();

            throw new NotImplementedException();
        }
    }
}
