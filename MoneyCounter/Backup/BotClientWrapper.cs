using System;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;

namespace MoneyCounter.Backup
{
    public class BotClientWrapper
    {
        async public Task SendDocumentAsync(int userId, InputOnlineFile file, string text)
        {
            var botClient = StaticBotClientContainer.BotClient;
            if (botClient != null)
                await botClient.SendDocumentAsync(userId, file, text);
            else
                Console.WriteLine("BotClientWrapper: botClient ==  null!!");
        }
    }
}
