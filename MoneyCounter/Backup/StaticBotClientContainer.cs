using Telegram.Bot;

namespace MoneyCounter.Backup
{
    public static class StaticBotClientContainer
    {
        public static TelegramBotClient BotClient { get; set; }
    }
}
