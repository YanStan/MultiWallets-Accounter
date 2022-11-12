using MoneyCounter.Backup;
using MoneyCounter.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace MoneyCounter.Commands
{
    class BackupCommand : Command
    {
        public override string Name { get; set; } = "/backup";
        public async override Task<Messages> Execute(MessageEventArgs e, TelegramBotClient botClient)
        {
            var backuper = new DataBaseBackuper();
            string backupFileName = backuper.SetBackUpToHardDrive();
            return await SendFiles(e, botClient, backupFileName);
        }


        private static async Task<Messages> SendFiles(MessageEventArgs e, TelegramBotClient botClient, string backupFileName)
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(backupFileName);
            using var ms = new MemoryStream(bytes);
            using var ms2 = new MemoryStream(bytes);
            var file = new InputOnlineFile(ms);
            var file2 = new InputOnlineFile(ms2);
            string dateTime = DateTime.Now.ToString("dd.MM.yy(HHmmss)");
            var msg1 = await SendFileToStorage(e, botClient, file2, dateTime);
            var msg2 = await SendFileToUser(e, botClient, file, dateTime);       
            return GetMessages(msg1, msg2);
        }

        private static async Task<Message> SendFileToUser(MessageEventArgs e, TelegramBotClient botClient,
            InputOnlineFile file, string dateTime)
           => await botClient.SendDocumentAsync(e.Message.Chat.Id, file, $"Бекап выполнен!\nФайл создан: {dateTime}");

        private static async Task<Message> SendFileToStorage(MessageEventArgs e, TelegramBotClient botClient,
            InputOnlineFile file, string dateTime)
        {
            string firstName = e.Message.From.FirstName;
            string username = e.Message.From.Username;
            return await botClient.SendDocumentAsync(-503709101, file, $"Бекап выполнен по требованию пользователя" +
                $" {firstName} (@{username})!\nФайл создан: {dateTime}");
        }
        private static Messages GetMessages(Message msg1, Message msg2) => new Messages(new List<Message>() { msg1, msg2 });

    }
}
