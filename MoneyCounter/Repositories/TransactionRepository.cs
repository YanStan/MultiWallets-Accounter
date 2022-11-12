using Microsoft.EntityFrameworkCore;
using MoneyCounter.Analyzers;
using MoneyCounter.Models;
using MoneyCounter.Wrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MoneyCounter.Repositories
{
    class TransactionRepository : FinanceEntityRepository
    {
        public override string Name { get; set; } = "TRANSACTION";


        public override void AddCategoryWithSubcategory(string newCategoryName, string newSubcategoryName)
        {
            var transactionCategory = new TransactionCategory
            {
                CategoryKey = newCategoryName,
                SubCategoryKey = newSubcategoryName
            };
            db.TransactionCategories.Add(transactionCategory);
            db.SaveChanges();
        }

        public override void AddSubcategory(string newCategoryName, string subcategoryName)
        {
            TransactionCategory transactionCategory = new TransactionCategory
            {
                CategoryKey = newCategoryName,
                SubCategoryKey = subcategoryName
            };
            db.TransactionCategories.Add(transactionCategory);
            db.SaveChanges();
        }
        public override void DeleteCategory(string entityName)
        {
            var categoriesToRemove = db.TransactionCategories.Where(x => x.CategoryKey == entityName);
            db.TransactionCategories.RemoveRange(categoriesToRemove);
            db.SaveChanges();
        }
        public override void DeleteSubcategory(string category, string subcategory)
        {
            var categoriesToRemove = db.TransactionCategories.Where(x => x.CategoryKey == category && x.SubCategoryKey == subcategory);
            db.TransactionCategories.RemoveRange(categoriesToRemove);
            db.SaveChanges();
        }
        public override void RenameCategory(string oldCategoryName, string newCategoryName)
        {
            var listOfOldNamedCategories = db.TransactionCategories.Where(x => x.CategoryKey == oldCategoryName).ToList();
            db.TransactionCategories.RemoveRange(listOfOldNamedCategories);
            db.SaveChanges();
            listOfOldNamedCategories.ForEach(x => x.CategoryKey = newCategoryName);
            db.TransactionCategories.AddRange(listOfOldNamedCategories);
            db.SaveChanges();
        }

        public override void RenameSubcategory(string categoryName, string oldSubcategoryName, string newSubcategoryName)
        {
            var listOfOldNamedCategories = db.TransactionCategories.Where(x => x.CategoryKey == categoryName && x.SubCategoryKey == oldSubcategoryName).ToList();
            db.TransactionCategories.RemoveRange(listOfOldNamedCategories);
            db.SaveChanges();
            listOfOldNamedCategories.ForEach(x => x.SubCategoryKey = newSubcategoryName);
            db.TransactionCategories.AddRange(listOfOldNamedCategories);
            db.SaveChanges();
        }

        public override bool IsSubcategoryExists(string subcategoryName) =>
            db.TransactionCategories.FirstOrDefault(x => x.SubCategoryKey == subcategoryName) != null;

        public override bool IsCategoryExists(string categoryName) =>
            db.TransactionCategories.FirstOrDefault(x => x.CategoryKey == categoryName) != null;

        public override bool IsFromWalletInDb(string firstWallet) //TODO refactor name
        {
            var transactionWhereFromWallet = db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == firstWallet);
            var transactionWhereToWallet = db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.ToWallet == firstWallet);
            if (transactionWhereFromWallet == null && transactionWhereToWallet == null)
                return false;
            else
                return true;
        }
        public bool IsTransactionIdExists(int transactionId) => GetTransactionById(transactionId) != null;

        public bool IsNextTransactionsWithThisWallets(int transactionId)
        {
            var transaction = GetTransactionById(transactionId);
            var nextTransaction = db.Transactions.OrderByDescending(item => item.Id)
                .FirstOrDefault(x => x.Id > transactionId
                    && (x.FromWallet == transaction.FromWallet || x.ToWallet == transaction.ToWallet
                    || x.FromWallet == transaction.ToWallet || x.ToWallet == transaction.FromWallet));
            return nextTransaction != null;
        }

        public Transaction GetTransactionById(int transactionsId) => db.Transactions
            .FirstOrDefault(x => x.Id == transactionsId);

        public void DeleteTransactionById(int transactionsId) 
        {
            db.Transactions.Remove(db.Transactions.FirstOrDefault(x => x.Id == transactionsId));
            db.SaveChanges();
        } 


        public override int GetLastSumOnWallet(string wallet)
        {
            var transFrS = GetMoneyStatusOnFromSource(wallet);
            var transToA = GetMoneyStatusOnToAim(wallet);
            if (transFrS.IsMoney && !transToA.IsMoney)//приват яна .. 
                return transFrS.Sum;
            else if (!transFrS.IsMoney && transToA.IsMoney)//.. приват яна
                return transToA.Sum;
            else if(transFrS.IsMoney && transToA.IsMoney)
            {
                if (transFrS.Transaction.DatetimeOfFinish > transToA.Transaction.DatetimeOfFinish)
                    return transFrS.Sum;
                else
                    return transToA.Sum;
            }
            else
                return 0;           
        }

        private TransactionStatusWrapper GetMoneyStatusOnFromSource(string wallet)
        {
            int sum;
            var transactionWhereFromWallet = db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == wallet);
            bool wasMoney = transactionWhereFromWallet != null;
            if (wasMoney)
                sum = ParseMoneySum(transactionWhereFromWallet.SumOnSourceWallet);
            else
                sum = 0;
            return new TransactionStatusWrapper(wasMoney, sum, transactionWhereFromWallet);
        }

        private TransactionStatusWrapper GetMoneyStatusOnToAim(string wallet)
        {
            int sum;
            var transactionWhereToWallet = db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.ToWallet == wallet);
            bool wasMoney = transactionWhereToWallet != null;
            if (wasMoney)
                sum = ParseMoneySum(transactionWhereToWallet.SumOnAimWallet);
            else
                sum = 0;
            return new TransactionStatusWrapper(wasMoney, sum, transactionWhereToWallet);
        }

        private int ParseMoneySum(string sumText) => int.Parse(sumText.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);

        public override bool WasThisWalletEverUsed(string sourceWallet)
            => db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == sourceWallet || x.ToWallet == sourceWallet) != null;

        public override bool WasThisWalletSourceAndStart(string sourceWallet)
        {
            var transactionWhereFromWallets = db.Transactions.OrderByDescending(item => item.Id).Where(x => x.FromWallet == sourceWallet).ToList();
            foreach (Transaction transaction in transactionWhereFromWallets)
            {
                if (transaction.IsStart)
                    return true;
            }
            return false;
        }

        public override string[] GetEntityCategoriesNames() => db.TransactionCategories.Select(x => x.CategoryKey).Distinct().ToArray();

        public override string[] GetEntitySubcategoriesNames() => db.TransactionCategories.Select(x => x.SubCategoryKey).Distinct().ToArray();


        public override string[] GetEntitySubcategoriesOfCategory(string categoryName)
        {
            return db.TransactionCategories.Where(x=> x.CategoryKey == categoryName).Select(x => x.SubCategoryKey).Distinct().ToArray();
        }

        public List<string> GetAllUsedCompanyWallets()
        {
            List<string> walletNames = FormTransactionWalletNamesList();
            walletNames.AddRange(GetAllUsedFoundersWallets());
            IEnumerable<IGrouping<string, string>> walletNamesGroupings = walletNames.GroupBy(name => name).OrderByDescending(gr => gr.Count());
            return walletNamesGroupings.Select(gr => gr.Key).ToList();
        }

        public List<string> GetUsedTransactionWallets()
        {
            List<string> walletNames = FormTransactionWalletNamesList();
            IEnumerable<IGrouping<string, string>> walletNamesGroupings = walletNames.GroupBy(name => name).OrderByDescending(gr => gr.Count());
            return walletNamesGroupings.Select(gr => gr.Key).ToList();
        }
        private List<string> FormTransactionWalletNamesList()
        {
            List<string> walletNames = new List<string> { };
            var transactions = db.Transactions.Where(x => x.IsFinal == false);
            walletNames.AddRange(transactions.Select(x => x.ToWallet).ToList());
            walletNames.AddRange(transactions.Select(x => x.FromWallet).ToList());
            walletNames.AddRange(db.Transactions.Where(x => x.IsFinal == true).Select(x => x.FromWallet).ToList());
            return walletNames;
        }
        public List<string> GetAllUsedFoundersWallets()
        {
            List<string> walletNames = new List<string> { };
            var foundersTransactions = db.FoundersTransactions;
            walletNames.AddRange(foundersTransactions.Select(x => x.ToWallet).ToList());
            walletNames.AddRange(foundersTransactions.Select(x => x.FromWallet).ToList());
            var transactions = db.Transactions.Where(x => x.IsStart == true);
            walletNames.AddRange(transactions.Select(x => x.FromWallet).ToList());

            IEnumerable<IGrouping<string, string>> walletNamesGroupings = walletNames.GroupBy(name => name).OrderByDescending(gr => gr.Count());
            return walletNamesGroupings.Select(gr => gr.Key).ToList();
        }

        public override List<string> GetCategoryNamesFromPerformedEntities()
            => db.Transactions.Where(x => x.IsGain == false && x.MoneyAmount != null).Select(x => x.Category).Distinct().ToList();

        public List<string> GetGainCategoryNamesFromPerformedEntities()
            => db.Transactions.Where(x => x.IsGain == true && x.MoneyAmount != null).Select(x => x.Category).Distinct().ToList();

        public override List<AbstractFinanceEntity> GetAllEntitiesFromTimeToTime(DateTime from, DateTime to)
            => db.Transactions.Where(x => x.DatetimeOfFinish > from && x.DatetimeOfFinish < to && x.FromWallet != null)
                .OfType<AbstractFinanceEntity>().ToList();
        public List<AbstractFinanceEntity> GetAllNoGainTransactionsFromTimeToTime(DateTime from, DateTime to)
            => db.Transactions.Where(x => x.IsGain == false && x.DatetimeOfFinish > from && x.DatetimeOfFinish < to && x.FromWallet != null)
                .OfType<AbstractFinanceEntity>().ToList();
        public List<AbstractFinanceEntity> GetAllGainsFromTimeToTime(DateTime from, DateTime to)
            => db.Transactions.Where(x => x.IsGain == true && x.DatetimeOfFinish > from && x.DatetimeOfFinish < to && x.FromWallet != null)
                .OfType<AbstractFinanceEntity>().ToList();


        public override List<AbstractFinanceEntity> GetAllEntities()
            => db.Transactions.Where(x => x.FromWallet != null).OfType<AbstractFinanceEntity>().ToList();

        public List<TransactionCategory> GetAllDbRowsOfTransactionCategory(string Name)
            => db.TransactionCategories.Where(x => x.CategoryKey == Name).ToList();

        public override AbstractFinanceEntity SetEntity(UserData u)
        {
            var firstWallet = u.UserStatusArray[6].Split(">")[0];
            var secondWallet = u.UserStatusArray[6].Split(">")[1];
            Transaction transaction = new Transaction
            {
                Category = u.UserStatusArray[3],
                Subcategory = u.UserStatusArray[5],
                MoneyAmount = u.UserStatusArray[7],
                DatetimeOfFinish = DateTime.UtcNow,
                FirstName = u.FirstName,
                Username = u.Username,
                UserId = u.UserId,
                FromWallet = firstWallet,
                ToWallet = secondWallet,
                IsStart = bool.Parse(u.UserStatusArray[9].Split("*")[0]),
                IsFinal = bool.Parse(u.UserStatusArray[9].Split("*")[1]),
                IsReversal = bool.Parse(u.UserStatusArray[9].Split("*")[2]),
                IsGain = bool.Parse(u.UserStatusArray[9].Split("*")[3])
            };
            var completedTransaction = (Transaction)SetWalletsToEntity(firstWallet, secondWallet, transaction);
            db.Transactions.Add(completedTransaction);
            db.SaveChanges();
            return transaction;
        }

        public void SetBalanceMultiplication(Transaction transaction, string multipliedAdjunctionSumText)
        {
            BalanceMultiplication bM = new BalanceMultiplication
            {
                AdjunctionSum = multipliedAdjunctionSumText,
                Transaction = transaction
            };
            db.BalanceMultiplications.Add(bM);
            db.SaveChanges();
        }


        public List<(string, Transaction)> GetTransactionsWithBMAdjustsInDict(DateTime from, DateTime to)
        {
            var filteredTransactions = GetTimedTransactionsWithBMsIds(from, to);
            var bMSums = filteredTransactions.Select(x => x.bM.AdjunctionSum).ToList();
            return filteredTransactions.Zip(bMSums, (trans, sum) => (sum, trans)).ToList();//.ToDictionary(s => s.sum, t => t.trans);
        }
        private List<Transaction> GetTimedTransactionsWithBMsIds(DateTime from, DateTime to)
        {
            return db.Transactions
                .Include(x => x.bM)
                .Where(x => x.bM != null && x.DatetimeOfFinish > from && x.DatetimeOfFinish < to && x.FromWallet != null)
                .ToList();
        }
 

        public int GetTimedSumOfBalanceMultiplications(DateTime from, DateTime to)
        {
            var bMs = GetTimedTransactionsWithBMsIds(from, to).Select(x => x.bM).ToList();
            return GetbMSum(bMs);
        }
        private static int GetbMSum(List<BalanceMultiplication> bMs) => (bMs.Count > 0) ?
            bMs.Select(x => int.Parse(x.AdjunctionSum.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0])).Sum() : 0;



        private AbstractFinanceEntity SetWalletsToEntity(string sourceWallet, string destWallet, AbstractFinanceEntity transaction)
        {
            //search for wallets with possible last changes of moneySums in current Source Wallet and AimWallet
            //TODO Selects with getting needed sums against entire enitites//cant do easily cause Calc compares also 2 datetimes of transactions.
            var prevWhereThisSourceWalletWasSource = db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == sourceWallet && x.SumOnSourceWallet != null);
            var prevWhereThisSourceWalletWasAim = db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.ToWallet == sourceWallet && x.SumOnSourceWallet != null);
            var prevWhereThisDestWalletWasSource = db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == destWallet && x.SumOnAimWallet != null);
            var prevWhereThisDestWalletWasAim = db.Transactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.ToWallet == destWallet && x.SumOnAimWallet != null);
            //end get that sums in a currently specific ways
            var calculator = new LastSumsInWalletsCalculator();
            switch(((Transaction)transaction).IsGain)
            {
                case true:
                    transaction = calculator.CalcNewMoneyAmountsOnSourceAndAimWalletsForGainsTrans(transaction, prevWhereThisSourceWalletWasSource, prevWhereThisSourceWalletWasAim,
                    prevWhereThisDestWalletWasAim, prevWhereThisDestWalletWasSource);
                    break;
                case false:
                    transaction = calculator.CalcNewMoneyAmountsOnSourceAndAimWallets(transaction, prevWhereThisSourceWalletWasSource, prevWhereThisSourceWalletWasAim,
                    prevWhereThisDestWalletWasAim, prevWhereThisDestWalletWasSource);
                    break;
            }
            return transaction;
        }
    }
}
