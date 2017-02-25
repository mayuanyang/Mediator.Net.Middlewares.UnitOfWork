using System;
using System.IO;
using Autofac;
using Autofac.Core;
using Mediator.Net.Autofac;
using Mediator.Net.Binding;
using Mediator.Net.Middlewares.UnitOfWork.Test.Database;
using NUnit.Framework;

namespace Mediator.Net.Middlewares.UnitOfWork.Test
{
    public class TestBase
    {
        protected ContainerBuilder ContainerBuilder;
        protected IContainer Container;
        [OneTimeSetUp]
        public void Setup()
        {
            ContainerBuilder = new ContainerBuilder();
            ContainerBuilder.RegisterType<MyDbContext>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            MessageHandlerRegistry.MessageBindings.Clear();

        }
    }
}
