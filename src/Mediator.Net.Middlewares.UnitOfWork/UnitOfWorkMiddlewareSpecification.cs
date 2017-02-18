using System;
using System.Threading.Tasks;
using System.Transactions;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;

namespace Mediator.Net.Middlewares.UnitOfWork
{
    public class UnitOfWorkMiddlewareSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : IContext<IMessage>
    {
        private readonly Func<bool> _shouldExecute;
        private readonly CommittableTransaction _committableTransaction;

        public UnitOfWorkMiddlewareSpecification(CommittableTransaction committableTransaction, Func<bool> shouldExecute)
        {
            _shouldExecute = shouldExecute;
            _committableTransaction = committableTransaction;
            
        }

        public bool ShouldExecute(TContext context)
        {
            if (_shouldExecute == null)
                return true;
            return _shouldExecute.Invoke();
        }

        public async Task ExecuteBeforeConnect(TContext context)
        {
            if (ShouldExecute(context))
            {
                context.RegisterService(_committableTransaction);
                await Task.FromResult(0).ConfigureAwait(false);
            }
        }

        public async Task ExecuteAfterConnect(TContext context)
        {
            if (ShouldExecute(context))
            {
                await Task.Factory.FromAsync(_committableTransaction.BeginCommit, _committableTransaction.EndCommit, null).ConfigureAwait(false);
            }
        }

        public void OnException(Exception ex, TContext context)
        {
            if (ShouldExecute(context))
            {
                _committableTransaction.Rollback();
            }
            throw ex;
        }
    }
}
