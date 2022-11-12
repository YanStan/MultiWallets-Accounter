using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors.CustomizationProcessors
{
    class RenameSomeSubcategoryTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string entityTypeUpper = u.UserStatusArray[4];
            string categoryName = u.UserStatusArray[6];
            var reposFactory = new FinanceEntityRepositoryFactory();
            var repos = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            var subcategoriesNames = repos.GetEntitySubcategoriesOfCategory(categoryName).ToList();
            var answer = await SendMsgs(botClient, u.UserId, subcategoriesNames);
            UserRepository reposOfUser = new UserRepository();
            reposOfUser.SetUserChatStatus(u.UserId, $"WAIT/RENAME/SUBCATEGORY/{entityTypeUpper}/CATEG/{categoryName}");
            return answer;
        }
        private static async Task<Messages> SendMsgs(TelegramBotClient botClient, int userId, List<string> subcategoriesNames)
        {
            var keyboardFormer = new KeyboardFormer();
            var keyboard = keyboardFormer.FormSubCategoriesFromListForChanging(subcategoriesNames);
            var msg = await botClient.SendTextMessageAsync(userId, "Выбери субкатегорию для переименования", replyMarkup: keyboard);
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });

    }
}
