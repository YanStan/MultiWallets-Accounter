using MoneyCounter.Analyzers;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.TextCatchers.OrdinaryInputedTextCatchers
{
    public class CustomWalletNameForHisHistoryTextCatcher
    {
        public async Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            TransactionRepository repos = new TransactionRepository();
            string walletName = u.UserText;
            if (repos.WasThisWalletEverUsed(walletName))
                return await ExecuteAfterValidation(botClient, u.UserId, walletName);
            else
                return GetMessages(await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Кошелек {walletName} ни разу не был использован!"));
        }

        private static async Task<Messages> ExecuteAfterValidation(TelegramBotClient botClient, int userId, string walletName)
        {
            UserRepository userRepository = new UserRepository();
            userRepository.SetUserChatStatus(userId, $"WAIT/CHOOSING/ANALYSIS/TIMETYPE/WALLETSHISTORYONE/LASTNDAYS/{walletName}");
            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormDaysForAnalysisKeyboard();
            return GetMessages(await botClient.SendTextMessageAsync(userId, "Выбери период для аналитики", replyMarkup: keyboard));
        }

        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });

    }
}
