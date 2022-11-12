using MoneyCounter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace MoneyCounter.Wrappers
{
    class TransactionStatusWrapper
    {
        public bool IsMoney { get; }
        public int Sum { get; }
        public AbstractFinanceEntity Transaction { get; }

        public TransactionStatusWrapper(bool isSearchedMoney, int sum, AbstractFinanceEntity transaction)
        {
            this.IsMoney = isSearchedMoney;
            this.Sum = sum;
            this.Transaction = transaction;
        }
    }
}
