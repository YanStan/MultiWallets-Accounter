using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace MoneyCounter.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ChatStatus { get; set; }
        public bool IsAdmin { get; set; }
        //public bool WasIteratedAboutWrongWallet { get; set; }
    }
}
