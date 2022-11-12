using Hangfire;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;

namespace MoneyCounter.Backup
{
    class DataBaseBackuper
    {
        public void LaunchSheduler()
        {
            RecurringJob.AddOrUpdate(
                () => Execute(),
                Cron.DayInterval(1));
        }

        public async Task Execute()
        {
            string backupFileName = SetBackUpToHardDrive();
            await SendFile(backupFileName);
        }
        public string SetBackUpToHardDrive()
        {
            Directory.CreateDirectory("Backups");
            var backupFileName = "Backups/MoneyCounter" + "ScheduledBackup.db";
            using (var source = new SqliteConnection("Data Source=MoneyCounterDb.db"))
            using (var destination = new SqliteConnection("Data Source=" + backupFileName))
            {
                source.Open();
                source.BackupDatabase(destination);
            }
            return backupFileName;
        }

        private static async Task SendFile(string backupFileName)
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(backupFileName);
            using var ms = new MemoryStream(bytes);
            var file = new InputOnlineFile(ms);
            string dateTime = DateTime.Now.ToString("dd.MM.yy(HHmmss)");
            BotClientWrapper botClientWrapper = new BotClientWrapper();
            await SendFileToStorage(botClientWrapper, file, dateTime);
        }

        private static async Task SendFileToStorage(BotClientWrapper botClientWrapper,
            InputOnlineFile file, string dateTime)
            => await botClientWrapper.SendDocumentAsync(-503709101, file, $"⏲ Бекап выполнен по расписанию!\nФайл создан: {dateTime}");
    }
}
