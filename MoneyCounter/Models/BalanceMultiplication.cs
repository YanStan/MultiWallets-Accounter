
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyCounter.Models
{
    public class BalanceMultiplication
    {
        [Key]
        public int BMId { get; set; }
        [ForeignKey("Transaction")]
        public int TransactionId { get; set; }
        public string AdjunctionSum { get; set; }
        public Transaction Transaction { get; set; }
    }
}
