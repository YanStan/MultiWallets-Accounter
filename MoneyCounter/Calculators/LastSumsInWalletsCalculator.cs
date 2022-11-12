using System;
using MoneyCounter.Models;

namespace MoneyCounter.Analyzers
{
    class LastSumsInWalletsCalculator
    {
        //TODO refactor params n names
        public AbstractFinanceEntity CalcNewMoneyAmountsOnSourceAndAimWallets(AbstractFinanceEntity transaction,
            AbstractFinanceEntity prevWhereThisSourceWalletWasSource, AbstractFinanceEntity prevWhereThisSourceWalletWasAim,
            AbstractFinanceEntity prevWhereThisDestWalletWasAim, AbstractFinanceEntity prevWhereThisDestWalletWasSource)
        {
            var arrayOfTransMoneyAmount = transaction.MoneyAmount.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var currencyString = arrayOfTransMoneyAmount[1];
            var transactionMoneyAmount = int.Parse(arrayOfTransMoneyAmount[0]);

            transaction = SetMoneyOfSourceWalletToTransaction(transaction, prevWhereThisSourceWalletWasSource, prevWhereThisSourceWalletWasAim,
            transactionMoneyAmount, currencyString);
            transaction = SetMoneyOfAimWalletToTransaction(transaction, prevWhereThisDestWalletWasAim, prevWhereThisDestWalletWasSource,
            transactionMoneyAmount, currencyString);
            return transaction;
        }
        public AbstractFinanceEntity CalcNewMoneyAmountsOnSourceAndAimWalletsForFoundersTrans(AbstractFinanceEntity transaction,
            AbstractFinanceEntity prevWhereThisSourceWalletWasSource, AbstractFinanceEntity prevWhereThisSourceWalletWasAim,
            AbstractFinanceEntity prevWhereThisDestWalletWasAim, AbstractFinanceEntity prevWhereThisDestWalletWasSource)
        {
            var arrayOfTransMoneyAmount = transaction.MoneyAmount.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var currencyString = arrayOfTransMoneyAmount[1];
            var transactionMoneyAmount = int.Parse(arrayOfTransMoneyAmount[0]);

            transaction = SetMoneyOfSourceWalletToFoundersTransaction(transaction, prevWhereThisSourceWalletWasSource, prevWhereThisSourceWalletWasAim,
            transactionMoneyAmount, currencyString);
            transaction = SetMoneyOfAimWalletToTransaction(transaction, prevWhereThisDestWalletWasAim, prevWhereThisDestWalletWasSource,
            transactionMoneyAmount, currencyString);
            return transaction;
        }

        public AbstractFinanceEntity CalcNewMoneyAmountsOnSourceAndAimWalletsForGainsTrans(AbstractFinanceEntity transaction,
            AbstractFinanceEntity prevWhereThisSourceWalletWasSource, AbstractFinanceEntity prevWhereThisSourceWalletWasAim,
            AbstractFinanceEntity prevWhereThisDestWalletWasAim, AbstractFinanceEntity prevWhereThisDestWalletWasSource
            )
        {
            var arrayOfTransMoneyAmount = transaction.MoneyAmount.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var currencyString = arrayOfTransMoneyAmount[1];
            var transactionMoneyAmount = int.Parse(arrayOfTransMoneyAmount[0]);

            transaction = SetMoneyOfSourceWalletToFoundersTransaction(transaction, prevWhereThisSourceWalletWasSource, prevWhereThisSourceWalletWasAim,
            transactionMoneyAmount, currencyString);
            transaction = SetMoneyOfAimWalletToGainTransaction(transaction, prevWhereThisDestWalletWasAim, prevWhereThisDestWalletWasSource,
            currencyString);
            return transaction;
        }

        private AbstractFinanceEntity SetMoneyOfSourceWalletToTransaction(AbstractFinanceEntity transaction,
            AbstractFinanceEntity prevWhereThisSourceWalletWasSource, AbstractFinanceEntity prevWhereThisSourceWalletWasAim,
            int transactionMoneyAmount, string currencyString)
        {       
            if (prevWhereThisSourceWalletWasSource == null && prevWhereThisSourceWalletWasAim == null)
                transaction.SumOnSourceWallet = "-0 " + currencyString;//$"-{transaction.MoneyAmount}";
            else
            {
                int newMoneySum = CalcSumIfPreviousSourceWalletExists(prevWhereThisSourceWalletWasSource,
                    prevWhereThisSourceWalletWasAim, transactionMoneyAmount);
                if (newMoneySum >= 0)
                    transaction.SumOnSourceWallet = newMoneySum.ToString() + " " + currencyString;
                else
                    transaction.SumOnSourceWallet = "-0 " + currencyString;
            }
            return transaction;
        }

