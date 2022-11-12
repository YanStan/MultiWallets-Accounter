using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors
{
    public class DeleteChosenCategoryTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            Message msg;
            var reposOfTransaction = new TransactionRepository();
            if(!reposOfTransaction.IsCategoryExists(u.UserText))
            {
                msg = await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Вы пытаетесь удалить категорию \"{u.UserText}\", но она уже удалена или переименована!");
            }
            else
            {
                reposOfTransaction.DeleteCategory(u.UserText);
                msg = await botClient.SendTextMessageAsync(u.UserId, $"✅⚙️ Категория \"{u.UserText}\" успешно удалена.");
            }
            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            return messages;
        }
    }
}
