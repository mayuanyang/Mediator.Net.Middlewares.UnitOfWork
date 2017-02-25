using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Autofac;
using Mediator.Net.Autofac;
using Mediator.Net.Middlewares.UnitOfWork.Test.Database;
using Mediator.Net.Middlewares.UnitOfWork.Test.Events;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;

namespace Mediator.Net.Middlewares.UnitOfWork.Test
{
    public class ExceptionInHandlerShouldRollbackTransaction : TestBase
    {
        private IContainer _container;
        private Guid _personId = Guid.NewGuid();
        private Guid _carId = Guid.NewGuid();
        private bool _exceptionHappened = false;
        public void GivenAMediatorWithUnitOfWorkAndMultiEventHandlers()
        {

            var builder = new MediatorBuilder();

            builder.RegisterHandlers(typeof(ExceptionInHandlerShouldRollbackTransaction).Assembly)
                .ConfigureGlobalReceivePipe(x =>
            {
                x.UseUnitOfWork(() => true);
            });

            ContainerBuilder.RegisterMediator(builder);

            _container = ContainerBuilder.Build();

        }

        public async Task WhenAEventIsPublishedAndOneOfTheHandlerThrowException()
        {
            try
            {
                var mediator = _container.Resolve<IMediator>();
                await mediator.PublishAsync(new PersonAndCarAddedEvent(_personId, "person", _carId, "Benz", true));
            }
            catch (Exception)
            {
                _exceptionHappened = true;
            }

        }

        public void ThenExceptionShouldHappen()
        {
            _exceptionHappened.ShouldBeTrue();
        }

        public void AndTheTransactionShouldBeRolledback()
        {
            
            var db = _container.Resolve<MyDbContext>();
            var per = db.Persons.SingleOrDefault(x => x.Id == _personId);
            per.ShouldBeNull();

            var car = db.Cars.SingleOrDefault(x => x.Id == _carId);
            car.ShouldBeNull();
        }

        [Test]
        public void Run()
        {
            this.BDDfy();
        }
    }
}
