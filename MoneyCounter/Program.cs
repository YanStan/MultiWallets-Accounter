using System;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using MoneyCounter.Backup;
using Hangfire;
using Hangfire.Storage.SQLite;

namespace MoneyCounter
{
    class Program
    {
        private static void Main()
        {
            using (MoneyCounterContext db = new MoneyCounterContext())
            {
                db.Database.Migrate();
            }
            TelegramBotClient botClient = new TelegramBotClient("1448186834:AAF6Z8I2iKIcC87eQaEkF4BZTJI7x1vFNJg");
            StaticBotClientContainer.BotClient = botClient;
            UserActionsHandler handler = new UserActionsHandler();
            Console.WriteLine("start");
            botClient.OnMessage += async (sender, args) => await handler.Bot_OnMessage(args, botClient);
            botClient.OnCallbackQuery += async (sender, args) => await handler.Bot_OnCallbackQuery(args, botClient);
            botClient.StartReceiving();


            GlobalConfiguration.Configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseColouredConsoleLogProvider()
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSQLiteStorage("MoneyCounterDb.sqlite");
            DataBaseBackuper backuper = new DataBaseBackuper();
            backuper.LaunchSheduler();
            BackgroundJob.Enqueue(() => Console.WriteLine("Hannngfire!!"));
            using (var server = new BackgroundJobServer())
            {
                Console.ReadLine();
            }


            Console.WriteLine("Enter any key to exit");
            Console.Read();
            botClient.StopReceiving();
        }
    }
}