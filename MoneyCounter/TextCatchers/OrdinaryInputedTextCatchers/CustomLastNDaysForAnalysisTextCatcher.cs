using MoneyCounter.Analyzers;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    public class CustomLastNDaysForAnalysisTextCatcher
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string nDays = u.UserText;
            if (u.UserText == "Удалить трансакцию по номеру")
                return await RequestForTransNumberForDeletion(u.UserId, botClient);
            if (u.UserText == "Изменить сумму перевода по номеру")
                return GetMessages(await botClient.SendTextMessageAsync(u.UserId, "Not implemented yet!"));
            if (nDays.StartsWith("-"))
                return GetMessages(await botClient.SendTextMessageAsync(u.UserId, "⚠️ Для введения отрицательных дней нужна машина времени.\n" +
                    "Машина времени ожидает вас на углу Братиславской и Электротехнической, с 14 до 18 вечера 25.04.2256 в г. Киев."));
            if (int.TryParse(nDays, out int daysAmount))
                return await SwitchFinanceAnalysisType(botClient, u.UserId, daysAmount);
            else
                return GetMessages(await botClient.SendTextMessageAsync(u.UserId, "⚠️ Неверный формат ввода кол-ва дней. Попробуй еще раз."));
        }

        private static async Task<Messages> SwitchFinanceAnalysisType(TelegramBotClient botClient, int userId, int daysAmount)
        {
            if(daysAmount > 400000)
                return GetMessages(await botClient.SendTextMessageAsync(userId, 
                    $"⚠️ Записи за {daysAmount} последних дней отсутствуют, так как были сделаны на папирусе.\n" +
                    $"Пожалуйста, введите другое значение."));
            UserRepository reposOfUser = new UserRepository();
            var userStatusArray = reposOfUser.GetUserChatStatus(userId).Split("/");
            var analysisType = userStatusArray[5];
            TypesOfTimeAnalysisHandler handler = new TypesOfTimeAnalysisHandler();
            return analysisType switch
            {
                "EXPANDGAINS" => await handler.AnalyzeExpAndGainsForNdays(botClient, userId, daysAmount),
                "WALLETSSTATE" => await handler.AnalyzeWalletsStatesForNdays(botClient, userId, daysAmount),
                "BALANCEMULTIPLICATIONS" => await handler.ViewBalanceMultiplicationsForNdays(botClient, userId, daysAmount),
                "WALLETSHISTORYALL" => await handler.ViewAllWalletsHistoryForNdays(botClient, userId, daysAmount),
                "WALLETSHISTORYONE" => await handler.ViewSomeWalletHistoryForNDays(botClient, userId, daysAmount),
                _ => GetMessages(await botClient.SendTextMessageAsync(userId, "⚠️ Error №5! Please report to @Yan-stan")),
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
