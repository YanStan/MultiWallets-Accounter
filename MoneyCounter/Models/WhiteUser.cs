using System.ComponentModel.DataAnnotations;

namespace MoneyCounter.Models
{
    public class WhiteUser
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
    }
}
