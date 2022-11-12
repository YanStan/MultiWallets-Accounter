using System.ComponentModel.DataAnnotations;

namespace MoneyCounter.Models
{
    public class TransactionIDForDeletion
    {
        [Key]
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public int OperatorThatRequestsId { get; set; }
        public string OperatorThatRequests { get; set; }
        public int? AdminThatAllowedId { get; set; }
        public string AdminThatAllowed { get; set; }
        public bool WasDeletionAllowed { get; set; }
        public bool IsActive { get; set; }

    }
}
