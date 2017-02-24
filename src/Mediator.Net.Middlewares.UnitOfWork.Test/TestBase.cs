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
            var dbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitOfWork.mdf");
            var dbLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitOfWork_log.ldf");
            try
            {
                File.Delete(dbFile);
                File.Delete(dbLogFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var fromdbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.ToUpper().Replace("BIN\\DEBUG", ""), "APP_Data\\UnitOfWork.mdf");
            var fromdbLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.ToUpper().Replace("BIN\\DEBUG", ""), "APP_DATA\\UnitOfWork_log.ldf");
            File.Copy(fromdbFile, dbFile);
            File.Copy(fromdbLogFile, dbLogFile);

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
