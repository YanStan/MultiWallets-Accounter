using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors
{
    class DeleteSomeSubCategoryTextProcessor : TextProcessor
    {

        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string entityTypeUpper = u.UserStatusArray[4];
            string categoryName = u.UserStatusArray[6];

            var reposFactory = new FinanceEntityRepositoryFactory();
            var repos = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            var subcategoriesNames = repos.GetEntitySubcategoriesOfCategory(categoryName).ToList();
            var answer = await SendMsgs(u, botClient, subcategoriesNames);
            UserRepository reposOfUser = new UserRepository();
            reposOfUser.SetUserChatStatus(u.UserId, $"WAIT/DELETE/SUBCATEGORY/{entityTypeUpper}/CATEG/{categoryName}");
            return answer;
        }
        private static async Task<Messages> SendMsgs(UserData u, TelegramBotClient botClient, List<string> subcategoriesNames)
        {
            KeyboardFormer keyboardFormer = new KeyboardFormer();
            var keyboard = keyboardFormer.FormSubCategoriesFromListForChanging(subcategoriesNames);
            var msg = await botClient.SendTextMessageAsync(u.UserId,
                "Выбери одну или несколько субкатегорий для удаления", replyMarkup: keyboard);
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
