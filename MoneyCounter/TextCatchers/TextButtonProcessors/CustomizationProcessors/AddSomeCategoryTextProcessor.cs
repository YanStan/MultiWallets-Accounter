using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors
{
    class AddSomeCategoryTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var userRepos = new UserRepository();
            var entityTypeUpper = "TRANSACTION";
            Message msg;
            userRepos.SetUserChatStatus(u.UserId, $"WAIT/ADDENTITY/{entityTypeUpper}");
            msg = await botClient.SendTextMessageAsync(u.UserId, "Введи имя новой категории!🙂\nМожешь использовать смайлики телеграм.");

            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            return messages;
        }
    }
}
