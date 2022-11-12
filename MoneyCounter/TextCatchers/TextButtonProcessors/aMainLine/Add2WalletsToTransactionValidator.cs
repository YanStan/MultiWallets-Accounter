using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;
using MoneyCounter.Analyzers;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    class Add2WalletsToTransactionValidator
    {
        async public Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var walletName = u.UserText;
            if (walletName == "Нет, я ошибся вводом")
                return await SendMsg(botClient, u.UserId, "Введите правильное название кошелька");
            if (walletName.Length < 5)
                return await SendMsg(botClient, u.UserId, "⚠️ Неверный формат ввода кошелька. Название не может быть короче 5 символов.");
            if (walletName.Contains(" грн"))
                return await SendMsg(botClient, u.UserId, "⚠️ Неверный формат ввода кошелька. Вы пытаетесь ввести сумму денег.");
            if (walletName.Contains('/'))
                return await SendMsg(botClient, u.UserId, "⚠️ К сожалению, символ \"/\" запрещен для ввода. Все остальные символы разрешены");
            if (walletName.Contains('>'))
                return await SendMsg(botClient, u.UserId, "⚠️ К сожалению, символы \">\" и \"/\"запрещены в названиях кошельков. Все остальные символы разрешены");

            if (walletName.StartsWith("Да, перевести с "))
                walletName = walletName.Replace("Да, перевести с ", string.Empty);
            return await ExecuteAfterValidation(u, botClient, walletName);
        }



        private static async Task<Messages> ExecuteAfterValidation(UserData u, TelegramBotClient botClient, string walletName)
        {
            Message msg;
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            walletName = myTI.ToTitleCase(walletName.ToLower());
            bool wasIteratedAboutMistypedWallet = u.UserStatusArray.Last().Contains("(IterOfMisstypedWallet)");
            string similarWallet = FindSimilarWalletName(walletName);
            if (similarWallet != null && !wasIteratedAboutMistypedWallet)
                msg = await HandleHaveYouMentionedPrevWallet(u, botClient, walletName, similarWallet);
            else
                msg = await HandleNoMentionedPrevWallet(u, botClient, walletName, wasIteratedAboutMistypedWallet);
            return GetMessages(msg);
        }

        private static async Task<Message> HandleNoMentionedPrevWallet(UserData u, TelegramBotClient botClient, string walletName,
            bool wasIteratedAboutMistypedWallet)
        {
            UserRepository reposOfUser = new UserRepository();
            if (wasIteratedAboutMistypedWallet)
            {
                var userChatStatus = u.UserStatus.Replace("(IterOfMisstypedWallet)", string.Empty);
                reposOfUser.SetUserChatStatus(u.UserId, userChatStatus);
                u.SetNewStatusData(userChatStatus);
            }
            return await HandleSomeWalletInput(u, botClient, walletName);
        }

        private static async Task<Message> HandleSomeWalletInput(UserData u, TelegramBotClient botClient, string walletName)
        {
            Message msg;
            if (u.UserStatusArrLen == 7)
                return await HandleFirstWalletInput(u, botClient, walletName);
            if (u.UserStatusArrLen == 8 && u.UserStatusArray[7].Contains(">"))
                return await HandleSecondWalletInput(u, botClient, walletName);
            msg = await botClient.SendTextMessageAsync(u.UserId, "null! Show it to admin @Yan_stan"); //TODO SomeEX
            return msg;
        }

        private static async Task<Message> HandleSecondWalletInput(UserData u, TelegramBotClient botClient, string walletName)
        {
            string firstWallet = u.UserStatusArray[7].Split(">")[0];
            if (walletName == firstWallet)
                return await botClient.SendTextMessageAsync(u.UserId, "⚠️ Этот кошелек вы уже ввели как стартовый");

            string categoryName = u.UserStatusArray[4];
            string subcategoryName = u.UserStatusArray[6];
            string sourceWallet = u.UserStatusArray[7].Split(">")[0];
            string destWallet = u.UserText;
            string textaboutSuccess = FormTextaboutSuccessfulEntityCreation(categoryName, subcategoryName, sourceWallet, destWallet);
            var executor = new Add2WalletsToTransactionExecutor();
            UserRepository reposOfUser = new UserRepository();
            executor.AddSecondWalletToUserStatusData(u.UserStatusArray, walletName, reposOfUser, u.UserId);
            return await SendMsgAboutSuccess(u, botClient, sourceWallet, textaboutSuccess);
        }

        private static async Task<Message> SendMsgAboutSuccess(UserData u, TelegramBotClient botClient, string sourceWallet,
            string textaboutSuccess)
        {
            var entityNameUpper = u.UserStatusArray[2];
            var reposofTrans = new TransactionRepository();
            var keyboardFormer = new KeyboardFormer();
            ReplyKeyboardMarkup keyboard;
            if (entityNameUpper == "FOUNDERSTRANSACTION" ||
                !reposofTrans.WasThisWalletEverUsed(sourceWallet) || reposofTrans.WasThisWalletSourceAndStart(sourceWallet))
            {
                keyboard = null;
            }
            else
            {
                int moneySum = reposofTrans.GetLastSumOnWallet(sourceWallet);
                keyboard = keyboardFormer.FormTransactAllMoneyKeyboard(moneySum);
            }
            var msg = await botClient.SendTextMessageAsync(u.UserId, $"💫🧑‍💻 <b>Следующие данные о переводе будут добавлены:</b>\n" +
                textaboutSuccess + "\n\nОтправь сумму сообщением в формате:\nX грн", ParseMode.Html, replyMarkup: keyboard);
            return msg;
        }

        private static async Task<Message> HandleFirstWalletInput(UserData u, TelegramBotClient botClient, string walletName)
        {
            Message msg;
            var entityNameUpper = u.UserStatusArray[2];
            var reposofTrans = new TransactionRepository();
            bool wasIteratedAboutFMistypedWallet = u.UserStatusArray.Last().Contains("(IterOfMisstypedFoundersWallet)");

            if (!reposofTrans.WasThisWalletEverUsed(walletName) && entityNameUpper != "FOUNDERSTRANSACTION"
                && !wasIteratedAboutFMistypedWallet)
                msg = await HandleUnknownSourceWallet(u, botClient, walletName);
            else
                msg = await HandleFstWalletAddition(u, botClient, walletName, wasIteratedAboutFMistypedWallet);
            return msg;
        }

        private static async Task<Message> HandleFstWalletAddition(UserData u, TelegramBotClient botClient, string walletName, 
            bool wasIteratedAboutFMistypedWallet)
        {
            string userChatStatus = u.UserStatus;
            var entityNameUpper = u.UserStatusArray[2];
            UserRepository reposOfUser = new UserRepository();
            if (wasIteratedAboutFMistypedWallet)
            {
                userChatStatus = userChatStatus.Replace("(IterOfMisstypedFoundersWallet)", string.Empty);
                reposOfUser.SetUserChatStatus(u.UserId, userChatStatus);
                u.SetNewStatusData(userChatStatus);
            }
            var executor = new Add2WalletsToTransactionExecutor();
            executor.AddFirstWalletToUserStatusData(u.UserStatusArray, walletName, reposOfUser, u.UserId);
            var former = new KeyboardFormer();
            var keyboard = former.FormUsedWalletsKeyboard(entityNameUpper);
            var msg = await botClient.SendTextMessageAsync(u.UserId, "Введи или выбери второй кошелек", replyMarkup: keyboard);
            return msg;
        }

        private static async Task<Message> HandleUnknownSourceWallet(UserData u, TelegramBotClient botClient, string walletName)
        {
            var former = new KeyboardFormer();
            var keyboard = former.FormAreYouSureItsFoundersWallet(walletName);
            var msg = await botClient.SendTextMessageAsync(u.UserId, "💫🧑‍💻 Вы пытаетесь перевести деньги с кошелька," +
                " который не был ранее известен. Это кошелек основателя?", replyMarkup: keyboard);
            UserRepository reposOfUser = new UserRepository();
            reposOfUser.SetUserChatStatus(u.UserId, u.UserStatus + "(IterOfMisstypedFoundersWallet)");
            return msg;
        }

        private static async Task<Message> HandleHaveYouMentionedPrevWallet(UserData u, TelegramBotClient botClient,
            string walletName, string similarWallet)
        {
            var former = new KeyboardFormer();
            var keyboard = former.FormMaybeYouHaveMentionedKeyboard(walletName, similarWallet);
            var msg = await botClient.SendTextMessageAsync(u.UserId, $"👁 Возможно, Вы имели ввиду: {similarWallet}\n" +
                $"Данный кошелек уже сохранен в базе.\n\n<b>Пожалуйста, выберите правильный вариант</b>", replyMarkup: keyboard, parseMode: ParseMode.Html);
            UserRepository reposOfUser = new UserRepository();

            reposOfUser.SetUserChatStatus(u.UserId, u.UserStatus + "(IterOfMisstypedWallet)");
            return msg;
        }

        private static string FormTextaboutSuccessfulEntityCreation(string categoryName, string subcategoryName, string sourceWallet, 
            string destWallet)
        {
            string text = $"Категория: {categoryName}\n" +
                    $"Подкатегория: {subcategoryName}\n" +
                    $"Перевод с кошелька: {sourceWallet}\n" +
                    $"На кошелек: {destWallet}";
            return text;
        }

        private static string FindSimilarWalletName(string inputWallet)
        {
            var repos = new TransactionRepository();
            var usedWallets = repos.GetAllUsedCompanyWallets();     //  AnswerIfExistsSimilar  float koef
            List<float> allKoefArray = new List<float> { };
            var len = inputWallet.Length;
            float gatePercentage = (float)(Math.Log(len, 9.7 + (1.1 * len)));
            usedWallets.ForEach(x => allKoefArray.Add((float)ComputeStringSimilarity.CalculateSimilarity(inputWallet, x)));
            var koefList = allKoefArray.Where(x => x < 1 && x >= gatePercentage).ToList();
            if (koefList.Count > 0)
            {
                int indexOfWallet = allKoefArray.IndexOf(koefList.Max());
                return usedWallets[indexOfWallet];
            }
            return null;
        }

        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });

    }
}
