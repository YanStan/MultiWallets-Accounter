using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors.CustomizationProcessors
{
    class ChooseForRenamingSomeSubcategoryTextProcessor
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            Message msg;        
            string entityTypeUpper = u.UserStatusArray[3];       
            string oldSubcategoryName = u.UserText;

            FinanceEntityRepositoryFactory reposFactory = new FinanceEntityRepositoryFactory();
            var repos = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            msg = repos.IsSubcategoryExists(oldSubcategoryName) ?
                await ExecuteAfterValidation(u, botClient, entityTypeUpper)
                :
                await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Вы хотите переименовать субкатегорию \"{oldSubcategoryName}\", но она уже удалена или переименована.");
            return GetMessages(msg);
        }
        private static async Task<Message> ExecuteAfterValidation(UserData u, TelegramBotClient botClient, string entityTypeUpper)
        {
            string categoryName = u.UserStatusArray[5];
            string oldSubcategoryName = u.UserText;
            UserRepository reposOfUser = new UserRepository();
            reposOfUser.SetUserChatStatus(u.UserId, $"WAIT/RENAME/SUBCATEGORY/{entityTypeUpper}/CATEG/{categoryName}/SUBCATEG/{oldSubcategoryName}");
            return await botClient.SendTextMessageAsync(u.UserId, $"Введи новое название для субкатегории \"{oldSubcategoryName}\"!");
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
