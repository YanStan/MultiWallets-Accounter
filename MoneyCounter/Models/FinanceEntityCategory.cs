using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyCounter.Models
{
    public abstract class FinanceEntityCategory
    {
        public abstract string CategoryKey { get; set; }
        public abstract string SubCategoryKey { get; set; }

    }
}
