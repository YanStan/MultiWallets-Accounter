using MoneyCounter.TextButtonProcessors;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.TextCatchers.TextButtonProcessors.AdminProcessors
{
    class UserManagementTypeTextCatcher : TextProcessor
    {
        public async override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            UserRepository reposOfUser = new UserRepository();
            KeyboardFormer former = new KeyboardFormer();
            if(reposOfUser.IsUserAdmin(u.UserId))
                return await ExecuteForAdmin(u, botClient, reposOfUser, former);
            else
                return await SendMsg(botClient, u.UserId, "⚠️ Вы уже не администратор!");
        }

        private static async Task<Messages> ExecuteForAdmin(UserData u, TelegramBotClient botClient, UserRepository reposOfUser, KeyboardFormer former)
        {
            return u.UserText switch
            {
                "✴️ Добавить администратора" => await ExecuteAdminAddition(u, botClient, reposOfUser, former),
                "📴 Удалить администратора" => await ExecuteAdminDeletion(u, botClient, reposOfUser, former),
                "💟 Добавить пользователя" => await ExecuteUserAddition(u, botClient, reposOfUser),
                "♒️ Удалить пользователя" => await ExecuteUserDeletion(u, botClient, reposOfUser, former),
                _ => await ExecuteMistypeAnswer(u, botClient),
            };
        }

        private static async Task<Messages> ExecuteAdminAddition(UserData u, TelegramBotClient botClient, UserRepository reposOfUser,
            KeyboardFormer former)
        {
            var keyboard = former.FormUsernames(false);
            reposOfUser.SetUserChatStatus(u.UserId, "WAIT/INPUTFOR/USER/MANAGEMENT/ADMIN/ADDITION");
            return await SendMsg(botClient, u.UserId, "Введи или выбери @username будущего администратора", keyboard);
        }

        private static async Task<Messages> ExecuteAdminDeletion(UserData u, TelegramBotClient botClient, UserRepository reposOfUser,
            KeyboardFormer former)
        {
            if (u.UserId != 359043468 && u.UserId != 430611757)// vladkheylo customer founder, Yan_stan executor-programmer
                return await SendMsg(botClient, u.UserId, "⚠️ Вы не можете выполнить эту операцию." +
                    " Обратитесь к администратору-заказчику программы.");

            var keyboard = former.FormUsernames(true);
            reposOfUser.SetUserChatStatus(u.UserId, "WAIT/INPUTFOR/USER/MANAGEMENT/ADMIN/DELETION");
            return await SendMsg(botClient, u.UserId, "Введи или выбери @username администратора для удаления", keyboard);
        }

        private static async Task<Messages> ExecuteUserAddition(UserData u, TelegramBotClient botClient, UserRepository reposOfUser)
        {
            reposOfUser.SetUserChatStatus(u.UserId, "WAIT/INPUTFOR/USER/MANAGEMENT/USER/ADDITION");
            return await SendMsg(botClient, u.UserId, "Введи  @username нового пользователя");
        }

        private static async Task<Messages> ExecuteUserDeletion(UserData u, TelegramBotClient botClient, UserRepository reposOfUser,
            KeyboardFormer former)
        {
            var keyboard = former.FormUsernamesFromWhiteList();
            reposOfUser.SetUserChatStatus(u.UserId, "WAIT/INPUTFOR/USER/MANAGEMENT/USER/DELETION");
            return await SendMsg(botClient, u.UserId, "Введи или выбери @username пользователя для удаления", keyboard);
        }

        private static async Task<Messages> ExecuteMistypeAnswer(UserData u, TelegramBotClient botClient)
        {
            return await SendMsg(botClient, u.UserId, "⚠️ Неверный ввод! Выберите один из вариантов" +
                " управления пользователями.");
        }
        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
