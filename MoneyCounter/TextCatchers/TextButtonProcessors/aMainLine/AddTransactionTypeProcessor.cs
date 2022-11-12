using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoneyCounter.TextButtonProcessors.aMainLine
{
    class AddTransactionTypeProcessor : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            bool isChosen = u.UserText == "👨‍✈️🔛 И начальный, и конечный" || u.UserText == "👨‍✈️🔜 Начальный" ||
                u.UserText == "👨‍💼⏭ Конечный" || u.UserText == "👨‍💼🔄 Не начальный и не конечный" ||
                u.UserText == "👨‍💼🔙 Реверсивный" || u.UserText == "🟢❇️  Чистый доход ❇️ 🟢";
            Message msg;
            Messages messages;
            if (isChosen)
                msg = await Validate(u, botClient);
            else
                msg = await botClient.SendTextMessageAsync(u.UserId, "⚠️ Выбери тип перевода!");

            var messagesList = new List<Message>() { msg };
            messages = new Messages(messagesList);
            return messages;
        }

        private static async Task<Message> Validate(UserData u, TelegramBotClient botClient)
        {
            bool isStart = false;
            bool isFinal = false;
            bool isReversal = false;
            bool isGain = false;
            switch (u.UserText)
            {
                case "👨‍✈️🔛 И начальный, и конечный":
                    isStart = true;
                    isFinal = true;
                    break;
                case "👨‍✈️🔜 Начальный":
                    isStart = true;
                    break;
                case "👨‍💼⏭ Конечный":
                    isFinal = true;
                    break;
                case "👨‍💼🔄 Не начальный и не конечный":
                    break;
                case "👨‍💼🔙 Реверсивный":
                    isReversal = true;
                    break;
                case "🟢❇️  Чистый доход ❇️ 🟢":
                    isGain = true;
                    break;
            }
            string transactionType = $"{isStart}*{isFinal}*{isReversal}*{isGain}";
            return await ExecuteAfterValidation(u, botClient, transactionType);
        }

        private static async Task<Message> ExecuteAfterValidation(UserData u, TelegramBotClient botClient,
             string transactionType)
        {
            string category = u.UserStatusArray[3];
            string subcategory = u.UserStatusArray[5];
            string firstWallet = u.UserStatusArray[6].Split(">")[0];
            string secondWallet = u.UserStatusArray[6].Split(">")[1];
            string moneyAmount = u.UserStatusArray[7];
            string multipliedAdjunctionSumText = u.UserStatusArray[8];
            string answerText = $"💫🧑‍💻<b> Все ли данные введены верно?</b>\n" +
                $"Категория: {category}\n" +
                $"Субкатегория: {subcategory}\n" +
                $"С кошелька: {firstWallet}\n" +
                $"На кошелек: {secondWallet}\n" +
                $"Сумма: {moneyAmount}\n" +
                $"Тип перевода: {u.UserText}";

            var msg = await SendMsgAboutSuccess(botClient, u.UserId, answerText);

            var userRepository = new UserRepository();
            userRepository.SetUserChatStatus(u.UserId, $"WAIT/CONFIRMTRANSACTION/CATEGORY/{category}" +
                $"/SUBCATEGORY/{subcategory}/{firstWallet}>{secondWallet}" +
                $"/{moneyAmount}/{multipliedAdjunctionSumText}/{transactionType}");
            return msg;
        }
        private static async Task<Message> SendMsgAboutSuccess(TelegramBotClient botClient, int userId, string answerText)
        {
            var former = new KeyboardFormer();
            var keyboard = former.FormTransactionConfidenceAnswers();
            return await botClient.SendTextMessageAsync(userId, answerText,
                replyMarkup: keyboard, parseMode: ParseMode.Html);
        }
    }
}
