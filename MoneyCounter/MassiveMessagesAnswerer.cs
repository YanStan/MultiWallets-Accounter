using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter
{
    public class MassiveMessagesAnswerer
    {
        async public Task<Messages> MassiveTextMessageAnswer(TelegramBotClient botClient, string text, int userId,
            ReplyKeyboardMarkup keyboard = null)
        {
            return (text.Length <= 4096) ?
                GetMessages(await botClient.SendTextMessageAsync(userId, text, ParseMode.Html, replyMarkup: keyboard))
                :
                await MassiveMessageAnswerChecked(botClient, text, userId);
        }

        async public Task<Messages> MassiveMultipleMessagesAnswer(TelegramBotClient botClient, int userId, List<string> msgtexts)
        {
            var messagesList = new List<Message> { };
            int truthCounter = 0;
            msgtexts.ForEach(x => { if (x.Length <= 4096) { truthCounter += 1; } });
            if(truthCounter == msgtexts.Count)
                return await FormTextsInMessages(botClient, userId, msgtexts, messagesList);
            else
                return await MassiveMessageAnswerChecked(botClient, string.Join("\n", msgtexts), userId);
        }

        private static async Task<Messages> FormTextsInMessages(TelegramBotClient botClient, int userId, List<string> msgtexts, List<Message> messagesList)
        {
            foreach (string text in msgtexts)
                messagesList.Add(await botClient.SendTextMessageAsync(userId, text, parseMode: ParseMode.Html));
            return new Messages(messagesList);
        }


        async public Task<Messages> MassiveMessageAnswerChecked(TelegramBotClient botClient, string text, int userId)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            using MemoryStream stream = new MemoryStream(bytes);
            var inputOnlineFile = new InputOnlineFile(stream);
            var msg = await botClient.SendDocumentAsync(userId, inputOnlineFile, "👾 Так как текст довольно большой, я решил записать его в файл.\nУдачного чтения!");
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });

    }
}
