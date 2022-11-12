using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.Inline
{
    class ChooseTransactionEntityTypeInlineProcessor : InlineProcessor
    {
        public override string Name { get; set; } = "Внести перевод";

        public override async Task<Messages> Execute(CallbackQuery c, TelegramBotClient botClient)
        {
            int userId = (int)c.Message.Chat.Id;
            string entityTypeUpper = "TRANSACTION";
            var messages = await SendMsg(botClient, entityTypeUpper, userId);
            var userRepository = new UserRepository();
            userRepository.SetUserChatStatus(userId, "WAIT/CHOOSING/OR/CHANGE/TRANSACTION/CATEGORY");
            return messages;
        }
        async private Task<Messages> SendMsg(TelegramBotClient botClient, string entityTypeUpper, int userId)
        {
            var x = new KeyboardFormer();
            var categories = x.FormCategoriesTextKeyboard(entityTypeUpper);
            var msg = await botClient.SendTextMessageAsync(
                chatId: userId,
                text: $"Пожалуйста, выбери одну из категорий или добавь новую",
                replyMarkup: categories);
            var messagesList = new List<Message>() { msg };
            return new Messages(messagesList);
        }
    }
}
