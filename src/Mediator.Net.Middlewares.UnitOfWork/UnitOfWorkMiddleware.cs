using System;
using System.Transactions;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;

namespace Mediator.Net.Middlewares.UnitOfWork
{
    public static class UnitOfWorkMiddleware
    {
        public static void UseUnitOfWork<TContext>(this IPipeConfigurator<TContext>  configurator,  Func<bool> shouldExecute, Action<ITransactionConfigurator> configureTransaction = null, CommittableTransaction transaction = null)
            where TContext : IContext<IMessage>
        {
            var txConfigurator = new TransactionConfigurator
            {
                Timeout = TimeSpan.FromSeconds(30),
                IsolationLevel = IsolationLevel.ReadCommitted
            };
            configureTransaction?.Invoke(txConfigurator);
            if (transaction == null)
            {
                transaction = new CommittableTransaction(new TransactionOptions {Timeout = txConfigurator.Timeout, IsolationLevel = txConfigurator.IsolationLevel});
            }
          
            var spec = new UnitOfWorkMiddlewareSpecification<TContext>(transaction, shouldExecute);
            
            configurator.AddPipeSpecification(spec);
        }
    }
}
