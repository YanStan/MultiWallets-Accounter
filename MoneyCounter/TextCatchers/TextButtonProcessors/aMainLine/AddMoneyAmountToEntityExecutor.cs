using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    class AddMoneyAmountToEntityExecutor
    {
        async public Task<Messages> AddMoneyToFinanceEntity(UserData u, TelegramBotClient botClient, string withdrawSumText,
            string multipliedAdjunctionSumText = "0 грн")
        {
            string entityTypeUpper = u.UserStatusArray[2];
            return entityTypeUpper switch
            {
                "TRANSACTION" => await TransactionResponse(u, botClient, withdrawSumText, multipliedAdjunctionSumText),
                "FOUNDERSTRANSACTION" => await FoundersTransactionResponse(u, botClient),
                _ => GetMessages(await botClient.SendTextMessageAsync(u.UserId, "4⚠️ Error! Please write to @Yan_stan")),
            };
        }

        async private Task<Messages> FoundersTransactionResponse(UserData u, TelegramBotClient botClient)
        {
            UserRepository reposOfUser = new UserRepository();
            var repos = new FoundersTransactionRepository();
            var sourceWallet = u.UserStatusArray[7].Split(">")[0];
            var destWallet = u.UserStatusArray[7].Split(">")[1];
            repos.SetEntity(u);
            var msg = await botClient.SendTextMessageAsync(u.UserId, "🍺 Фаундерский перевод добавлен:\n" +
                $"Перевод с кошелька: {sourceWallet}\n" +
                $"На кошелек: {destWallet}\n" +
                $"Сумма: {u.UserText}\n\n" +
                "Что будем делать дальше?");
            reposOfUser.SetUserChatStatus(u.UserId, "FOUNDERS/TRANSACTION/ADDED!");
            return GetMessages(msg);
        }
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });

        async private Task<Messages> TransactionResponse(UserData u, TelegramBotClient botClient, string withdrawSumText,
            string multipliedAdjunctionSumText)
        {
            UserRepository reposOfUser = new UserRepository();
            var categoryName = u.UserStatusArray[4];
            var subcategoryName = u.UserStatusArray[6];
            var sourceWallet = u.UserStatusArray[7].Split(">")[0];
            var destWallet = u.UserStatusArray[7].Split(">")[1];
            reposOfUser.SetUserChatStatus(u.UserId, $"WAIT/ADDTRANSACTIONTYPE/CATEGORY/{categoryName}/SUBCATEGORY/{subcategoryName}/{sourceWallet}>{destWallet}/{withdrawSumText}/{multipliedAdjunctionSumText}");
            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormTransactionTypeKeyboard(); 
            var msg = await botClient.SendTextMessageAsync(u.UserId, "🧵 Выбери тип перевода:\n" +
                            "--------------\n" +
                            "👨‍✈️ И начальный, и конечный\n" +
                            "    (платеж основателем за посторонние услуги или товары)\n" +
                            "👨‍✈️ Начальный\n" +
                            "    (перевод со счета основателя на счет сотрудника)\n" +
                            "👨‍💼 Конечный\n" +
                            "    (платеж со счета сотрудника на посторонний счет)\n" +
                            "👨‍💼 Не начальный и не конечный\n" +
                            "    (перевод с одного счета сотрудника на другой счет сотрудника)\n" +
                            "👨‍💼 Реверсивный\n" +
                            "    (перевод неиспользованных средств со счета сотрудника обратно на счет основателя)\n\n" +
                            "--------------\n" +
                            "🟢❇️  <u>Чистый доход</u> ❇️ 🟢\n" +
                            "    (вывод личных средств на счет основателя)",
                            replyMarkup: keyboard, parseMode: ParseMode.Html);
            return GetMessages(msg);
        }
    }
}

