using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    class RenameSomeSubcategoryTextCatcher
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string entityTypeUpper = u.UserStatusArray[3];

            var reposFactory = new FinanceEntityRepositoryFactory();
            var reposOfEntity = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            return await Validate(u, botClient, reposOfEntity);
        }

        async private Task<Messages> Validate(UserData u, TelegramBotClient botClient, FinanceEntityRepository reposOfEntity)
        {
            Message msg;
            string newSubcategoryName = u.UserText;
            if (reposOfEntity.IsSubcategoryExists(newSubcategoryName))
                msg = await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Вы пытаетесь изменить название категории на \"{newSubcategoryName}\", но такая категория уже существует!");
            else if (u.UserText.Contains('/'))
                msg = await botClient.SendTextMessageAsync(u.UserId, "⚠️ К сожалению, символ \"/\" запрещен для ввода. Все остальные символы разрешены");
            else
                msg = await ExecuteAfterValidation(u, botClient, reposOfEntity, newSubcategoryName);
            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            return messages;
        }

        async private Task<Message> ExecuteAfterValidation(UserData u, TelegramBotClient botClient,
            FinanceEntityRepository reposOfEntity, string newSubcategoryName)
        {
            string categoryName = u.UserStatusArray[5];
            string oldSubcategoryName = u.UserStatusArray[7];
            reposOfEntity.RenameSubcategory(categoryName, oldSubcategoryName, newSubcategoryName);
            var msg = await botClient.SendTextMessageAsync(u.UserId, $"✅⚙️ Изменения в категории {categoryName}:\n" +
                $"Субкатегория \"{oldSubcategoryName}\" успешно переименована в \"{newSubcategoryName}\"!\n");
            UserRepository reposOfuser = new UserRepository();
            reposOfuser.SetUserChatStatus(u.UserId, "SUBCATEGORY/RENAMING FINISHED!");
            return msg;
        }
    }
}
