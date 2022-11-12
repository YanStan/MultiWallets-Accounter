using MoneyCounter.Repositories;
using MoneyCounter.TextButtonProcessors.CustomizationProcessors;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MoneyCounter.TextButtonProcessors
{
    public class ChooseCategoryOfTransactionProcessor : TextProcessor
    {
        public string FinanceEntityType { get; set; } = "Transaction";

        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            Messages messages;
            switch (u.UserText)
            {
                case "⚙️ Добавить категорию ⚙️":
                    {
                        AddSomeCategoryTextProcessor processor = new AddSomeCategoryTextProcessor();
                        return await processor.Execute(u, botClient);
                    }

                case "⚙️ Переименовать к.":
                    {
                        RenameSomeCategoryTextProcessor processor = new RenameSomeCategoryTextProcessor();
                        return await processor.Execute(u, botClient);
                    }

                case "Удалить к. ⚙️":
                    {
                        DeleteSomeCategoryTextProcessor processor = new DeleteSomeCategoryTextProcessor();
                        messages = await processor.Execute(u, botClient);
                        break;
                    }

                default:
                    {
                        messages = await HandleChoosingCategory(u, botClient);
                        break;
                    }
            }
            return messages;
        }

        private async Task<Messages> HandleChoosingCategory(UserData u, TelegramBotClient botClient)
        {
            Message msg;
            TransactionRepository repos = new TransactionRepository();
            string[] subcategoriesNames = repos.GetEntitySubcategoriesOfCategory(u.UserText);
            string[] categoriesNames = repos.GetEntityCategoriesNames();
            msg = await Validate(u, botClient, subcategoriesNames, categoriesNames);
            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            return messages;
        }

        private async Task<Message> Validate(UserData u, TelegramBotClient botClient, string[] subcategoriesNames, string[] categoriesNames)
        {
            if (categoriesNames.Contains(u.UserText))
                return await ExecuteIfValidated(u, botClient, subcategoriesNames);
            else if (u.UserText.Length < 5)
                return await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Необходимо выбрать категорию из списка");
            else
                return await botClient.SendTextMessageAsync(u.UserId, $"⚠️ Данная категория удалена или переименована. В случае ошибки обратитесь к @Yan_stan");
        }

        private async Task<Message> ExecuteIfValidated(UserData u, TelegramBotClient botClient, string[] subcategoriesNames)
        {
            var userRepository = new UserRepository();
            var inlineKeyboardFormer = new KeyboardFormer();
            var subcategoriesKeyboard = inlineKeyboardFormer.FormSubCategoriesFromList(subcategoriesNames);
            var msg = await botClient.SendTextMessageAsync(u.UserId,
                $"Выбери подкатегорию в категории {u.UserText}", replyMarkup: subcategoriesKeyboard);
            userRepository.SetUserChatStatus(u.UserId, $"WAIT/CHOOSING/OR/CHANGE/TRANSACTION/SUBCATEGORY/{u.UserText}");
            return msg;
        }
    }
}