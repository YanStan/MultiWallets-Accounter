using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors
{
    class DeleteSomeCategoryTextProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var entityTypeUpper = "TRANSACTION";
            var reposFactory = new FinanceEntityRepositoryFactory();
            var repos = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);
            var categoriesNames = repos.GetEntityCategoriesNames();

            var keyboardFormer = new KeyboardFormer();
            var keyboard = keyboardFormer.FormCategoriesTextKeyboardForChanging(categoriesNames);
            var msg = await botClient.SendTextMessageAsync(u.UserId, "Выбери одну или несколько категорий для удаления", replyMarkup: keyboard);           
            UserRepository reposOfUser = new UserRepository();
            reposOfUser.SetUserChatStatus(u.UserId, $"WAIT/DELETE/ENTITY/{entityTypeUpper}");
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
