using MoneyCounter.Analyzers;
using MoneyCounter.Models;
using MoneyCounter.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneyCounter.Repositories
{
    class FoundersTransactionRepository : FinanceEntityRepository
    {
        public override string Name { get; set; } = "FOUNDERSTRANSACTION";

        public override AbstractFinanceEntity SetEntity(UserData u)
        {
            FoundersTransaction transaction = new FoundersTransaction();
            transaction.Category = u.UserStatusArray[4];
            transaction.Subcategory = u.UserStatusArray[6];
            transaction.DatetimeOfFinish = DateTime.UtcNow;
            transaction.FirstName = u.FirstName;
            transaction.Username = u.Username;
            transaction.UserId = u.UserId;
            transaction.MoneyAmount = u.UserText;
            var sourceWallet = u.UserStatusArray[7].Split(">")[0];
            var destWallet = u.UserStatusArray[7].Split(">")[1];
            transaction.FromWallet = sourceWallet;
            transaction.ToWallet = destWallet;
            FoundersTransaction completedTransaction = (FoundersTransaction)SetWalletsToEntity(sourceWallet, destWallet, transaction);
            db.FoundersTransactions.Add(completedTransaction);
            db.SaveChanges();
            return transaction;
        }
        public List<string> GetAllUsedFoundersWallets()
        {
            List<string> walletNames = new List<string> { };
            var foundersTransactions = db.FoundersTransactions;
            walletNames.AddRange(foundersTransactions.Select(x => x.ToWallet).ToList());
            walletNames.AddRange(foundersTransactions.Select(x => x.FromWallet).ToList());
            IEnumerable<IGrouping<string, string>> walletNamesGroupings = walletNames.GroupBy(name => name).OrderByDescending(gr => gr.Count());
            return walletNamesGroupings.Select(gr => gr.Key).ToList();
        }

        private AbstractFinanceEntity SetWalletsToEntity(string firstWallet, string secondWallet, AbstractFinanceEntity entity)
        {
            var prevWhereThisSourceWalletIsSource = db.FoundersTransactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == firstWallet && x.SumOnSourceWallet != null);
            var prevWhereThisSourceWalletIsAim = db.FoundersTransactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.ToWallet == firstWallet && x.SumOnSourceWallet != null);
            var prevSecondWalletWithMoney = db.FoundersTransactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.ToWallet == secondWallet && x.SumOnAimWallet != null);
            var prevSecondAndSourceWalletWithMoney = db.FoundersTransactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == secondWallet && x.SumOnAimWallet != null);

            //end get that sums in a currently specific ways
            //TODO change using variable "transaction" for second time in 2 dif ways!
            var calculator = new LastSumsInWalletsCalculator();
            entity = calculator.CalcNewMoneyAmountsOnSourceAndAimWalletsForFoundersTrans(entity, prevWhereThisSourceWalletIsSource, prevWhereThisSourceWalletIsAim,
                prevSecondWalletWithMoney, prevSecondAndSourceWalletWithMoney);
            return entity;
        }
        public override List<AbstractFinanceEntity> GetAllEntities()
            => db.FoundersTransactions.Where(x => x.FromWallet != null).ToList()
            .Select(x => (AbstractFinanceEntity)x).ToList();

        public override bool WasThisWalletEverUsed(string sourceWallet)
           => db.FoundersTransactions.OrderByDescending(item => item.Id).FirstOrDefault
                (x => x.FromWallet == sourceWallet || x.ToWallet == sourceWallet) != null;
        public override bool WasThisWalletSourceAndStart(string sourceWallet) => true;

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
            var transactionWhereFromWallet = db.FoundersTransactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == wallet);
            bool wasMoney = transactionWhereFromWallet != null ? true : false;
            if (wasMoney)
                sum = ParseMoneySum(transactionWhereFromWallet.SumOnSourceWallet);
            else
                sum = 0;
            return new TransactionStatusWrapper(wasMoney, sum, transactionWhereFromWallet);
        }

        private TransactionStatusWrapper GetMoneyStatusOnToAim(string wallet)
        {
            int sum;
            var transactionWhereToWallet = db.FoundersTransactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.ToWallet == wallet);
            bool wasMoney = transactionWhereToWallet != null;
            if (wasMoney)
                sum = ParseMoneySum(transactionWhereToWallet.SumOnAimWallet);
            else
                sum = 0;
            return new TransactionStatusWrapper(wasMoney, sum, transactionWhereToWallet);
        }

        private int ParseMoneySum(string sumText) => int.Parse(sumText.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);

        public override void AddCategoryWithSubcategory(string newCategoryName, string newSubcategoryName)
        => throw new NotImplementedException();
        public override void AddSubcategory(string newCategoryName, string subcategoryName)
        => throw new NotImplementedException();
        public override void DeleteCategory(string entityName)
        => throw new NotImplementedException();
        public override void DeleteSubcategory(string category, string subcategory)
        => throw new NotImplementedException();
        public override List<AbstractFinanceEntity> GetAllEntitiesFromTimeToTime(DateTime from, DateTime to)
        => throw new NotImplementedException();
        public override List<string> GetCategoryNamesFromPerformedEntities()
        => throw new NotImplementedException();
        public override string[] GetEntityCategoriesNames()
        => throw new NotImplementedException();
        public override string[] GetEntitySubcategoriesNames()
        => throw new NotImplementedException();
        public override string[] GetEntitySubcategoriesOfCategory(string categoryName)
        => throw new NotImplementedException();
        public override bool IsCategoryExists(string categoryName)
        => throw new NotImplementedException();
        public override bool IsFromWalletInDb(string firstWallet)
        => throw new NotImplementedException();
        public override bool IsSubcategoryExists(string subcategoryName)
        => throw new NotImplementedException();
        public override void RenameCategory(string oldCategoryName, string newCategoryName)
        => throw new NotImplementedException();
        public override void RenameSubcategory(string categoryName, string oldSubcategoryName, string newSubcategoryName)
        => throw new NotImplementedException();

    }
}
