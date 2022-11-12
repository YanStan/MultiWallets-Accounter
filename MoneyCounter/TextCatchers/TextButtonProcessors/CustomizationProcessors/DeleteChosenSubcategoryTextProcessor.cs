using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors.CustomizationProcessors
{
    class DeleteChosenSubcategoryTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            Message msg;
            string entityTypeUpper = u.UserStatusArray[3];
            string categoryName = u.UserStatusArray[5];

            var reposFactory = new FinanceEntityRepositoryFactory();
            var repos = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);

            if (!repos.IsSubcategoryExists(u.UserText))
            {
                msg = await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Вы пытаетесь удалить субкатегорию \"{u.UserText}\", но она уже удалена или переименована!");
            }
            else
            {
                repos.DeleteSubcategory(categoryName, u.UserText);
                msg = await botClient.SendTextMessageAsync(u.UserId, $"✅⚙️ Субкатегория \"{u.UserText}\" успешно удалена.");
            }
            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            return messages;
        }
    }
}
