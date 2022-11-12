using MoneyCounter.Wrappers;
using System.Threading.Tasks;
using Telegram.Bot;


namespace MoneyCounter.TextButtonProcessors
{
    public abstract class TextProcessor
    {
        public abstract Task<Messages> Execute(UserData u, TelegramBotClient botClient); // а это нужно, т.к в след. методе Фабрики ниже обращаемся к cmd_instance.Execute(e);
    }
}