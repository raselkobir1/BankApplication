using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.ContractModels.ResponseModel
{
    public class InvalidVoucherExcelModel
    {

        public string Code { get; set; }
        public string Months { get; set; }
        public string IsMultiple { get; set; }
        public string ExpiryOn { get; set; }
        public string IsUsed { get; set; }
        public string UsedBy { get; set; }
    }
}
