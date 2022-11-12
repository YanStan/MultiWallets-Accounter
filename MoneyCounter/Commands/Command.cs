using MoneyCounter.Wrappers;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace MoneyCounter.Commands
{
    abstract class Command
    {
        public abstract string Name { get; set; } //этот класс должен содержать имя, т.к. в методе Фабрики мы обращаемся к "command.Name."

        public abstract Task<Messages> Execute(MessageEventArgs e, TelegramBotClient botClient); // а это нужно, т.к в методе ниже обращаемся к cmd_instance.Execute(e);

    }
}
