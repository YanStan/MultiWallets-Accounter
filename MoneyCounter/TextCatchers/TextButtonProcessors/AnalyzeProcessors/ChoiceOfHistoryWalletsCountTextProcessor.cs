using MoneyCounter.Analyzers;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.TextCatchers.TextButtonProcessors.AnalyzeProcessors
{
    class ChoiceOfHistoryWalletsCountTextProcessor
    {
        public async Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            UserRepository userRepository = new UserRepository();
            KeyboardFormer former = new KeyboardFormer();
            switch (u.UserText)
            {
                case "🔗Один кошелек":
                    userRepository.SetUserChatStatus(u.UserId, "WAIT/INPUTFOR/ANALYSIS/TIMETYPE/WALLETSHISTORYONE");
                    var keyboard1 = former.FormUsedWalletsKeyboard("TRANSACTION");
                    return GetMessages(await botClient.SendTextMessageAsync(u.UserId, "Выбери название кошелька", replyMarkup: keyboard1));

                case "Все кошельки 🖇":
                    var keyboard2 = former.FormDaysForAnalysisKeyboard();
                    var msg = await botClient.SendTextMessageAsync(u.UserId, "Выбери период для аналитики", replyMarkup: keyboard2);
                    userRepository.SetUserChatStatus(u.UserId, "WAIT/CHOOSING/ANALYSIS/TIMETYPE/WALLETSHISTORYALL");
                    return GetMessages(msg);
                default:
                    return GetMessages(await botClient.SendTextMessageAsync(u.UserId, "⚠️ Выбери количество кошельков!"));
            }
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
