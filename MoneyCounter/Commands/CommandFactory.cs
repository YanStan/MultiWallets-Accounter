using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.Commands
{
    class CommandFactory
    {
        private readonly Command[] commandsArray; 
        public CommandFactory() // конструктор
        {   
            commandsArray = new Command[]
            {
                new StartCommand(),// конструктор экземпляра класса StartCommand. 
                new BackupCommand()
            };
        }
        protected Command GetCommandInstanceFromStringCommandName(string msgText) =>
            commandsArray.FirstOrDefault(x => msgText.StartsWith(x.Name));

        public async Task<Messages> RunCommandExecution(MessageEventArgs e, TelegramBotClient botClient)
        {
            Command cmdInstance = GetCommandInstanceFromStringCommandName(e.Message.Text); //вытягиваем инстанс по названию команды
            if (cmdInstance != null)
            {
                return await cmdInstance.Execute(e, botClient); //выполняем тело команды}
            }
            else
            {
                var msg = await botClient.SendTextMessageAsync(e.Message.Chat.Id, "⚠️ Неизвестная команда");
                var messagesList = new List<Message>() { msg };
                return new Messages(messagesList);
            }
        }
    }
}