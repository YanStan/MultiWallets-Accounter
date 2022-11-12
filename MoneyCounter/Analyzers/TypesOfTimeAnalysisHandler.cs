using MoneyCounter.Wrappers;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MoneyCounter.Analyzers
{
    class TypesOfTimeAnalysisHandler
    {
        public async Task<Messages> AnalyzeWalletsStatesForNdays(TelegramBotClient botClient, int userId, int daysAmount)
        {
            var fstDateTime = DateTime.UtcNow.Date.AddDays(1 - daysAmount);
            var scdDateTime = DateTime.UtcNow;
            string datetimeText = $"За последние {daysAmount} дней\n(с {fstDateTime} по {scdDateTime})\n";
            return await AnalyzeForWallets(botClient, userId, datetimeText, fstDateTime, scdDateTime);
        }

        public async Task<Messages> AnalyzeExpAndGainsForNdays(TelegramBotClient botClient,
            int userId, int daysAmount)
        {
            var scdDateTime = DateTime.UtcNow;
            var fstDateTime = scdDateTime.Date.AddDays(1 - daysAmount);
            return await AnalyzeForExpAndGains(botClient, userId, fstDateTime, scdDateTime);
        }
        public async Task<Messages> ViewBalanceMultiplicationsForNdays(TelegramBotClient botClient,
            int userId, int daysAmount)
        {
            var scdDateTime = DateTime.UtcNow;
            var fstDateTime = scdDateTime.Date.AddDays(1 - daysAmount);
            return await ViewBalanceMultiplications(botClient, userId, fstDateTime, scdDateTime);
        }
        public async Task<Messages> ViewAllWalletsHistoryForNdays(TelegramBotClient botClient, int userId, int daysAmount)
        {
            var scdDateTime = DateTime.UtcNow;
            var fstDateTime = scdDateTime.Date.AddDays(1 - daysAmount);
            return await ViewAllWalletsHistory(botClient, userId, fstDateTime, scdDateTime);
        }

        public async Task<Messages> ViewSomeWalletHistoryForNDays(TelegramBotClient botClient, int userId, int daysAmount)
        {
            var scdDateTime = DateTime.UtcNow;
            var fstDateTime = scdDateTime.Date.AddDays(1 - daysAmount);
            return await ViewSomeWalletHistory(botClient, userId, fstDateTime, scdDateTime);
        }


        public async Task<Messages> AnalyzeForWallets(TelegramBotClient botClient,
            int userId, string datetimeText, DateTime fstDate, DateTime sndDate)
        {
            var analyzer = new WalletsAnalyzer();
            var msgText = analyzer.GetFinalTextAfterValidation(datetimeText, fstDate, sndDate);
            MassiveMessagesAnswerer messagesAnswerer = new MassiveMessagesAnswerer();
            return await messagesAnswerer.MassiveTextMessageAnswer(botClient, msgText, userId);
        }
        public async Task<Messages> AnalyzeForExpAndGains(TelegramBotClient botClient,
            int userId, DateTime fstDate, DateTime sndDate)
        {
            var analyzer = new EntitiesFromDateToDateAnalyzer();
            string startStringAboutTotalExpense = $"✅ Общая сумма расходов за период с {fstDate} по {sndDate}:\n";
            string middleStringAboutNetProfit = $"Чистая прибыль за выбранный период:\n";
            var msgTexts = analyzer.GetAnalysisText(botClient, userId, fstDate, sndDate, startStringAboutTotalExpense, middleStringAboutNetProfit);
            MassiveMessagesAnswerer messagesAnswerer = new MassiveMessagesAnswerer();
            return await messagesAnswerer.MassiveMultipleMessagesAnswer(botClient, userId, msgTexts);
        }

        public async Task<Messages> ViewBalanceMultiplications(TelegramBotClient botClient,
            int userId, DateTime fstDate, DateTime sndDate)
        {
            var viewer = new BalanceMultiplicationsViewer();
            var msgText = viewer.GetViewText(fstDate, sndDate);
            MassiveMessagesAnswerer messagesAnswerer = new MassiveMessagesAnswerer();
            return await messagesAnswerer.MassiveTextMessageAnswer(botClient, msgText, userId);
        }

        public async Task<Messages> ViewAllWalletsHistory(TelegramBotClient botClient,
            int userId, DateTime fstDate, DateTime sndDate)
        {
            var viewer = new WalletsHistoryViewer();
            var msgText = viewer.ViewAllTransactions(fstDate, sndDate);
            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormTransactionsManipulating();
            MassiveMessagesAnswerer messagesAnswerer = new MassiveMessagesAnswerer();
            return await messagesAnswerer.MassiveTextMessageAnswer(botClient, msgText, userId, keyboard);
        }

        public async Task<Messages> ViewSomeWalletHistory(TelegramBotClient botClient,
            int userId, DateTime fstDate, DateTime sndDate)
        {
            var viewer = new WalletsHistoryViewer();
            UserRepository reposOfuser = new UserRepository();
            string someWallet = reposOfuser.GetUserChatStatus(userId).Split("/")[6];
            var msgText = viewer.ViewSomeWalletTransactions(someWallet, fstDate, sndDate);
            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormTransactionsManipulating();
            MassiveMessagesAnswerer messagesAnswerer = new MassiveMessagesAnswerer();
            return await messagesAnswerer.MassiveTextMessageAnswer(botClient, msgText, userId, keyboard);
        }
    }
}
