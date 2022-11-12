
using MoneyCounter.Models;
using MoneyCounter.Repositories;
using MoneyCounter.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace MoneyCounter.Inline
{
    class InlineProcessorFactory
    {
        private List<InlineProcessor> InlineProcessorsList { get; set; }
        public InlineProcessorFactory() // конструктор
        {
            InlineProcessorsList = new List<InlineProcessor>
            {
                new ChooseTransactionEntityTypeInlineProcessor(),
                new ChooseAnalysisTypeInlineProcessor(),
                new AdminPanelInlineProcessor(),
                new AdminPanelTransactionsInlineProcessor(),
                new AdminPanelAnalysisInlineProcessor(),
                new UserManagementInlineProcessor(),
            };
        }
        public InlineProcessor GetInlineProcessorInstanceFromButtonName(string callbackData)
        {
            var processorInstance = InlineProcessorsList.FirstOrDefault(x => callbackData.StartsWith(x.Name));
            return processorInstance;
        }
        public async Task<Messages> RunInlineProcessor(CallbackQuery c, TelegramBotClient botClient)
        {
            InlineProcessor processorInstance = GetInlineProcessorInstanceFromButtonName(c.Data);
            Message msg;
            Messages messages;
            if (processorInstance != null)
                messages = await processorInstance.Execute(c, botClient); //выполняем тело команды
            else
            {
                msg = await botClient.SendTextMessageAsync(c.Message.Chat.Id,
                    $"⚠️ Неизвестная команда: \"{c.Data}\". Возможно, команда была переименована или удалена.");
                var messagesList = new List<Message>() { msg };
                messages = new Messages(messagesList);
            }
            return messages;
        }
    }
}

