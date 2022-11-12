using MoneyCounter.Analyzers;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    class Custom2DatetimesOfAnalysisTextCatcher
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {       
            var textArray = u.UserText.Split("-");
            if (u.UserText == "Удалить трансакцию по номеру")
                return await RequestForTransNumberForDeletion(u.UserId, botClient);
            if (u.UserText == "Изменить сумму перевода по номеру")
                return GetMessages(await botClient.SendTextMessageAsync(u.UserId, "Not implemented yet!"));
            if (DateTime.TryParse(textArray[0], out DateTime fstDate) && DateTime.TryParse(textArray[1], out DateTime sndDate))
                return await SwitchFinanceAnalysisType(u, botClient, fstDate, sndDate);
            else
                return GetMessages(await botClient.SendTextMessageAsync(u.UserId, "⚠️ Неверный формат ввода двух дат. Попробуй еще раз."));
        }

        private static async Task<Messages> SwitchFinanceAnalysisType(UserData u, TelegramBotClient botClient,
            DateTime fstDate, DateTime sndDate)
        {
            var analysisType = u.UserStatusArray[5];
            string datetimeText = $"В диапазоне {u.UserText}\n";
            TypesOfTimeAnalysisHandler handler = new TypesOfTimeAnalysisHandler();
            return analysisType switch
            {
                "EXPANDGAINS" => await handler.AnalyzeForExpAndGains(botClient, u.UserId, fstDate, sndDate),
                "WALLETSSTATE" => await handler.AnalyzeForWallets(botClient, u.UserId, datetimeText, fstDate, sndDate),
                "BALANCEMULTIPLICATIONS" => await handler.ViewBalanceMultiplications(botClient, u.UserId, fstDate, sndDate),
                "WALLETSHISTORYALL" => await handler.ViewAllWalletsHistory(botClient, u.UserId, fstDate, sndDate),
                "WALLETSHISTORYONE" => await handler.ViewSomeWalletHistory(botClient, u.UserId, fstDate, sndDate),
                _ => GetMessages(await botClient.SendTextMessageAsync(u.UserId, "⚠️ Error №6! Please report to @Yan-stan")),
            };
        }

        private static async Task<Messages> RequestForTransNumberForDeletion(int userId, TelegramBotClient botClient)
        {
            UserRepository userRepository = new UserRepository();
            var delRepos = new TransactionsForDeletionRepository();
            delRepos.DeleteAllSpareEntities(userId);
            var msg = await botClient.SendTextMessageAsync(userId, $"Введи номер трансакции числом");
            userRepository.SetUserChatStatus(userId, $"WAIT/INPUTFOR/TRANSACTION/DELETION/TRANSID");
            return GetMessages(msg);
        }

        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
