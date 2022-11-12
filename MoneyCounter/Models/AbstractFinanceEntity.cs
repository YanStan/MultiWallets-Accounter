using System;

namespace MoneyCounter.Models
{
    abstract public class AbstractFinanceEntity
    {
        public abstract int Id { get; set; }
        public abstract string Category { get; set; }
        public abstract string Subcategory { get; set; }
        public abstract string MoneyAmount { get; set; }
        public abstract DateTime DatetimeOfFinish { get; set; }
        public abstract string FirstName { get; set; }
        public abstract string Username { get; set; }
        public abstract int UserId { get; set; }
        public abstract string SumOnSourceWallet { get; set; }
        public abstract string SumOnAimWallet { get; set; }
        public abstract string FromWallet { get; set; }
        public abstract string ToWallet { get; set; }
    }
}
