
using System.Linq;

namespace MoneyCounter.Repositories
{
    class FinanceEntityRepositoryFactory
    {
        private readonly FinanceEntityRepository[] repositoriesArray;
        public FinanceEntityRepositoryFactory() // конструктор
        {
            repositoriesArray = new FinanceEntityRepository[]
            {
                new TransactionRepository(),
                new FoundersTransactionRepository(),
            };
        }

        public FinanceEntityRepository GetRepositoryInstanceFromItsUpperName(string upperRepositoryName)  // метод который возвращает один из инстансов
            => repositoriesArray.FirstOrDefault(x => upperRepositoryName == x.Name);

        public bool IsRepositoryExists(string upperRepositoryName) => GetRepositoryInstanceFromItsUpperName(upperRepositoryName) != null;
    }
}
