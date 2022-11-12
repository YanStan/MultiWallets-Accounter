using System;
using System.ComponentModel.DataAnnotations;

namespace MoneyCounter.Models
{
    public class FoundersTransaction : AbstractFinanceEntity
    {
        [Key]
        public override int Id { get; set; }
        public override string Category { get; set; }
        public override string Subcategory { get; set; }
        public override string MoneyAmount { get; set; }
        public override DateTime DatetimeOfFinish { get; set; }
        public override string FirstName { get; set; }
        public override string Username { get; set; }
        public override int UserId { get; set; }
        public override string FromWallet { get; set; }
        public override string ToWallet { get; set; }
        public override string SumOnSourceWallet { get; set; }
        public override string SumOnAimWallet { get; set; }
    }
}
