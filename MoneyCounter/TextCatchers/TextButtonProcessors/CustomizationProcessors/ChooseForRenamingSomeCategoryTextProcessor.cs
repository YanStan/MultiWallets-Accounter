using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors
{
    public class ChooseForRenamingSomeCategoryTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var oldCategoryName = u.UserText;
            var userRepos = new UserRepository();
            TransactionRepository repos = new TransactionRepository();
            string userStatus = userRepos.GetUserChatStatus(u.UserId);
            string entityTypeUpper = userStatus.Split("/")[3];
            Message msg;
            if (repos.IsCategoryExists(oldCategoryName))
            {
                userRepos.SetUserChatStatus(u.UserId, $"WAIT/RENAME/ENTITY/{entityTypeUpper}/CATEG/{oldCategoryName}");
                msg = await botClient.SendTextMessageAsync(u.UserId, $"Введи новое название для категории \"{oldCategoryName}\".\nМожно использовать смайлики.");
            }
            else
            {
                msg = await botClient.SendTextMessageAsync(u.UserId, $"⚠️Вы пытаетесь переименовать категорию \"{oldCategoryName}\", но она уже удалена или переименована.");
            }
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
