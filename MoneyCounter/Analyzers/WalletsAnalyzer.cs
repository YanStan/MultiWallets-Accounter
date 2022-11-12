using MoneyCounter.Models;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MoneyCounter.Analyzers
{
    class WalletsAnalyzer
    {
        public string GetFinalTextAfterValidation(string datetimeText, DateTime fstDateTime, DateTime scdDateTime)
        {
            var repos = new TransactionRepository();
            List<Transaction> allTransactions = repos.GetAllEntities().OfType<Transaction>().ToList();
            List<Transaction> allTimedTransactions = repos.GetAllNoGainTransactionsFromTimeToTime(fstDateTime, scdDateTime).OfType<Transaction>().ToList();
            List<Transaction> allTimedGainTransactions = repos.GetAllGainsFromTimeToTime(fstDateTime, scdDateTime).OfType<Transaction>().ToList();
            int bMSum = repos.GetTimedSumOfBalanceMultiplications(fstDateTime, scdDateTime);
            int gainSum = GetSumsFromTransactions(allTimedGainTransactions.ToList()).Sum();
            string allTextAboutTransactions = FormAllTextAboutTransactions(datetimeText, allTransactions, allTimedTransactions,
                allTimedGainTransactions, bMSum, gainSum);
            return allTextAboutTransactions;     
        }

        private string FormAllTextAboutTransactions(string datetimeText, List<Transaction> allTransactions,
            List<Transaction> allTimedTransactions, List<Transaction> allTimedGainTransactions, int bMSum, int gainSum)
        {   
            WalletsAnalyzerResults wrapperOfSumOfDeposits = GetTextAboutSumOfDepositTransactions(allTimedTransactions, bMSum);
            WalletsAnalyzerResults wrapperOfSumOfSpendings = GetTextAboutSumOfSpendTransactions(allTimedTransactions);
            WalletsAnalyzerResults wrapperOfSumOfLeftovers = GetTextAboutSumOfLeftoversOnWallets(allTransactions, allTimedTransactions,
                allTimedGainTransactions, bMSum, gainSum);
            string gainsText = GetTextAboutGains(gainSum, allTimedGainTransactions);
            string checkText = GetCheckText(wrapperOfSumOfDeposits.Sum, wrapperOfSumOfSpendings.Sum,
                wrapperOfSumOfLeftovers.Sum, bMSum, gainSum);
            string allText = datetimeText + wrapperOfSumOfDeposits.Text + wrapperOfSumOfSpendings.Text + gainsText +
                wrapperOfSumOfLeftovers.Text + checkText;
            return allText;
        }

        private string GetCheckText(int sumOfDeposits, int sumOfSpendings, int sumOfLeftovers, int bMSum, int gainSum)
        {
            int checkSumOfMoney = sumOfDeposits + bMSum - sumOfSpendings - sumOfLeftovers - gainSum;
            string bMSumText = "";
            string gainText = "";
            if (bMSum != 0)
                bMSumText = $" + {bMSum}";
            if (gainSum != 0)
                gainText = $" - {gainSum}";

            string checkText = $"\nПроверка подсчета: {sumOfDeposits} грн{bMSumText} - {sumOfSpendings} - {sumOfLeftovers}{gainText} = {checkSumOfMoney} грн.\n";
            if (checkSumOfMoney == 0)
                checkText += "✅ Подсчет правильный!";
            else
                checkText += "❌ Подсчет неправильный! Произошла ошибка подсчета! Обратись к @Yan_stan";
            return checkText;
        }

        private WalletsAnalyzerResults GetTextAboutSumOfDepositTransactions(List<Transaction> allTimedTransactions, int bmSum)
        {
            var firstTransactions = allTimedTransactions.Where(x => x.IsStart == true).ToList();

            var wrapper1 = DetailedAnalysisText(firstTransactions);
            string textSumOfStartTransactions = wrapper1.Text;
            int sumInWallets = wrapper1.Sum;

            var timedReversalTransactions = allTimedTransactions.Where(x => x.IsReversal == true).ToList();
            int timedReversalSum = GetSumsFromTransactions(timedReversalTransactions).Sum();
            string reversalSumText = "";
            string bmSumText = "";
            if (bmSum != 0)
            {
                bmSumText = $"Было заработано умножением на балансах:  {bmSum} грн.\n";
            }
            if (timedReversalSum != 0)
            {
                var wrapper2 = DetailedAnalysisText(timedReversalTransactions);
                reversalSumText = $"<b>Из них возвращено обратно: {wrapper2.Sum} грн.</b>\n" + wrapper2.Text + 
                    $"<b>В итоге было заведено:</b> {sumInWallets}-{wrapper2.Sum} = {sumInWallets - wrapper2.Sum} грн.\n";
            }
            var wrapper = new WalletsAnalyzerResults();
            wrapper.Text = $"<b>Всего было заведено в систему:</b>\n{sumInWallets} грн\nИз них:\n{textSumOfStartTransactions}{reversalSumText}{bmSumText}\n";
            wrapper.Sum = sumInWallets -= timedReversalSum;
            return wrapper;
        }

        private WalletsAnalyzerResults DetailedAnalysisText(List<Transaction> firstTransactions)
        {
            string text = "";
            int sumInWallets = 0;
            foreach (var fromWalletGrouping in firstTransactions.GroupBy(x => x.FromWallet))
            {
                text += $"С кошелька \"{fromWalletGrouping.Key}\":\n";
                foreach (var toWalletGrouping in fromWalletGrouping.GroupBy(x => x.ToWallet))
                {
                    var fromToWalletsTransactionsSum = toWalletGrouping.Sum(transaction => int.Parse(transaction.MoneyAmount.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]));
                    text += $"- На кошелек \"{toWalletGrouping.Key}\" ({fromToWalletsTransactionsSum} грн)\n";
                    sumInWallets += fromToWalletsTransactionsSum;
                }
            }
            WalletsAnalyzerResults wrapper = new WalletsAnalyzerResults();
            wrapper.Sum = sumInWallets;
            wrapper.Text = text;
            return wrapper;
        }

        private WalletsAnalyzerResults GetTextAboutSumOfSpendTransactions(List<Transaction> allTimedTransactions)
        {
            List<Transaction> lastTransactions = allTimedTransactions.Where(x => x.IsFinal == true).ToList();
            var sumsInAimWallets = GetSumsFromTransactions(lastTransactions);
            int firstTransTotalSum = sumsInAimWallets.Sum();
            var listOfUniqueAimWallets = lastTransactions.Select(x => x.ToWallet).Distinct().ToList();

            var text = $"<b>Всего было потрачено:</b>\n{firstTransTotalSum} грн\nИз них:\n";
            text += string.Join("\n", listOfUniqueAimWallets.Zip(sumsInAimWallets, (x, y) => $"— На \"{x}\" ({y} грн)"));
            WalletsAnalyzerResults wrapper = new WalletsAnalyzerResults();
            wrapper.Text = text + "\n";
            wrapper.Sum = firstTransTotalSum;
            return wrapper;
        }

        private string GetTextAboutGains(int gainSum, List<Transaction> gainTransactions)
        {
            string text = "";
            if(gainSum != 0)
            {
                var sumsInAimWallets = GetSumsFromTransactions(gainTransactions);
                int totalSum = sumsInAimWallets.Sum();
                var listOfUniqueAimWallets = gainTransactions.Select(x => x.ToWallet).Distinct().ToList();
                text = $"<b>Всего было выведено как чистый доход:</b>\n{totalSum} грн\nИз них:\n";
                text += string.Join("\n", listOfUniqueAimWallets.Zip(sumsInAimWallets, (x, y) => $"— На \"{x}\" ({y} грн)"));
                text += "\n";
            }
            return text;
        }

        private WalletsAnalyzerResults GetTextAboutSumOfLeftoversOnWallets(List<Transaction> allTransactions,
            List<Transaction> allTimedTransactions, List<Transaction> allTimedGainTransactions, int bMSum, int gainSum)
        {
            var allNotFinalTransactions = allTransactions.Where(x => x.IsFinal == false).ToList();
            var listOfAllUniqueAimWallets = allNotFinalTransactions.Select(x => x.ToWallet).Distinct().ToList();
            var sumsInAimWallets = GetSumsOnAimNotFinalWallets(allTransactions, allNotFinalTransactions, listOfAllUniqueAimWallets);   
            int leftoverTotalSum = sumsInAimWallets.Sum();
            int leftoverTimedSum = GetLeftoverTimedSum(allTimedTransactions, allTimedGainTransactions, bMSum, gainSum);
            WalletsAnalyzerResults wrapper = FormTextAboutSumOfLeftoversOnWallets(listOfAllUniqueAimWallets, sumsInAimWallets, leftoverTimedSum, leftoverTotalSum);
            return wrapper;
        }

        private WalletsAnalyzerResults FormTextAboutSumOfLeftoversOnWallets(List<string> listOfAllUniqueAimWallets, List<int> sumsInAimWallets, int leftoverTimedSum,
            int leftoverTotalSum)
        {
            var text = $"<b>\nОсталось денег в системе:</b>\nЗа выбранный период: {leftoverTimedSum} грн\nЗа всё время: {leftoverTotalSum} грн\nИз них:\n";
            var infoStrings = listOfAllUniqueAimWallets.Zip(sumsInAimWallets, (x, y) => (AimWallet: x, Sum: y)).Where(x => x.Sum != 0).Select(x => $"— {x.Sum} грн на {x.AimWallet}");
            text += string.Join("\n", infoStrings);
            WalletsAnalyzerResults wrapper = new WalletsAnalyzerResults();
            wrapper.Text = text + "\n";
            wrapper.Sum = leftoverTimedSum;
            return wrapper;
        }

        private int GetLeftoverTimedSum(List<Transaction> allTimedTransactions, List<Transaction> allTimedGainTransactions, int timedBMSum, int timedGainSum)
        {
            var repos = new TransactionRepository();
            int timedSumOfStartTransactions = GetSumsFromTransactions(allTimedTransactions.Where(x => x.IsStart == true).ToList()).Sum();
            int timedSumOfFinalTransactions = GetSumsFromTransactions(allTimedTransactions.Where(x => x.IsFinal == true).ToList()).Sum();
            int timedReversalSum = GetSumsFromTransactions(allTimedTransactions.Where(x => x.IsReversal == true).ToList()).Sum(); //TODO single time in this class.
            int leftoverTimedSum = timedSumOfStartTransactions - timedSumOfFinalTransactions + timedBMSum - timedGainSum - timedReversalSum;
            return leftoverTimedSum;
        }

        private List<int> GetSumsFromTransactions(List<Transaction> transactions)
        {
            List<string> listOfUniqueAimWallets = transactions.Select(x => x.ToWallet).Distinct().ToList();
            List<int> sumsOfFinalWallets = new List<int> { };
            foreach (string uniqueAimWalletName in listOfUniqueAimWallets)
            {   //accrual - начисление           
                var transactionsWhereThisAimWallet = transactions.Where(x => x.ToWallet == uniqueAimWalletName);
                var SumOfAccruals = transactionsWhereThisAimWallet.Select(x => int.Parse(x.MoneyAmount.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0])).Sum();
                var transactionsWhereThisSourceWallet = transactions.Where(x => x.FromWallet == uniqueAimWalletName && x.IsStart != true);
                SumOfAccruals -= transactionsWhereThisSourceWallet.Select(x=> int.Parse(x.MoneyAmount.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0])).Sum();
                sumsOfFinalWallets.Add(SumOfAccruals);
            }
            return sumsOfFinalWallets;
        }

        private List<int> GetSumsOnAimNotFinalWallets(List<Transaction> allTransactions, List<Transaction> notFinalTransactions, List<string> listOfUniqueAimWallets)
        {
            List<int> sumsInAimWallets = new List<int> { };
            foreach (string uniqueAimWalletName in listOfUniqueAimWallets)
            {
                var notFinalTransaction = notFinalTransactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.ToWallet == uniqueAimWalletName);
                var lastTransactionWhereAimWasSource = allTransactions.OrderByDescending(item => item.Id).FirstOrDefault(x => x.FromWallet == uniqueAimWalletName);
                if (lastTransactionWhereAimWasSource!= null)
                    if(notFinalTransaction.DatetimeOfFinish < lastTransactionWhereAimWasSource.DatetimeOfFinish)
                        sumsInAimWallets.Add(int.Parse(lastTransactionWhereAimWasSource.SumOnSourceWallet.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]));
                    else
                        sumsInAimWallets.Add(int.Parse(notFinalTransaction.SumOnAimWallet.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]));
                else
                    sumsInAimWallets.Add(int.Parse(notFinalTransaction.SumOnAimWallet.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]));

            }
            return sumsInAimWallets;
        }
    }
}
