using Bank.Service.ContractModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Interface
{
    public interface IBankAccountService
    {
        BankAccountDto CreateBankAccount(BankAccountDto bankAccountDto);
    }
}
