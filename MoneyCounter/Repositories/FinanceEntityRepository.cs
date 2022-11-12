using MoneyCounter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyCounter.Repositories
{
    public abstract class FinanceEntityRepository
    {
        protected readonly MoneyCounterContext db = new MoneyCounterContext();
        public abstract string Name { get; set; }
        public abstract void AddCategoryWithSubcategory(string newCategoryName, string newSubcategoryName);
        public abstract void AddSubcategory(string newCategoryName, string subcategoryName);
        public abstract void DeleteCategory(string entityName);
        public abstract void DeleteSubcategory(string category, string subcategory);
        public abstract void RenameCategory(string oldCategoryName, string newCategoryName);
        public abstract void RenameSubcategory(string categoryName, string oldSubcategoryName, string newSubcategoryName);
        public abstract string[] GetEntityCategoriesNames();
        public abstract string[] GetEntitySubcategoriesNames();
        public abstract string[] GetEntitySubcategoriesOfCategory(string categoryName);
        public abstract bool IsSubcategoryExists(string subcategoryName);
        public abstract bool IsCategoryExists(string categoryName);
        public abstract bool IsFromWalletInDb(string firstWallet);
        public abstract int GetLastSumOnWallet(string firstWallet);
        public abstract List<AbstractFinanceEntity> GetAllEntities();
        public abstract List<string> GetCategoryNamesFromPerformedEntities();
        public abstract List<AbstractFinanceEntity> GetAllEntitiesFromTimeToTime(DateTime from, DateTime to);
        public abstract bool WasThisWalletEverUsed(string sourceWallet);
        public abstract bool WasThisWalletSourceAndStart(string sourceWallet);
        public abstract AbstractFinanceEntity SetEntity(UserData u);

    }
}
