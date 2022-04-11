using Bank.Utilities.EmailConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Utilities.EmailConfig
{
    public interface IEmailSender
    {
        void SendEmail(Message message); 
    }
}
