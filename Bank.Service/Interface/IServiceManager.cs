using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Interface
{
    public interface IServiceManager
    {
        IBankAccountService BankAccountService { get; }
        IAuthinticationService AuthinticationService { get; }   
    }
}
