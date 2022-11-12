using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors.CustomizationProcessors
{
    class RenameSomeCategoryTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string entityTypeUpper = u.UserStatusArray[4];
            FinanceEntityRepositoryFactory reposFactory = new FinanceEntityRepositoryFactory();
            var repos = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            string[] categoriesNames = repos.GetEntityCategoriesNames();
            var answer = await SendMsgs(botClient, u.UserId, categoriesNames);
            var reposOfUser = new UserRepository();
            reposOfUser.SetUserChatStatus(u.UserId, $"WAIT/RENAME/ENTITY/{entityTypeUpper}");
            return answer;
        }
        private static async Task<Messages> SendMsgs(TelegramBotClient botClient, int userId, string[] categoriesNames)
        {
            var keyboardFormer = new KeyboardFormer();
            var keyboard = keyboardFormer.FormCategoriesTextKeyboardForChanging(categoriesNames);
            var msg = await botClient.SendTextMessageAsync(userId, "Выбери категорию для переименования", replyMarkup: keyboard);
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
