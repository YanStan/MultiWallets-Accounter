using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors //AnalyzeExpensesAndGainsTextProcessor
{
    class ChoiceOfAnalysisTypeTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var userRepository = new UserRepository();
            if (u.UserText == "🎞 История переводов")
                return await ExecuteOfHistorical(botClient, u.UserId, userRepository);
            else
                return await ExecuteIfNonHistorical(u, botClient);
        }

        private static async Task<Messages> ExecuteOfHistorical(TelegramBotClient botClient, int userId, UserRepository userRepository)
        {
            userRepository.SetUserChatStatus(userId, "WAIT/CHOOSING/HISTORY/WALLETSCOUNT/FOR/ANALYSIS");
            var former = new KeyboardFormer();
            var keyboard = former.FormForWalletHistoryType();
            return GetMessages(await botClient.SendTextMessageAsync(userId, "Выбери тип истории переводов", replyMarkup: keyboard));
        }

        private async Task<Messages> ExecuteIfNonHistorical(UserData u, TelegramBotClient botClient)
        {
            if(!(u.UserText == "💲 Доходы и расходы" 
                || u.UserText == "💼 Состояние кошельков" 
                || u.UserText == "📈 Доходные счета"))
                return GetMessages(await botClient.SendTextMessageAsync(u.UserId, "⚠️ Выбери тип анализа!"));

            UserRepository userRepository = new UserRepository();
            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormDaysForAnalysisKeyboard();
            var msg = await botClient.SendTextMessageAsync(u.UserId, "Выбери период для аналитики", replyMarkup: keyboard);
            SetUserStatus(u, userRepository);
            return GetMessages(msg);
        }

        private static void SetUserStatus(UserData u, UserRepository userRepository)
        {
            switch (u.UserText)
            {
                case "💲 Доходы и расходы":
                    userRepository.SetUserChatStatus(u.UserId, "WAIT/CHOOSING/ANALYSIS/TIMETYPE/EXPANDGAINS");
                    break;
                case "💼 Состояние кошельков":
                    userRepository.SetUserChatStatus(u.UserId, "WAIT/CHOOSING/ANALYSIS/TIMETYPE/WALLETSSTATE");
                    break;
                case "📈 Доходные счета":
                    userRepository.SetUserChatStatus(u.UserId, "WAIT/CHOOSING/ANALYSIS/TIMETYPE/BALANCEMULTIPLICATIONS");
                    break;
            }
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
