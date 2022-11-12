using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.Commands
{
    abstract class AdminCommand : Command
    {
        async public override Task<Messages> Execute(MessageEventArgs e, TelegramBotClient botClient)
        {
            var repos = new UserRepository();
            if (repos.IsUserAdmin(e.Message.From.Id) == false)
            {
                var msg = await botClient.SendTextMessageAsync(e.Message.Chat.Id, text: "Вы не имеете права на эту команду!");
                var messagesList = new List<Message>() { msg };
                var messages = new Messages(messagesList);
                return messages;
            }
            else
            {
                return await ExecuteAfterValidation(e, botClient);
            }
        }
        abstract public Task<Messages> ExecuteAfterValidation(MessageEventArgs e, TelegramBotClient botClient);
    }
}
