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
    public class AdminNameForTransactDeletionTextCatcher
    {
        public async Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            string adminUserName = u.UserText;
            UserRepository reposOfuser = new UserRepository();
            int? adminId = reposOfuser.GetIdFromUserName(adminUserName);
            int transactionId = int.Parse(u.UserStatusArray[5]);

            if (!adminUserName.StartsWith("@"))
                return await SendMsg(botClient, u.UserId, $"⚠️ Неверный ввод @username администратора!");
            if (adminId == null)
                return await SendMsg(botClient, u.UserId, $"⚠️ Такого пользователя не существует!");
            if (!reposOfuser.IsUserAdmin((int)adminId))
                return await SendMsg(botClient, u.UserId, $"⚠️ Этот пользователь не является администратором!");

            var trans = GetTransactionForDeletion(u.UserId, adminUserName, transactionId, adminId);
            reposOfuser.SetUserChatStatus(u.UserId, $"WAIT/CONFIRM/TRANSACTION/DELETION/FOR/OPERATOR/{u.UserId}/{trans.Id}");
            return await RequestAdminForDeletionPermission(u, botClient, trans);
            }

        private async Task<Messages> RequestAdminForDeletionPermission(UserData u, TelegramBotClient botClient, Transaction trans)
        {
            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormYesOrNoConfirmDeletion(trans.Id);
            var msg1 = await botClient.SendTextMessageAsync(u.UserId, $"Оператор бухгалтера @{u.Username} " +
                $"запрашивает разрешение на удаление трансакции:\n\n" +
               $"✅ id: {trans.Id}\n" +
               $"{trans.FromWallet} => {trans.ToWallet} ({trans.MoneyAmount})\n" +
               $"Автор: {trans.FirstName}({trans.Username})\n" +
               $"{trans.Subcategory}, {GetType(trans.IsFinal, trans.IsGain, trans.IsReversal, trans.IsStart)}\n" +
               $"Дата и время: {trans.DatetimeOfFinish}",
                replyMarkup: keyboard);
            var msg2 = await botClient.SendTextMessageAsync(u.UserId, "Запрос на подтверждение отправлен! Уведомление будет прислано в этот чат.");
            return GetMessages(msg1, msg2);

        }

        private static Transaction GetTransactionForDeletion(int userId, string adminUserName, int transId, int? adminId)
        {
            var delRepos = new TransactionsForDeletionRepository();
            delRepos.SetFinalTransactionIdForDeletion(userId, (int)adminId, transId, adminUserName);
            var transRepos = new TransactionRepository();
            var trans = transRepos.GetTransactionById(transId);
            return trans;
        }

        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText, ReplyKeyboardMarkup keyboard = null) =>
            GetMessage(await botClient.SendTextMessageAsync(userId, msgText, replyMarkup: keyboard));
        private static Messages GetMessage(Message msg) => new Messages(new List<Message>() { msg });
        private static Messages GetMessages(Message msg1, Message msg2) => new Messages(new List<Message>() { msg1, msg2 });

        public string GetType(bool isFinal, bool isGain, bool isReversal, bool isStart)
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
