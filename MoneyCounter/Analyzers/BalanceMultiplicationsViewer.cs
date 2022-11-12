using System;
using System.Collections.Generic;
using System.Linq;
using MoneyCounter.Models;
using MoneyCounter.Repositories;

namespace MoneyCounter.Analyzers
{
    class BalanceMultiplicationsViewer
    {
        public string GetViewText(DateTime fstDate, DateTime sndDate)
        {
            string dateTimeText = $"(с {fstDate} по {sndDate} (UTC))";
            TransactionRepository repos = new TransactionRepository();
            var transactionsWithBMS = repos.GetTransactionsWithBMAdjustsInDict(fstDate, sndDate);
            return FormDetailedViewText(transactionsWithBMS, dateTimeText);
        }
        private string FormDetailedViewText(List<(string, Transaction)> transactionsWithBMs, string dateTimeText)
        {
            string walletsText = "";
            int sumsOfAdjunctions = 0;
            foreach (var fromWalletGrouping in transactionsWithBMs.GroupBy(x => x.Item2.FromWallet))
                walletsText += ViewTextForSourceWallet(fromWalletGrouping);
            transactionsWithBMs.ToList().ForEach(x=> sumsOfAdjunctions += GetInt(x.Item1));
            string headText = $"✅ Доп. доходы внутри балансов за выбранный период\n{dateTimeText}:\n\n" +
                $"{sumsOfAdjunctions} доп. грн.\nИз них зафиксировано:\n";
            string allText = headText + walletsText;
            return allText;
        }

        private static string ViewTextForSourceWallet(IGrouping<string, (string, Transaction)> fromWalletGrouping)
        {
            string text = $"При переводе с кошелька \"{fromWalletGrouping.Key}\":\n";
            foreach (var toWalletGrouping in fromWalletGrouping.GroupBy(x => x.Item2.ToWallet))
            {
                var fromToWalletsTransactionsSum = toWalletGrouping.Sum(transaction => int.Parse(transaction.Item1.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]));
                text += $"- На кошелек \"{toWalletGrouping.Key}\" ({fromToWalletsTransactionsSum} доп. грн)\n";
            }
            return text;
        }

        private static int GetInt(string textSum) => int.Parse(textSum.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]);

    }
}
