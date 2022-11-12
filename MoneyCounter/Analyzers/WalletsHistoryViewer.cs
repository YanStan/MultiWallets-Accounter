
using MoneyCounter.Repositories;
using System;
using System.Linq;
using MoneyCounter.Models;
using System.Collections.Generic;

namespace MoneyCounter.Analyzers
{
    public class WalletsHistoryViewer
    {
        public string ViewAllTransactions(DateTime fstDate, DateTime sndDate)
        {
            var allTimedTransactions = GetAllTransactions(fstDate, sndDate);
            string msgText = $"Все трансакции за выбранный период\n(с {fstDate} по {sndDate}(UTC)):\n\n";
            return GetViewInfoFromTransactions(allTimedTransactions, msgText);
        }

        public string ViewSomeWalletTransactions(string someWallet, DateTime fstDate, DateTime sndDate)
        {
            var allTimedTransactions = GetAllTransactions(fstDate, sndDate)
                .Where(x=> x.FromWallet == someWallet || x.ToWallet == someWallet).ToList();
            string msgText = $"Все трансакции по кошельку \"{someWallet}\" за выбранный период (с {fstDate} по {sndDate}(UTC)):\n\n";
            return GetViewInfoFromTransactions(allTimedTransactions, msgText);
        }

        private string GetViewInfoFromTransactions(List<Transaction> allTimedTransactions, string msgText)
        {
            allTimedTransactions.ForEach(x =>
            {
                msgText += $"✅ id: {x.Id}\n" +
                           $"{x.FromWallet} => {x.ToWallet} ({x.MoneyAmount})\n" +
                           $"Автор: {x.FirstName}({x.Username})\n" +
                           $"{x.Subcategory}, {GetType(x.IsFinal, x.IsGain, x.IsReversal, x.IsStart)}\n" +
                           $"Дата и время: {x.DatetimeOfFinish}\n\n";
            });
            return msgText;
        }

        private static List<Transaction> GetAllTransactions(DateTime fstDate, DateTime sndDate)
        {
            TransactionRepository repos = new TransactionRepository();
            var allTimedTransactions = repos.GetAllEntitiesFromTimeToTime(fstDate, sndDate).OfType<Transaction>().ToList();
            return allTimedTransactions;
        }

        public string GetType(bool isFinal, bool isGain, bool isReversal, bool isStart)
        {
            if (isGain)
                return "Чистый доход";
            if (isReversal)
                return "Реверсивный";
            if (isStart && isFinal)
                return "И начальный, и конечный";
            if (isStart && !isFinal)
                return "Начальный";
            if (!isStart && isFinal)
                return "Конечный";
            if (!isStart && !isFinal)
                return "Не начальный и не конечный";
            return "⚠️ Error 7! Please write to @Yan_stan";
        }
    }
}
