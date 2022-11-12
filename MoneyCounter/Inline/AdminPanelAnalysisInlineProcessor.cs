using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.Inline
{
    class AdminPanelAnalysisInlineProcessor : InlineProcessor
    {
        public override string Name { get; set; } = "Анализировать основ. данные";

        async public override Task<Messages> Execute(CallbackQuery c, TelegramBotClient botClient)
        {
            int userId = (int)c.Message.Chat.Id;
            UserRepository reposOfUser = new UserRepository();
            if (!reposOfUser.IsUserAdmin(userId))
                return await SendMsg(botClient, userId, "⚠️ Вы уже не администратор!");

            var repos = new FoundersTransactionRepository();
            var transactions = repos.GetAllEntities();
            string msgText = "✅ Список переводов между основателями:\n";
            transactions.ForEach(x => msgText += $"{x.FromWallet}   => {x.ToWallet} ({x.MoneyAmount})\n");
            var walletNames = repos.GetAllUsedFoundersWallets();
            string sumsText = "\nЗа всё время было переведено\n";
            walletNames.ForEach(x => sumsText += $"На кошелек {x}:  {repos.GetLastSumOnWallet(x)} грн.\n");
            MassiveMessagesAnswerer messagesAnswerer = new MassiveMessagesAnswerer();
            return await messagesAnswerer.MassiveTextMessageAnswer(botClient, msgText + sumsText, userId);
        }
        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
