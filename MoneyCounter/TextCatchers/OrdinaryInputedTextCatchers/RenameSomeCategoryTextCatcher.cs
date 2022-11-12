using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    class RenameSomeCategoryTextCatcher
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var userRepos = new UserRepository();
            var userStatusArray = userRepos.GetUserChatStatus(u.UserId).Split("/");
            string oldCategoryName = userStatusArray[5];
            var reposFactory = new FinanceEntityRepositoryFactory();
            var reposOfEntity = reposFactory.GetRepositoryInstanceFromItsUpperName(userStatusArray[3]);
            return await Validate(u, botClient, reposOfEntity, oldCategoryName);
        }

        async private Task<Messages> Validate(UserData u, TelegramBotClient botClient, FinanceEntityRepository reposOfEntity,
            string oldCategoryName)
        {
            Message msg;
            if (reposOfEntity.IsCategoryExists(u.UserText))
                msg = await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Вы пытаетесь изменить название категории на \"{u.UserText}\", но такая категория уже существует!");
            else if (u.UserText.Contains('/'))
                msg = await botClient.SendTextMessageAsync(u.UserId, "⚠️ К сожалению, символ \"/\" запрещен для ввода. Все остальные символы разрешены");
            else
                msg = await ExecuteAfterValidation(u, botClient, reposOfEntity, oldCategoryName);
            return GetMessages(msg);
        }

        async private Task<Message> ExecuteAfterValidation(UserData u, TelegramBotClient botClient, FinanceEntityRepository reposOfEntity,
            string oldCategoryName)
        {
            string newCategoryName = u.UserText;
            reposOfEntity.RenameCategory(oldCategoryName, newCategoryName);
            var msg = await botClient.SendTextMessageAsync(u.UserId, $"✅⚙️ Категория \"{oldCategoryName}\" успешно переименована в \"{newCategoryName}\".");
            UserRepository userRepos = new UserRepository();
            userRepos.SetUserChatStatus(u.UserId, "CATEGORY/RENAMING/FINISHED!");
            return msg;
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
