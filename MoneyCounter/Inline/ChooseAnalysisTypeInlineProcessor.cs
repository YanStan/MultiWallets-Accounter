using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.Inline
{
    class ChooseAnalysisTypeInlineProcessor : InlineProcessor
    {
        public override string Name { get; set; } = "Анализировать данные";

        async public override Task<Messages> Execute(CallbackQuery c, TelegramBotClient botClient)
        {
            var keyboardFormer = new KeyboardFormer();
            var keyboard = keyboardFormer.FormAnalyseDataKeyboard();
            var msg = await botClient.SendTextMessageAsync(c.Message.Chat.Id,"Выбери тип анализа", replyMarkup: keyboard);
            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            var userRepository = new UserRepository();
            userRepository.SetUserChatStatus((int)c.Message.Chat.Id, "WAIT/CHOOSING/ANALYSIS/TYPE");
            return messages;
        }
    }
}
