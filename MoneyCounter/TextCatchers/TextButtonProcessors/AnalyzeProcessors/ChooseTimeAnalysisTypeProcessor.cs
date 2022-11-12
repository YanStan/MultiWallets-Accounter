using MoneyCounter.Analyzers;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoneyCounter.TextButtonProcessors
{
    public class ChooseTimeAnalysisTypeProcessor
    {
        delegate Task<Messages> ResponseExecutor(TelegramBotClient botClient, int userId, int daysAmount);

        public async Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string analysisTypeUpper = u.UserStatusArray[4];
            TypesOfTimeAnalysisHandler handler = new TypesOfTimeAnalysisHandler();
            ResponseExecutor executeForNDays = handler.AnalyzeExpAndGainsForNdays;
            ResponseExecutor processWalletsStatesNDays = handler.AnalyzeWalletsStatesForNdays;
            ResponseExecutor viewBalanceMultiplications = handler.ViewBalanceMultiplicationsForNdays;
            ResponseExecutor viewWalletsHistory = handler.ViewAllWalletsHistoryForNdays;
            ResponseExecutor viewWalletHistory = handler.ViewSomeWalletHistoryForNDays;

            return (analysisTypeUpper) switch
            {
                "EXPANDGAINS" => await ExecuteAnalysis(botClient, u, executeForNDays),
                "WALLETSSTATE" => await ExecuteAnalysis(botClient, u, processWalletsStatesNDays),
                "BALANCEMULTIPLICATIONS" => await ExecuteAnalysis(botClient, u, viewBalanceMultiplications),
                "WALLETSHISTORYALL" => await ExecuteAnalysis(botClient, u, viewWalletsHistory),
                "WALLETSHISTORYONE" => await ExecuteAnalysis(botClient, u, viewWalletHistory),

                _ => GetMessages(await botClient.SendTextMessageAsync(u.UserId, "3⚠️ Error! Please write to @Yan_stan")),
            };
        }
        async private Task<Messages> ExecuteAnalysis(TelegramBotClient botClient, UserData u, 
            ResponseExecutor myDelegate)
        {
            UserRepository userRepository = new UserRepository();
            return u.UserText switch
            {
                "За последний день" => await myDelegate(botClient, u.UserId, 1),
                "За последние 2 дня" => await myDelegate(botClient, u.UserId, 2),
                "За последний месяц" => await myDelegate(botClient, u.UserId, 31),
                "За всё время" => await myDelegate(botClient, u.UserId, 36500),
                "⌨️ За последние N дней" => await RequestForNDays(u.UserId, botClient, userRepository, u.UserStatusArray),
                "От времени до времени 📐" => await RequestFor2Datetimes(u.UserId, botClient, userRepository, u.UserStatusArray),
                "Удалить трансакцию по номеру" => await RequestForTransNumberForDeletion(u.UserId, botClient, userRepository),
                "Изменить сумму перевода по номеру" => GetMessages(await botClient.SendTextMessageAsync(u.UserId, "Not implemented yet!")),
                _ => GetMessages(await botClient.SendTextMessageAsync(u.UserId, "⚠️ Выбери период для аналитики!")),
            };
        }
        private static async Task<Messages> RequestForTransNumberForDeletion(int userId, TelegramBotClient botClient,
            UserRepository userRepository)
        {
            var delRepos = new TransactionsForDeletionRepository();
            delRepos.DeleteAllSpareEntities(userId);
            var msg = await botClient.SendTextMessageAsync(userId, $"Введи номер трансакции числом");
            userRepository.SetUserChatStatus(userId, $"WAIT/INPUTFOR/TRANSACTION/DELETION/TRANSID");
            return GetMessages(msg);
        }
        private static async Task<Messages> RequestFor2Datetimes(int userId, TelegramBotClient botClient, UserRepository userRepository,
            string[] userStatusArray)
        {
            string analysisTypeUpper = userStatusArray[4];
            if (analysisTypeUpper == "WALLETSHISTORYONE")
                analysisTypeUpper += $"/{userStatusArray[6]}";

            var msg = await botClient.SendTextMessageAsync(userId, $"Введи две DateTime по Гринвичу (Utc) через дефис в формате:\n`18/08/2018-20/08/2021`\nили\n`18/08/2018 07:22:16-20/08/2018 09:22:16`", ParseMode.Markdown);
            userRepository.SetUserChatStatus(userId, $"WAIT/INPUTFOR/ANALYSIS/TIMETYPE/TIMETOTIME/{analysisTypeUpper}");
            return GetMessages(msg);
        }
        private static async Task<Messages> RequestForNDays(int userId, TelegramBotClient botClient, UserRepository userRepository,
            string[] userStatusArray)
        {
            string analysisTypeUpper = userStatusArray[4];
            if (analysisTypeUpper == "WALLETSHISTORYONE")
                analysisTypeUpper += $"/{userStatusArray[6]}";

            var msg = await botClient.SendTextMessageAsync(userId, "Введи числом N =");
            userRepository.SetUserChatStatus(userId, $"WAIT/INPUTFOR/ANALYSIS/TIMETYPE/LASTNDAYS/{analysisTypeUpper}");
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}