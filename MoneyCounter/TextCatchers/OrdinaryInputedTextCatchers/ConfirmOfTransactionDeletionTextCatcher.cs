using MoneyCounter.Models;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.TextCatchers.OrdinaryInputedTextCatchers
{
    public class ConfirmOfTransactionDeletionTextCatcher
    {
        public async Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            int adminId = u.UserId;
            UserRepository reposOfuser = new UserRepository();
            var adminUsername = reposOfuser.GetUserFromUserId(adminId).UserName;
            var userId = int.Parse(u.UserStatusArray[6]);
            var transactionId = int.Parse(u.UserStatusArray[7]);
            var delRepos = new TransactionsForDeletionRepository();
            var transRepos = new TransactionRepository();
            var trans = transRepos.GetTransactionById(transactionId);

            if(u.UserText.StartsWith("✅ Удалить перевод №"))
            {
                delRepos.InactivateTransactionIdForDeletion(userId, transactionId, true);
                reposOfuser.SetUserChatStatus(adminId, "ADMIN/CONFIRMATION/APPROVED!");
                return await ProvideTransactionDeletion(botClient, userId, adminId, trans);
            }
            if (u.UserText.StartsWith("❌ Отказать в удалении"))
            {
                delRepos.InactivateTransactionIdForDeletion(userId, transactionId, false);
                reposOfuser.SetUserChatStatus(adminId, "ADMIN/CONFIRMATION/REJECTED!");
                return await AnswerAboutRejection(botClient, adminUsername, userId, trans);
            }
            return await SendMsg(botClient, userId, $"⚠️ Неверный ввод. Выберите и нажмите одну из кнопок.");

        }

        private static async Task<Messages> ProvideTransactionDeletion(TelegramBotClient botClient, int userId, int adminId,
            Transaction trans)
        {
            string transDescription = GetTransactionTextDescription(trans);
            var repos = new TransactionRepository();
            repos.DeleteTransactionById(trans.Id);
            var msg1 = await botClient.SendTextMessageAsync(userId, $"Удаление трансакции успешно выполнено!\n\n" +
                $"✅ " + transDescription);
            var msg2 = await botClient.SendTextMessageAsync(adminId, $"Удаление трансакции успешно выполнено!\n\n" +
                $"✅ " + transDescription);
            return Get2Messages(msg1, msg2);
        }

        private static async Task<Messages> AnswerAboutRejection(TelegramBotClient botClient, string adminUsername,
            int userId, Transaction trans)
        {
            string transDescription = GetTransactionTextDescription(trans);
            return await SendMsg(botClient, userId, $"❌ Вам отказано в удалении трансакции " +
                $"администратором {adminUsername}.\nТрансакция:\n\n" + transDescription);
        }

        private static string GetTransactionTextDescription(Transaction trans)
        {
            return $"id: {trans.Id}\n" +
                $"{trans.FromWallet} => {trans.ToWallet} ({trans.MoneyAmount})\n" +
                $"Автор: {trans.FirstName}({trans.Username})\n" +
                $"{trans.Subcategory}, {GetType(trans.IsFinal, trans.IsGain, trans.IsReversal, trans.IsStart)}\n" +
                $"Дата и время: {trans.DatetimeOfFinish}\n\n";
        }

        //TODO make class that has this methods.
        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
        private static Messages Get2Messages(Message msg1, Message msg2) => new Messages(new List<Message>() { msg1, msg2 });

        public static string GetType(bool isFinal, bool isGain, bool isReversal, bool isStart)
        {
            if (isGain)
                return "Чистый доход";
            if (isReversal)
                return "Реверсивный";
            if (isStart && isFinal)
                return "И начальный, и конечный";
            if (isStart && !isFinal)
                return "Начальный";
            if (!isStart && isFinal)
                return "Конечный";
            if (!isStart && !isFinal)
                return "Не начальный и не конечный";
            return "⚠️ Error 7! Please write to @Yan_stan";
        }

    }
}
