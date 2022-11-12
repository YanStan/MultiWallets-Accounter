using MoneyCounter.Repositories;
using MoneyCounter.TextButtonProcessors;
using MoneyCounter.TextButtonProcessors.CustomizationProcessors;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoneyCounter.aMainLine
{
    class ChooseSubcategoryOfFinanceEntityProcessor
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string entityTypeUpper = u.UserStatusArray[4];
            string categoryName = u.UserStatusArray[6];
            return await Validate(u, botClient, entityTypeUpper, categoryName);
        }

        private async Task<Messages> Validate(UserData u, TelegramBotClient botClient, string entityTypeUpper, string categoryName)
        {

            switch (u.UserText)
            {
                case "⚙️ Добавить субкатегорию ⚙️":
                    {
                        AddSomeSubcategoryTextProcessor processor = new AddSomeSubcategoryTextProcessor();
                        return await processor.Execute(u, botClient);
                    }
                case "⚙️ Переименовать с.":
                    {
                        RenameSomeSubcategoryTextProcessor processor = new RenameSomeSubcategoryTextProcessor();
                        return await processor.Execute(u, botClient);
                    }
                case "Удалить с. ⚙️":
                    {
                        DeleteSomeSubCategoryTextProcessor processor = new DeleteSomeSubCategoryTextProcessor();
                        return await processor.Execute(u, botClient);
                    }
                default:
                    {
                        return await ExecuteWhenChosenSubcategory(u, botClient, entityTypeUpper, categoryName);
                    }
            }
        }

        private async Task<Messages> ExecuteWhenChosenSubcategory(UserData u, TelegramBotClient botClient, string entityTypeUpper,
            string categoryName)
        {
            var reposFactory = new FinanceEntityRepositoryFactory();
            var reposOfFinanceEntity = reposFactory.GetRepositoryInstanceFromItsUpperName(entityTypeUpper);

            if (reposOfFinanceEntity.IsSubcategoryExists(u.UserText))
                return await ExecuteIfValidated(u, botClient, entityTypeUpper, categoryName);
            else if (u.UserText.Length < 5)
                return await SendMsg(botClient, u.UserId, $"⚠️ Необходимо выбрать субкатегорию из списка");
            else
                return await SendMsg(botClient, u.UserId, $"⚠️ Данная субкатегория удалена или переименована. В случае ошибки обратитесь к @Yan_stan");
        }

        async private Task<Messages> ExecuteIfValidated(UserData u, TelegramBotClient botClient, string upperFinanceEntity,
            string categoryName)
        {
            var reposOfUser = new UserRepository();
            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormUsedWalletsKeyboard(upperFinanceEntity);
            reposOfUser.SetUserChatStatus(u.UserId, $"WAIT/ADDWALLETSTOENTITY/{upperFinanceEntity}/CATEGORY/{categoryName}/SUBCATEGORY/{u.UserText}");

            var msg = await botClient.SendTextMessageAsync(u.UserId, $"Выбрана субкатегория: {u.UserText}\n" +
                $"<b>Теперь введи название кошелька, с которого переводишь</b>", ParseMode.Html, replyMarkup: keyboard);
            return GetMessages(msg);
        }

        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText));
    }
}
