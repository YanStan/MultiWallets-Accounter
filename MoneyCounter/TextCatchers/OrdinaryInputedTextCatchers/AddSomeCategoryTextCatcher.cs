using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{ 
    class AddSomeCategoryTextCatcher
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string startOfStatusText = "WAIT/ADDENTITY/";
            string entityTypeUpper = u.UserStatus[startOfStatusText.Length..];
            var reposFactory = new FinanceEntityRepositoryFactory();
            var repos = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            return await SendMsg(u, botClient, repos, entityTypeUpper);
        }

        async private Task<Messages> SendMsg(UserData u, TelegramBotClient botClient,
           FinanceEntityRepository repos, string entityTypeUpper)
        {
            Message msg;
            if (repos.IsCategoryExists(u.UserText))
                msg = await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Вы пытаетесь добавить новую категорию \"{u.UserText}\", но такая категория уже существует!");
            else if (u.UserText.Contains('/'))
                msg = await botClient.SendTextMessageAsync(u.UserId, "⚠️ К сожалению, символ \"/\" запрещен для ввода. Все остальные символы разрешены");
            else
                msg = await SendValidatedMsg(u, botClient, entityTypeUpper);
            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            return messages;
        }

        async private Task<Message> SendValidatedMsg(UserData u, TelegramBotClient botClient, string entityTypeUpper)
        {
            UserRepository userRepos = new UserRepository();
            userRepos.SetUserChatStatus(u.UserId, $"WAIT/ADDENTITY/{entityTypeUpper}/CATEG/{u.UserText}");
            return await botClient.SendTextMessageAsync(u.UserId, $"⚙️ Будет добавлена новая категория: {u.UserText}.\n" +
                $"Теперь введи название её подкатегории!\n\n" +
                $"В каждой категории должна быть хотя бы одна подкатегория, названная тобой. В другом случае, категория не может быть использована.");
        }
    }
}
