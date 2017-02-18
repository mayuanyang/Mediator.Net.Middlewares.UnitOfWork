using System;
using System.Transactions;

namespace Mediator.Net.Middlewares.UnitOfWork
{
    public class TransactionConfigurator : ITransactionConfigurator
    {
        public TimeSpan Timeout { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
    }
}