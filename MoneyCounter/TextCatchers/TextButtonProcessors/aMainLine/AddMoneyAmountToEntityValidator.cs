using MoneyCounter.Repositories;
using MoneyCounter.TextButtonProcessors;
using MoneyCounter.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    public class AddMoneyAmountToEntityValidator : TextProcessor
    {
        async public override Task<Messages> Execute(UserData u, TelegramBotClient botClient)
        {
            var repositoryFactory = new FinanceEntityRepositoryFactory();
            var repos = repositoryFactory.GetRepositoryInstanceFromItsUpperName(u.UserStatusArray[2]);
            string withdrawSumText = GetPreValidatedMessageText(u.UserText);
            return await Validate(u, botClient, repos, withdrawSumText);
        }

        private async Task<Messages> Validate(UserData u, TelegramBotClient botClient, FinanceEntityRepository repos,
            string withdrawSumText)
        {
            var msgArray = withdrawSumText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            bool isWrCurrency = GetIsWrongCurrency(msgArray);

            if (msgArray.Length < 2)
                return await SendMsg(botClient, u.UserId, $"⚠️ Неверный формат ввода денежной суммы. Попробуй еще раз в формате \"X грн\"");
            if (isWrCurrency)
                return await SendMsg(botClient, u.UserId, $"⚠️ Неверный формат ввода. Неизвестная валюта");
            if (!int.TryParse(msgArray[0], out _))
                return await SendMsg(botClient, u.UserId, $"⚠️ Не удалось превратить введенную сумму в число. Необходимы целочисленные значения. Попробуй еще раз");
            if (msgArray[0].StartsWith("-"))
                return await SendMsg(botClient, u.UserId, $"⚠️ Наш бот не поддерживает введение отрицательных сумм. Попробуй еще раз с положительным числом.");
            if (u.UserText.Contains('/'))
                return await SendMsg(botClient, u.UserId, "⚠️ К сожалению, символ \"/\" запрещен для ввода. Все остальные символы разрешены");

            return await ValidateBMAlert(u, botClient, withdrawSumText, repos);
        }

        private static async Task<Messages> ValidateBMAlert(UserData u, TelegramBotClient botClient, 
            string withdrawSumText, FinanceEntityRepository repos)
        {
            string sourceWalletName = u.UserStatusArray[7].Split(">")[0];
            bool shouldBmMultiplyingAlert = GetShouldBmAlert(u, repos, withdrawSumText, sourceWalletName);
            int walletSum = repos.GetLastSumOnWallet(sourceWalletName);
            int withdrawSum = int.Parse(withdrawSumText.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);

            if (shouldBmMultiplyingAlert)
            {
                var keyboard = GetAlertKeyboard(walletSum, withdrawSum);
                string msgAlertText = FormMsgAlertText(withdrawSumText, sourceWalletName, walletSum);
                UpdateUserStatusWithWasAlertMarker(u);
                return await SendBalanceMultilyingAlert(u.UserId, botClient, msgAlertText, keyboard);
            }
            else
            {
                string adjunctionSumText = GetAdjunctionSumText(u.UserText, walletSum, withdrawSum);//here we need raw text
                return await SendMsgAboutSuccess(u, botClient, withdrawSumText, adjunctionSumText);
            }
        }

        private static void UpdateUserStatusWithWasAlertMarker(UserData u)
        {
            UserRepository reposOfUser = new UserRepository();
            var newStatusSubstring = u.UserStatus.Remove(0, 22);
            var newUserStatus = "WAIT/ADDMONEYTOENTITY//" + newStatusSubstring;
            reposOfUser.SetUserChatStatus(u.UserId, newUserStatus);
        }

        private static ReplyKeyboardMarkup GetAlertKeyboard(int walletSum, int withdrawSum)
        {
            KeyboardFormer former = new KeyboardFormer();
            var keyboard = former.FormTransactAllMoneyOrMultiplyKeyboard(walletSum, withdrawSum);
            return keyboard;
        }

        private static string FormMsgAlertText(string withdrawSumText, string sourceWalletName, int walletSum)
        {
            return $"❎👁 На кошельке \"{sourceWalletName}\" не хватает денег.\n" +
            $"Вы хотите снять {withdrawSumText}, в то время как на кошельке всего {walletSum} грн.\n" +
            "Вы уверены, что хотите подтвердить операцию?\n\n" +
            "<b>Внимание! Ошибка может привести к введению ложных данных! Подтверждать операцию стоит, только если на балансе был произведен заработок средств.</b>";
        }

        private static async Task<Messages> SendBalanceMultilyingAlert(int userId, TelegramBotClient botClient, string msgAlertText,
            ReplyKeyboardMarkup keyboard)
        {
            var msg = await botClient.SendTextMessageAsync(userId, msgAlertText,
                ParseMode.Html, replyMarkup: keyboard);
            return GetMessages(msg);
        }

        private static bool GetIsWrongCurrency(string[] msgArray) =>
            msgArray.Length >= 2 && msgArray[1] != "грн" && msgArray[1] != "грн." && msgArray[1] != "гривен" && msgArray[1] != "гривен.";
        private static bool GetShouldBmAlert(UserData u, FinanceEntityRepository repos, string withdrawSumText, string sourceWalletName)
        {
            string entityNameUpper = u.UserStatusArray[2];
            return repos.WasThisWalletEverUsed(sourceWalletName)
                && !repos.WasThisWalletSourceAndStart(sourceWalletName)
                && !IsEnoughMoneyOnSourceWallet(sourceWalletName, withdrawSumText, entityNameUpper) &&
                u.UserStatus[22] != '/';
        }

        private static async Task<Messages> SendMsgAboutSuccess(UserData u, TelegramBotClient botClient, string withdrawSumText,
            string adjunctionSumText)
        {
            AddMoneyAmountToEntityExecutor executor = new AddMoneyAmountToEntityExecutor();
            return await executor.AddMoneyToFinanceEntity(u, botClient, withdrawSumText, adjunctionSumText);
        }

        private static string GetAdjunctionSumText(string rawMsgText, int walletSum, int withdrawSum)
        {
            string substr = "Подтвердить снятие заработанных ";
            return rawMsgText.StartsWith(substr) ? $"{withdrawSum - walletSum} грн" : "0 грн";
        }

        private string GetPreValidatedMessageText(string text)
        {
            string substring1 = "Вся сумма (";
            string substring2 = "Подтвердить снятие заработанных ";
            if (text.StartsWith(substring1))
                return text.Remove(text.Length - 1).Remove(0, substring1.Length);
            return text.StartsWith(substring2) ? text.Remove(0, substring2.Length) : text;
        }

        private static bool IsEnoughMoneyOnSourceWallet(string sourceWalletName, string withdrawSumText, string entityNameUpper)
        {
            if (entityNameUpper == "FOUNDERSTRANSACTION")
                return true;
            TransactionRepository repos = new TransactionRepository();
            int walletSum = repos.GetLastSumOnWallet(sourceWalletName);
            int withdrawSum = int.Parse(withdrawSumText.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]);
            return withdrawSum <= walletSum;
        }
        private static async Task<Messages> SendMsg(TelegramBotClient botClient, int userId, string msgText) =>
            GetMessages(await botClient.SendTextMessageAsync(userId, msgText));
        private static Messages GetMessages(Message msg) => new Messages(new List<Message>() { msg });


    }

}
