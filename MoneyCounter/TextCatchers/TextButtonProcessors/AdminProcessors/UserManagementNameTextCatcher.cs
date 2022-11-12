using MoneyCounter.TextButtonProcessors;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.TextCatchers.TextButtonProcessors.AdminProcessors
{
    class UserManagementNameTextCatcher : TextProcessor
    {
        public async override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            UserRepository reposOfuser = new UserRepository();
            if(!u.UserText.StartsWith('@'))
                return await SendMsg(botClient, u.UserId, "⚠️ Неверный формат ввода @username!");

            string state = u.UserStatusArray[4] + "_" + u.UserStatusArray[5];
            return state switch
            {
                "ADMIN_ADDITION" => await ProvideAdminAddition(u, botClient, reposOfuser),
                "ADMIN_DELETION" => await ProvideAdminDeletion(u, botClient, reposOfuser),
                "USER_ADDITION" => await ProvideUserAddition(u, botClient, reposOfuser),
                "USER_DELETION" => await ProvideUserDeletion(u, botClient, reposOfuser),
                _ => await ExecuteMisStatusAnswer(u, botClient),
            };
        }


        private static async Task<Messages> ProvideAdminAddition(UserData u, TelegramBotClient botClient,
            UserRepository reposOfuser)
        {
            if (!reposOfuser.IsUserExistsByName(u.UserText))
                return await SendMsg(botClient, u.UserId, $"⚠️ Пользователь {u.UserText} не является разрешенным оператором " +
                    $"или ни разу не пользовался бухгалтером!");
            //validated
            reposOfuser.UpgradeUserToAdmin(u.UserText);
            reposOfuser.SetUserChatStatus(u.UserId, "ADMIN/ADDING/FINISHED!");
            return await SendMsg(botClient, u.UserId, $"✅✴️ Пользователь {u.UserText} теперь администратор!");

        }
        private static async Task<Messages> ProvideAdminDeletion(UserData u, TelegramBotClient botClient,
            UserRepository reposOfuser)
        {
            if (!reposOfuser.IsUserAdminByName(u.UserText))
                return await SendMsg(botClient, u.UserId, "⚠️ Пользователь не является администратором!");
            //validated
            reposOfuser.DowngradeAdminToUser(u.UserText);
            reposOfuser.SetUserChatStatus(u.UserId, "ADMIN/DELETION/FINISHED!");
            return await SendMsg(botClient, u.UserId, $"✅📴 Пользователь {u.UserText} теперь не администратор!");
        }
        private static async Task<Messages> ProvideUserAddition(UserData u, TelegramBotClient botClient,
            UserRepository reposOfuser)
        {
            if(u.UserText.Length > 33)
                return await SendMsg(botClient, u.UserId, $"⚠️ Пользователь с таким @username не может сущестовать!");
            if (reposOfuser.IsUserInWhiteList(u.UserText))
                return await SendMsg(botClient, u.UserId, $"⚠️ Пользователь {u.UserText} уже является оператором!");
            //validated
            reposOfuser.SetUserNameToWhiteList(u.UserText);
            reposOfuser.SetUserChatStatus(u.UserId, "USER/ADDING/FINISHED!");
            return await SendMsg(botClient, u.UserId, $"✅💟 Пользователь {u.UserText} теперь может пользоваться бухгалтером!");
        }
        private static async Task<Messages> ProvideUserDeletion(UserData u, TelegramBotClient botClient,
            UserRepository reposOfuser)
        {
            if (!reposOfuser.IsUserInWhiteList(u.UserText))
                return await SendMsg(botClient, u.UserId, "⚠️ Такого пользователя не существует!");

            if (reposOfuser.IsUserExistsByName(u.UserText))
                reposOfuser.DeleteUser(u.UserText);
            //validated
            reposOfuser.DeleteUserNameFromWhiteList(u.UserText);
            reposOfuser.SetUserChatStatus(u.UserId, "USER/DELETION/FINISHED!");
            return await SendMsg(botClient, u.UserId, $"✅♒️ Пользователь {u.UserText} теперь не может пользоваться бухгалтером!");
        }


        private static async Task<Messages> ExecuteMisStatusAnswer(UserData u, TelegramBotClient botClient)
        {
            return await SendMsg(botClient, u.UserId, "⚠️ Eror code 10! Write to @Yan_stan");
        }
        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
