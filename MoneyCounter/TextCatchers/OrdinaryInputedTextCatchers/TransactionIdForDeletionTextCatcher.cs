using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.TextCatchers.OrdinaryInputedTextCatchers
{
    class TransactionIdForDeletionTextCatcher
    {
        public async Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            TransactionRepository repos = new TransactionRepository();
            if (!int.TryParse(u.UserText, out int transactionId))
                return await SendMsg(botClient, u.UserId, $"⚠️ Не удалось превратить введенную сумму в число. Необходимы целочисленные значения. Попробуй еще раз!");
            if (!repos.IsTransactionIdExists(transactionId))
                return await SendMsg(botClient, u.UserId, "⚠️ Трансакции с номером не существует или она уже была удалена!");
            if (repos.IsNextTransactionsWithThisWallets(transactionId))
                return await SendMsg(botClient, u.UserId, "🧑‍💻 Не могу разрешить эту операцию, так как кошельки," +
                    " использованные в этой трансакции, были использованы в последующих трансакциях. Попробуйте исправить трансакцию вместо её удаления.");
            else
                return await ExecuteAfterValidation(u, botClient, transactionId);
        }

        private static async Task<Messages> ExecuteAfterValidation(UserData u, TelegramBotClient botClient, int transactionId)
        {
            UserRepository reposOfUser = new UserRepository();
            if (reposOfUser.IsUserAdmin(u.UserId))
                return await ExecuteForAdmin(botClient, u.UserId, transactionId);
            else
                return await ExecuteForNonAdmin(u, botClient, reposOfUser, transactionId);
        }

        private static async Task<Messages> ExecuteForNonAdmin(UserData u, TelegramBotClient botClient, UserRepository reposOfUser,
            int transactionId)          
        {
            string operatorUserName = "@" + u.Username;    
            var delRepos = new TransactionsForDeletionRepository();
            delRepos.SetActiveTransactionIdForDeletion(transactionId, u.UserId, operatorUserName);

            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormUsernames(true);
            reposOfUser.SetUserChatStatus(u.UserId, $"WAIT/INPUTFOR/TRANSACTION/DELETION/ADMINNAME/{transactionId}");
            return await SendMsg(botClient, u.UserId, "Выбери администратора для подтверждения", keyboard);
        }
        private static async Task<Messages> ExecuteForAdmin(TelegramBotClient botClient, int userId, int transactionId)
        {
            var repos = new TransactionRepository();
            repos.DeleteTransactionById(transactionId);
            return await SendMsg(botClient, userId, $"✔️ Удаление трансакции №{transactionId} выполнено!");
        }

        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
    }
}
