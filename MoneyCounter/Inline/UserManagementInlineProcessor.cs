using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.Inline
{
    class UserManagementInlineProcessor : InlineProcessor
    {
        public override string Name { get; set; } = "Управление пользователями";

        public async override Task<Messages> Execute(CallbackQuery c, TelegramBotClient botClient)
        {
            int userId = (int)c.Message.Chat.Id;
            UserRepository reposOfUser = new UserRepository();
            if (!reposOfUser.IsUserAdmin(userId))
                return await SendMsg(botClient, userId, "⚠️ Вы уже не администратор!");

            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormUserManagement();
            var msg = await botClient.SendTextMessageAsync(userId,
                $"Выбери тип управления",
                replyMarkup: keyboard);
            reposOfUser.SetUserChatStatus(userId, "WAIT/CHOICEOF/USER/MANAGEMENT/TYPE");
            var msgList = new List<Message> { msg };
            return new Messages(msgList);
        }

        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
