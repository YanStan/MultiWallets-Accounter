
using System.Threading.Tasks;
using Telegram.Bot;
using MoneyCounter.Commands;
using MoneyCounter.TextButtonProcessors;
using Telegram.Bot.Types.Enums;
using MoneyCounter.OrdinaryInputedTextCatchers;
using Telegram.Bot.Types;
using System.Collections.Generic;
using Telegram.Bot.Args;
using MoneyCounter.Wrappers;
using System.Linq;
using MoneyCounter.Models;
using MoneyCounter.TextButtonProcessors.aMainLine;
using MoneyCounter.Inline;
using MoneyCounter.aMainLine;
using MoneyCounter.TextButtonProcessors.CustomizationProcessors;
using MoneyCounter.TextCatchers.TextButtonProcessors.AnalyzeProcessors;
using MoneyCounter.TextCatchers.OrdinaryInputedTextCatchers;
using MoneyCounter.TextCatchers.TextButtonProcessors.AdminProcessors;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter
{
    class UserActionsHandler
    {
        async public Task<Messages> Bot_OnMessage(MessageEventArgs e, TelegramBotClient botClient)
        {
            int userId = e.Message.From.Id;
            try
            {
                await botClient.DeleteMessageAsync(userId, e.Message.MessageId);
            }
            catch //TODO message to delete not found Ex?
            {
            }

            Message msg;
            List<Message> messagesList;
            Messages messages;
            var reposOfUser = new UserRepository();
            string messageText = e.Message.Text;


            reposOfUser.InitializeWhiteListIfEmpty("@vladkheylo", "@Yan_stan");
            if (!reposOfUser.IsUserInWhiteList($"@{e.Message.From.Username}"))
                return await SendMsg(botClient, e.Message.From.Id, "⚠️ Ты не являешься оператором! Обратись к руководству за разрешением.");


            if (e.Message.Type == MessageType.Text)
            {
                if (messageText.StartsWith("/"))
                {
                    CommandFactory cmdFactory = new CommandFactory(); //CommandFactory
                    messages = await cmdFactory.RunCommandExecution(e, botClient);
                }
                else
                {
                    UserData userData = new UserData();
                    userData.SetWithStatusArray(e);
                    if (userData.UserStatus == "WAIT/CHOOSING/OR/CHANGE/TRANSACTION/CATEGORY")
                    {//var processor = ChooseCategoryOfTransactionProcessor();
                        ChooseCategoryOfTransactionProcessor processor = new ChooseCategoryOfTransactionProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/CHOOSING/OR/CHANGE/TRANSACTION/SUBCATEGORY"))
                    {   //var processor = ChooseSubcategoryOfFinanceEntityProcessor
                        var processor = new ChooseSubcategoryOfFinanceEntityProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/ADDMONEYTOENTITY"))
                    {
                        AddMoneyAmountToEntityValidator catcher = new AddMoneyAmountToEntityValidator();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/ADDWALLETSTOENTITY/"))
                    {
                        var catcher = new Add2WalletsToTransactionValidator();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/ADDTRANSACTIONTYPE/CATEGORY/"))
                    {
                        var processor = new AddTransactionTypeProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/CONFIRMTRANSACTION/CATEGORY/"))
                    {
                        var processor = new ConfirmTransactionProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/CHOOSING/ANALYSIS/TYPE"))//!!!!!!!!!!!
                    {
                        var processor = new ChoiceOfAnalysisTypeTextProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus == "WAIT/CHOOSING/HISTORY/WALLETSCOUNT/FOR/ANALYSIS")
                    {
                        var processor = new ChoiceOfHistoryWalletsCountTextProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/CHOOSING/ANALYSIS/TIMETYPE"))
                    {
                        var processor = new ChooseTimeAnalysisTypeProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/INPUTFOR/ANALYSIS/TIMETYPE/TIMETOTIME"))
                    {
                        var catcher = new Custom2DatetimesOfAnalysisTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/INPUTFOR/ANALYSIS/TIMETYPE/LASTNDAYS"))
                    {
                        var catcher = new CustomLastNDaysForAnalysisTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/INPUTFOR/ANALYSIS/TIMETYPE/WALLETSHISTORYONE"))
                    {
                        var catcher = new CustomWalletNameForHisHistoryTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus == "WAIT/INPUTFOR/TRANSACTION/DELETION/TRANSID")
                    {
                        var catcher = new TransactionIdForDeletionTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/INPUTFOR/TRANSACTION/DELETION/ADMINNAME"))
                    {
                        var catcher = new AdminNameForTransactDeletionTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/CONFIRM/TRANSACTION/DELETION/FOR/OPERATOR/"))
                    {
                        var catcher = new ConfirmOfTransactionDeletionTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus == "WAIT/CHOICEOF/USER/MANAGEMENT/TYPE")
                    {
                        var catcher = new UserManagementTypeTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/INPUTFOR/USER/MANAGEMENT/"))
                    {
                        var catcher = new UserManagementNameTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/ADDENTITY/") 
                        && userData.UserStatusArray.Length == 3)
                    {
                        var catcher = new AddSomeCategoryTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/ADDENTITY/") 
                        || (userData.UserStatus.StartsWith("WAIT/ADDSUBCATEGORY/") && userData.UserStatusArrLen == 5))
                    {
                        var catcher = new AddSomeSubcategoryTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/RENAME/ENTITY/") && userData.UserStatusArrLen == 4)
                    {
                        var processor = new ChooseForRenamingSomeCategoryTextProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/RENAME/ENTITY/") && userData.UserStatusArrLen == 6)
                    {
                        var catcher = new RenameSomeCategoryTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith($"WAIT/RENAME/SUBCATEGORY/") && userData.UserStatusArrLen == 6)
                    {
                        var processor = new ChooseForRenamingSomeSubcategoryTextProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith($"WAIT/RENAME/SUBCATEGORY/") && userData.UserStatusArrLen == 8)
                    {
                        var catcher = new RenameSomeSubcategoryTextCatcher();
                        messages = await catcher.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith("WAIT/DELETE/ENTITY/"))
                    {
                        var processor = new DeleteChosenCategoryTextProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else if (userData.UserStatus.StartsWith($"WAIT/DELETE/SUBCATEGORY/"))
                    {
                        var processor = new DeleteChosenSubcategoryTextProcessor();
                        messages = await processor.Execute(userData, botClient);
                    }
                    else
                    {
                        msg = await botClient.SendTextMessageAsync(userData.UserId, $"⚠️ Для введения текста или значения, выбери <b>субкатегорию</b> или введи команду", ParseMode.Html);
                        messagesList = new List<Message>() { msg };
                        messages = new Messages(messagesList);
                    }
                }
                messagesList = messages.readOnlyMessages.ToList();
                reposOfUser.SetBotChatMessages(messagesList); // ADD CURRENT BOT MESSAGE!
                await DeleteUserChatMessages(botClient, reposOfUser, userId);
                reposOfUser.DeleteExtraBotMessagesInDb(e.Message.From.Id); //DELETE EXTRA MESSAGES FROM DB!!
                return messages;
            }
            return null;
        }
        public async Task<Messages> Bot_OnCallbackQuery(CallbackQueryEventArgs c, TelegramBotClient botClient)
        {
            var chat = c.CallbackQuery.Message.Chat;
            UserRepository reposOfUser = new UserRepository();
            reposOfUser.InitializeWhiteListIfEmpty("@vladkheylo", "@Yan_stan");
            if (!reposOfUser.IsUserInWhiteList($"@{chat.Username}"))
                return await SendMsg(botClient, (int)chat.Id, "⚠️ Ты не являешься оператором! Обратись к руководству за разрешением.");

            var callbackQuery = c.CallbackQuery;          
            var inlineButtonProcessorFactory = new InlineProcessorFactory(); //TextFactory
            var messages = await inlineButtonProcessorFactory.RunInlineProcessor(callbackQuery, botClient);
            await DeleteBotOnCallbackQueryMessages(botClient, callbackQuery, messages);
            return messages;
        }

        private async Task DeleteBotOnCallbackQueryMessages(TelegramBotClient botClient, CallbackQuery c, Messages messages)
        {
            int userId = (int)c.Message.Chat.Id;
            var reposOfUser = new UserRepository();
            var messagesList = messages.readOnlyMessages.ToList();
            reposOfUser.SetBotChatMessages(messagesList); // ADD CURRENT BOT MESSAGE!
            await DeleteUserChatMessages(botClient, reposOfUser, userId);
            reposOfUser.DeleteExtraBotMessagesInDb(userId); //DELETE EXTRA MESSAGES FROM DB!!
        }

        private async Task DeleteUserChatMessages(TelegramBotClient botClient, UserRepository reposOfUser, int userId)
        {
            var listOfUserMessages = reposOfUser.GetBotMessagesWithUser(userId);
            if(listOfUserMessages.Count > 3)
            {
                int headerId = listOfUserMessages.IndexOf(listOfUserMessages.LastOrDefault(x => x.IsMainMenu == true));
                listOfUserMessages.RemoveAt(headerId);
                listOfUserMessages.RemoveAt(listOfUserMessages.Count - 1);
                listOfUserMessages.RemoveAt(listOfUserMessages.Count - 1);
                foreach (UserMessage userMessage in listOfUserMessages)
                {
                    try
                    {
                        await botClient.DeleteMessageAsync(userId, userMessage.MessageId);
                    }
                    catch{ }
                }
            }
        }
        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}