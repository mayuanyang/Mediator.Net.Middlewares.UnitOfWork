using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Mediator.Net.Autofac;
using Mediator.Net.Middlewares.UnitOfWork.Test.Commands;
using Mediator.Net.Middlewares.UnitOfWork.Test.Database;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;

namespace Mediator.Net.Middlewares.UnitOfWork.Test
{
    public class ExceptionInHandlerShouldRollbackTransaction : TestBase
    {
        private IContainer _container;
        public void GivenAMediatorWithUnitOfWork()
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

        public async Task WhenExceptionThrowInHandler()
        {
            try
            {
                var mediator = _container.Resolve<IMediator>();
                await mediator.SendAsync(new PrintCommand());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void ThenTransactionShouldBeRolledback()
        {
            
            var db = _container.Resolve<MyDbContext>();
            var per = db.Persons.Single(x => x.Id == 1);
            per.FirstName.ShouldBe("hello world");
        }

        [Test]
        public void Run()
        {
            this.BDDfy();
        }
    }
}
