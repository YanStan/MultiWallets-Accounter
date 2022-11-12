using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    class AddSomeSubcategoryTextCatcher
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string entityTypeUpper = u.UserStatusArray[2];
            var repos = GetReposFromEntityTypeUpper(entityTypeUpper);
            return await Validate(u, botClient, repos);
        }

        async private Task<Messages> Validate(UserData u, TelegramBotClient botClient, FinanceEntityRepository repos)
        {
            Message msg;
            Messages messages;
            string subcategoryName = u.UserText;
            if (subcategoryName.Contains('/'))
            {
                msg = await botClient.SendTextMessageAsync(u.UserId, "⚠️ К сожалению, символ \"/\" запрещен" +
                    " для ввода. Все остальные символы разрешены");
                return GetMessages(msg);
            }
            else if (repos.IsSubcategoryExists(subcategoryName))
            {
                msg = await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Вы пытаетесь добавить новую субкатегорию" +
                    $" \"{subcategoryName}\", но такая субкатегория уже существует!");
                return GetMessages(msg);
            }
            else
            {
                messages = await ExecuteAfterValidation(u, botClient, repos);
            }
            return messages;
        }

        async private Task<Messages> ExecuteAfterValidation(UserData u, TelegramBotClient botClient,
            FinanceEntityRepository repos)
        {
            Messages messages;
            string categoryName = u.UserStatusArray[4];
            if (u.UserStatus.StartsWith("WAIT/ADDENTITY/"))
                messages = await ExecuteIfFinishingCategoryByAddingSubcategory(u, botClient, repos, categoryName);
            else if (u.UserStatus.StartsWith("WAIT/ADDSUBCATEGORY/"))
                messages = await ExecuteIfJustAddingSubcategory(u, botClient, repos, categoryName);
            else
                messages = null; //TODO EXCEPTION
            return messages;
        }

        async private Task<Messages> ExecuteIfFinishingCategoryByAddingSubcategory(UserData u, TelegramBotClient botClient,
            FinanceEntityRepository repos, string categoryName)
        {       
            string subcategoryName = u.UserText;
            AddCategoryWithSubcategory(repos, u.UserId, categoryName, subcategoryName);
            var msg = await botClient.SendTextMessageAsync(u.UserId, $"✅⚙️ Была успешно добавлена субкатегория:\n{subcategoryName}\nв категорию:\n{categoryName}!");
            return GetMessages(msg);
        }

        async private Task<Messages> ExecuteIfJustAddingSubcategory(UserData u, TelegramBotClient botClient,
            FinanceEntityRepository repos, string categoryName)
        {
            string subcategoryName = u.UserText;
            repos.AddSubcategory(categoryName, subcategoryName);
            var msg = await botClient.SendTextMessageAsync(u.UserId, $"✅⚙️ Была успешно добавлена субкатегория:\n{subcategoryName}\nв категорию:\n{categoryName}!\n" +
                $"Ты можешь добавить еще одну субкатегорию, введя ее название.");
            return GetMessages(msg);
        }

        private FinanceEntityRepository GetReposFromEntityTypeUpper(string entityTypeUpper)
        {
            var reposFactory = new FinanceEntityRepositoryFactory();
            var repos = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            return repos;
        }

        private void AddCategoryWithSubcategory(FinanceEntityRepository repos, int userId,
            string categoryName, string subcategoryName)
        {
            repos.AddCategoryWithSubcategory(categoryName, subcategoryName);
            UserRepository userRepos = new UserRepository();
            userRepos.SetUserChatStatus(userId, $"CATEGORY/ADDING/FINISHED");
        }

        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
