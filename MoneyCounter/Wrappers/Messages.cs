using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace MoneyCounter.Wrappers
{
    public class Messages
    {
        public IReadOnlyCollection<Message> readOnlyMessages { get; }
        public Messages(IReadOnlyCollection<Message> messages)
        {
            readOnlyMessages = messages;
        }

    }
}
