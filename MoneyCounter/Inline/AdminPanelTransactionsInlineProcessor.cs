using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.Inline
{
    class AdminPanelTransactionsInlineProcessor : InlineProcessor
    {
        public override string Name { get; set; } = "Внести основ. перевод";

        async public override Task<Messages> Execute(CallbackQuery c, TelegramBotClient botClient)
        {
            int userId = (int)c.Message.Chat.Id;
            UserRepository reposOfUser = new UserRepository();
            if (!reposOfUser.IsUserAdmin(userId))
                return await SendMsg(botClient, userId, "⚠️ Вы уже не администратор!");

            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormUsedWalletsKeyboard("FOUNDERSTRANSACTION");
            var msg = await botClient.SendTextMessageAsync(c.Message.Chat.Id, $"Введи или выбери кошелек-источник", replyMarkup: keyboard);

            reposOfUser.SetUserChatStatus((int)c.Message.Chat.Id, "WAIT/ADDWALLETSTOENTITY/FOUNDERSTRANSACTION/CATEGORY/🎩 Founders/SUBCATEGORY/🎩 Founders");
            var msgList = new List<Message> { msg };
            return new Messages(msgList);
        }

        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
