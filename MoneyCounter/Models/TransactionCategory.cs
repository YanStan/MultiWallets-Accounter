using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MoneyCounter.Models
{
    public class TransactionCategory : FinanceEntityCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override string CategoryKey { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override string SubCategoryKey { get; set; }
    }
}

