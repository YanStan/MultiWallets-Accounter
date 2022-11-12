using MoneyCounter.Wrappers;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.Inline
{
    public abstract class InlineProcessor
    {
        public abstract string Name { get; set; } //этот класс должен содержать имя, т.к. в методе Фабрики мы обращаемся к "command.Name."
        public abstract Task<Messages> Execute(CallbackQuery c, TelegramBotClient botClient); // а это нужно, т.к в след. методе Фабрики ниже обращаемся к cmd_instance.Execute(e);
    }
}
