using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Diagnostics;
using Telegram.Bot.Types.Enums;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyCounter.Commands
{
    class StartCommand : Command
    {
        public override string Name { get; set; } = "/start";

        public override async Task<Messages> Execute(MessageEventArgs e, TelegramBotClient botClient)
        {
            int userId = e.Message.From.Id;
            UserRepository reposOfUser = new UserRepository();
            Stopwatch sw = Stopwatch.StartNew();
            reposOfUser.SetUserIfNotExist(e);
            reposOfUser.SetAdminIfRootUser(userId);
            reposOfUser.SetUserChatStatus(userId, "START/WAS/COMMANDED");
            Console.WriteLine($"Create user in DB: {sw.ElapsedMilliseconds} ms");
            sw.Restart();
            var messages = await FormMsg(e, botClient, reposOfUser);
            Console.WriteLine($"Total send: {sw.ElapsedMilliseconds} ms");
            return messages;
        }

        public async Task<Messages> FormMsg(MessageEventArgs e, TelegramBotClient botClient, UserRepository reposOfUser)
        {
            var x = new KeyboardFormer();
            InlineKeyboardMarkup startInlMarkup;
            if (reposOfUser.IsUserAdmin(e.Message.From.Id))
                startInlMarkup = x.FormStartCmdTextKeyboardForAdmin();
            else
                startInlMarkup = x.FormStartCmdTextKeyboardForNoAdmin();
            var msg = await botClient.SendTextMessageAsync(e.Message.Chat.Id, parseMode: ParseMode.Markdown, text:
                    $"Бухгалтер-бот приветствует тебя, {e.Message.From.FirstName}! " +
                    $"Что будешь делать?", replyMarkup: startInlMarkup);
            var messagesList = new List<Message>() { msg };
            var messages = new Messages(messagesList);
            return messages;
        }
    }
}