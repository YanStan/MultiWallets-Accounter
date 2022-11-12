using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors.CustomizationProcessors
{
    class AddSomeSubcategoryTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var userRepos = new UserRepository();
            string userChatStatus = userRepos.GetUserChatStatus(u.UserId);
            var userStatusArray = userChatStatus.Split("/");
            string entityTypeUpper = userStatusArray[4];
            string categoryName = userStatusArray[6];
            userRepos.SetUserChatStatus(u.UserId, $"WAIT/ADDSUBCATEGORY/{entityTypeUpper}/CATEG/{categoryName}");
            var msg = await botClient.SendTextMessageAsync(u.UserId, "Введи имя новой субкатегории!🙂\nМожешь использовать смайлики телеграм. Также можно добавить несколько субкатегорий через Enter.");
            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            return messages;
        }
    }
}
