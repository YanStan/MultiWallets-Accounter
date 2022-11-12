using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MoneyCounter.Models
{
    public class UserMessage
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MessageId { get; set; }
        public bool IsMainMenu { get; set; }
    }
}
