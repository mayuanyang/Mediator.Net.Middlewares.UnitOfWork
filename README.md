![Build status](https://ci.appveyor.com/api/projects/status/c3j3jfuvpe5cafrp?svg=true) [![Mediator.Net on Stack Overflow](https://img.shields.io/badge/stack%20overflow-Mediator.Net-yellowgreen.svg)](http://stackoverflow.com/questions/tagged/memdiator.net)

# Mediator.Net.Middlewares.UnitOfWork
Middleware for Mediator.Net to support unit of work, it is a Middleware for Mediator.Net's pipeline. 
This middleware will add a CommittableTransaction into the context, handlers require unit of work can then get it and enlist the transaction

## Setup
Create a BusBuilder as normal and add this middleware into the pipeline you prefer
```C#

	var builder = new MediatorBuilder();

    builder.RegisterHandlers(typeof(ExceptionInHandlerShouldRollbackTransaction).Assembly)
        .ConfigureGlobalReceivePipe(x =>
    {
        x.UseUnitOfWork(() => true);
    });

                   
```

## Use UnitOfWork From Handlers
This example has two handlers that handle the same event PersonAndCarAddedEvent, class WhenACarIsAdded will try to add the Car into database and WhenAPersonIsAdded is add a Person into database, both handlers will then share the same CommittableTransaction hence will be in one unit of work, any exception being thrown in the handlers will cause the CommittableTransaction to rollback.
```
	class WhenACarIsAdded : IEventHandler<PersonAndCarAddedEvent>
    {
        private readonly MyDbContext _db;

        public WhenACarIsAdded(MyDbContext db)
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

            var car = new Car {Id = context.Message.CarId, Name = context.Message.CarName};
            _db.Cars.Add(car);
            await _db.SaveChangesAsync();
            if (context.Message.ShouldThrow)
            {
                throw new Exception(nameof(WhenACarIsAdded));
            }
            
        }
    }
	
	
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

```

## Mediator.Net
[Mediator.Net](https://github.com/mayuanyang/Mediator.Net) - A simple mediator for .Net for sending command, publishing event and request response with pipelines supported.
