using System;
using System.Transactions;

namespace Mediator.Net.Middlewares.UnitOfWork
{
    public interface ITransactionConfigurator
    {
        TimeSpan Timeout { set; }
        IsolationLevel IsolationLevel { set; }
    }
}
