using MoneyCounter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MoneyCounter.Repositories;
using Telegram.Bot;
using MoneyCounter.Wrappers;

namespace MoneyCounter.Analyzers
{

    //TODO refactor params
    class EntitiesFromDateToDateAnalyzer
    {
        public List<string> GetAnalysisText(TelegramBotClient botClient, int userId, DateTime fstDate, DateTime sndDate, string startStringAboutTotalExpense, string middleStringAboutNetProfit)
        {
            var wrapperOfExpenseData = GetDataAboutTransExSumsOfMoney(fstDate, sndDate);
            var wrapperOfGainsData = GetDataAboutTransGainsSumsOfMoney(fstDate, sndDate);

            return FormMessages(startStringAboutTotalExpense, middleStringAboutNetProfit,
                wrapperOfExpenseData.Sum, wrapperOfExpenseData.Text, wrapperOfGainsData.Sum, wrapperOfGainsData.Text);
        }

        private ExpensesAndGainsAnalyzerResults GetDataAboutTransExSumsOfMoney(DateTime fstDate, DateTime sndDate)
        {
            ExpensesAndGainsAnalyzerResults wrapper = new ExpensesAndGainsAnalyzerResults();
            TransactionRepository repos = new TransactionRepository();
            var transExpenses = repos.GetAllNoGainTransactionsFromTimeToTime(fstDate, sndDate);
            var startTransEx = transExpenses.OfType<Transaction>().Where(x => x.IsStart).OfType<AbstractFinanceEntity>().ToList();
            var reverseTransEx = transExpenses.OfType<Transaction>().Where(x => x.IsReversal).OfType<AbstractFinanceEntity>().ToList();
            wrapper.Sum = GetSumOfMoneyInFinanceEntities(startTransEx);
            wrapper.Sum -= GetSumOfMoneyInFinanceEntities(reverseTransEx);
            wrapper.Text = GetTextAboutTransEx(startTransEx, reverseTransEx); 
            return wrapper;
        }

        private ExpensesAndGainsAnalyzerResults GetDataAboutTransGainsSumsOfMoney(DateTime fstDate, DateTime sndDate)
        {
            ExpensesAndGainsAnalyzerResults wrapper = new ExpensesAndGainsAnalyzerResults();
            TransactionRepository repos = new TransactionRepository(); //here new Trans.., cause Gains now are in Transaction entity
            var gains = repos.GetAllGainsFromTimeToTime(fstDate, sndDate);
            wrapper.Sum = GetSumOfMoneyInFinanceEntities(gains);
            wrapper.Text = FormTextAboutGainSumsOfMoney(gains);
            return wrapper;
        }
        private string FormTextAboutGainSumsOfMoney(List<AbstractFinanceEntity> allGains)///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        {
            var repos = new TransactionRepository(); //here new Trans.., cause Gains now are in Transaction entity
            var gainCategories = repos.GetGainCategoryNamesFromPerformedEntities();
            var gainsSumList = FormListOfSumsOfMoneyAmountsFromCategories(allGains, gainCategories);
            string text = string.Join("\n", gainsSumList.Zip(gainCategories, (sum, catgName) => $"Сумма доходов из категории {catgName}:\n{sum} грн."));
            return text;
        }

        private List<string> FormMessages(string startStringAboutTotalExpense, string middleStringAboutNetProfit,
            int expensesTotalSum, string textAboutExpenses, int gainsTotalSum, string textAboutGains)
        {
            string textOfMsg1 = $"--------------------------------------------------------------------------------\n"
                 + startStringAboutTotalExpense +
                $"{expensesTotalSum} грн.\n" +
                $"Сумма доходов:\n" +
                $"{gainsTotalSum} грн.\n"
                 + middleStringAboutNetProfit +
                $"<b>{gainsTotalSum - expensesTotalSum} грн.</b>\n\n" +
                $"Расходы:\n" + textAboutExpenses;

            string textOfMsg2 = "Доходы:\n" + textAboutGains;
            return new List<string> { textOfMsg1, textOfMsg2 };
        }

        private int GetSumOfMoneyInFinanceEntities(List<AbstractFinanceEntity> entities) =>
            entities.Select(x => int.Parse(x.MoneyAmount.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0])).Sum();

        private string GetTextAboutTransEx(List<AbstractFinanceEntity> startTransEx, List<AbstractFinanceEntity> reverseTransEx)// отсюда смотрим
        {
            var repos = new TransactionRepository(); //here new Trans.., cause Expenses now are in Transaction entity
            var transExCategoriesNames = repos.GetCategoryNamesFromPerformedEntities();
            return FormTextAboutTransExSumsOfMoney(transExCategoriesNames, startTransEx, reverseTransEx);
        }
        private string FormTextAboutTransExSumsOfMoney(List<string> transExCategoriesNames, List<AbstractFinanceEntity> startTransEx,
            List<AbstractFinanceEntity> reverseTransEx)
        {
            var startTransExSumList = FormListOfSumsOfMoneyAmountsFromCategories(startTransEx, transExCategoriesNames);
            var reverseTransExSumList = FormListOfSumsOfMoneyAmountsFromCategories(reverseTransEx, transExCategoriesNames);
            string text = string.Join("\n", transExCategoriesNames.Zip(startTransExSumList.Zip(reverseTransExSumList, Tuple.Create), (catgName, sums) => $"Сумма расходов на категорию {catgName}:\n{sums.Item1-sums.Item2} грн."));
            return text;
        }

        private List<int> FormListOfSumsOfMoneyAmountsFromCategories(List<AbstractFinanceEntity> allEntitiesFromTimeToTime, List<string> categoriesNames)
        {
            var sumsOfMoney = categoriesNames.Select(x => GetSumOfMoneyInFinanceEntities(GetAllEntitiesWithCategoryName(x, allEntitiesFromTimeToTime)));
            return sumsOfMoney.ToList();
        }
        private List<AbstractFinanceEntity> GetAllEntitiesWithCategoryName(string someCategoryName, List<AbstractFinanceEntity> entities)
        {
            return entities.Where(x => x.Category == someCategoryName).ToList();
        }
    }
}
