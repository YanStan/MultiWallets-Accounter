using MoneyCounter.Models;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoneyCounter.TextButtonProcessors.aMainLine
{
    class ConfirmTransactionProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var userRepository = new UserRepository();
            if (u.UserText == "Да, сохранить данные")
                return await SetTransaction(u, botClient, userRepository);
            if(u.UserText == "Нет, ввести другие данные")
                return await HandleRetypeData(u, botClient, userRepository);

            return await HandleMisstype(botClient, u.UserId);
        }

        private static async Task<Messages> SetTransaction(UserData u, TelegramBotClient botClient, UserRepository userRepository)
        {
            string multipliedAdjunctionSumText = u.UserStatusArray[8];

            var transactionRepository = new TransactionRepository();
            var transaction = transactionRepository.SetEntity(u);
            if (multipliedAdjunctionSumText != "0 грн")
                transactionRepository.SetBalanceMultiplication((Transaction)transaction, multipliedAdjunctionSumText);
            return await SendMessagesAboutSuccess(u, botClient, userRepository);
        }

        private static async Task<Messages> SendMessagesAboutSuccess(UserData u, TelegramBotClient botClient,  UserRepository userRepository)
        {
            string firstWallet = u.UserStatusArray[6].Split(">")[0];
            string secondWallet = u.UserStatusArray[6].Split(">")[1];

            var msg1 = await botClient.SendTextMessageAsync(u.UserId, "💸");
            var msg2 = await botClient.SendTextMessageAsync(u.UserId, "✅ Деньги уже в пути!\n" +
                        "Средства переведены с кошелька:\n" +
                        $"<b>{firstWallet}</b>\n" +
                        "На кошелек:\n" +
                        $"<b>{secondWallet}</b>\n\n" +
                        "Что будем делать дальше?:)", ParseMode.Html);
            userRepository.SetUserChatStatus(u.UserId, "TRANSACTION/ADDING/COMPLETED!");
            return GetMessages(msg1, msg2);
        }

        private static async Task<Messages> HandleRetypeData(UserData u, TelegramBotClient botClient, UserRepository userRepository)
        {
            userRepository.SetUserChatStatus(u.UserId, "RETYPE/ENTITY/DATA/WAS/CHOSEN!");
            var msg = await botClient.SendTextMessageAsync(u.UserId,
               $"💫🧑‍💻 Бухгалтер-бот к твоим услугам, {u.FirstName}! " +
               $"Что будешь делать?");
            return GetMessages(msg);
        }

        private static async Task<Messages> HandleMisstype(TelegramBotClient botClient, int userId)
        {
            var msg = await botClient.SendTextMessageAsync(userId, "⚠️ Допустимы следующие варианты ответа:\n" +
                "Да, сохранить данные\n" +
                "Нет, ввести другие данные");
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });
        private static Messages GetMessages(Message msg1, Message msg2) => new Messages(new List<Message>() { msg1, msg2 });
    }
}