        private AbstractFinanceEntity SetMoneyOfSourceWalletToFoundersTransaction(AbstractFinanceEntity transaction,
            AbstractFinanceEntity prevWhereThisSourceWalletWasSource, AbstractFinanceEntity prevWhereThisSourceWalletWasAim,
            int transactionMoneyAmount, string currencyString)
        {
            if (prevWhereThisSourceWalletWasSource == null && prevWhereThisSourceWalletWasAim == null)
                transaction.SumOnSourceWallet = $"-{transaction.MoneyAmount}";
            else
            {
                int newMoneySum = CalcSumIfPreviousSourceWalletExists(prevWhereThisSourceWalletWasSource,
                    prevWhereThisSourceWalletWasAim, transactionMoneyAmount);
                transaction.SumOnSourceWallet = newMoneySum.ToString() + " " + currencyString;
            }
            return transaction;
        }

        private int CalcSumIfPreviousSourceWalletExists(AbstractFinanceEntity prevWhereThisSourceWalletWasSource,
            AbstractFinanceEntity prevWhereThisSourceWalletWasAim, int transactionMoneyAmount)
        {
            int previousSumOnSourceWallet;
            if (prevWhereThisSourceWalletWasSource != null && prevWhereThisSourceWalletWasAim == null)
                previousSumOnSourceWallet = int.Parse(prevWhereThisSourceWalletWasSource.SumOnSourceWallet.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
            else if (prevWhereThisSourceWalletWasSource == null && prevWhereThisSourceWalletWasAim != null)
                previousSumOnSourceWallet = int.Parse(prevWhereThisSourceWalletWasAim.SumOnAimWallet.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
            else// != null && != null
            {
                if (prevWhereThisSourceWalletWasSource.DatetimeOfFinish > prevWhereThisSourceWalletWasAim.DatetimeOfFinish)
                    previousSumOnSourceWallet = int.Parse(prevWhereThisSourceWalletWasSource.SumOnSourceWallet.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
                else
                    previousSumOnSourceWallet = int.Parse(prevWhereThisSourceWalletWasAim.SumOnAimWallet.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            int newMoneySum = -transactionMoneyAmount + previousSumOnSourceWallet;
            return newMoneySum;
        }

        private AbstractFinanceEntity SetMoneyOfAimWalletToTransaction(AbstractFinanceEntity transaction, 
            AbstractFinanceEntity prevWhereThisDestWalletWasAim, AbstractFinanceEntity prevWhereThisDestWalletWasSource,
            int transactionMoneyAmount, string currencyString)
        {        
            if (prevWhereThisDestWalletWasAim == null && prevWhereThisDestWalletWasSource == null)
                transaction.SumOnAimWallet = transaction.MoneyAmount;
            else
            {
                int previousSumOnAimWallet = GetPreviousSumIfPreviousAimWalletExists(prevWhereThisDestWalletWasAim, prevWhereThisDestWalletWasSource);
                int newMoneySum = transactionMoneyAmount + previousSumOnAimWallet;
                transaction.SumOnAimWallet = newMoneySum.ToString() + " " + currencyString;
            }
            return transaction;
        }

        private AbstractFinanceEntity SetMoneyOfAimWalletToGainTransaction(AbstractFinanceEntity transaction,
            AbstractFinanceEntity prevWhereThisDestWalletWasAim, AbstractFinanceEntity prevWhereThisDestWalletWasSource,
            string currencyString)
        {
            if (prevWhereThisDestWalletWasAim == null && prevWhereThisDestWalletWasSource == null)
            {
                transaction.SumOnAimWallet = transaction.MoneyAmount;
                return transaction;
            }         
            else
            {
                int previousSumOnAimWallet = GetPreviousSumIfPreviousAimWalletExists(prevWhereThisDestWalletWasAim, prevWhereThisDestWalletWasSource);
                transaction.SumOnAimWallet = previousSumOnAimWallet.ToString() + " " + currencyString;
                return transaction;
            }
        }

        private int GetPreviousSumIfPreviousAimWalletExists(AbstractFinanceEntity prevWhereThisDestWalletWasAim,
            AbstractFinanceEntity prevWhereThisDestWalletWasSource)
        {
            int previousSumOnAimWallet;
            if (prevWhereThisDestWalletWasAim != null && prevWhereThisDestWalletWasSource == null)
                previousSumOnAimWallet = int.Parse(prevWhereThisDestWalletWasAim.SumOnAimWallet.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
            else if (prevWhereThisDestWalletWasAim == null && prevWhereThisDestWalletWasSource != null)
                previousSumOnAimWallet = int.Parse(prevWhereThisDestWalletWasSource.SumOnSourceWallet.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
            else// != null && != null
            {
                if (prevWhereThisDestWalletWasAim.DatetimeOfFinish > prevWhereThisDestWalletWasSource.DatetimeOfFinish)
                    previousSumOnAimWallet = int.Parse(prevWhereThisDestWalletWasAim.SumOnAimWallet.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
                else
                    previousSumOnAimWallet = int.Parse(prevWhereThisDestWalletWasSource.SumOnSourceWallet.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            return previousSumOnAimWallet;
        }
    }
}
