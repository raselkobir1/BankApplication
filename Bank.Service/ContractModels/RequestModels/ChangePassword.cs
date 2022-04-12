using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.ContractModels.RequestModels
{
    public class ChangePassword
    {
        [Required, DataType(DataType.Password)] 
        public string CurrentPassword { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } 

        [Required, DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage ="Password don't match")]
        public string ConfirmNewPassword { get; set; }  
    }
}
