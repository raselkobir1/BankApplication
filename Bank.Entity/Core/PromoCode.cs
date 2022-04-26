using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Entity.Core
{
    public class PromoCode
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public int Months { get; set; }
        public bool IsMultiple { get; set; }
        public DateTime? ExpiryOn { get; set; }
        public bool IsUsed { get; set; }
        public long? UsedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
