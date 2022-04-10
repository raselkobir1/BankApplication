using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Repository.Interfaces
{
    public interface IRepositoryManager
    {
        IBankAccRepository BankAccount { get; }
        IBankTransactionRepository BankTransaction { get; }
        IAuthinticationRepository AuthinticationRepository { get; } 
        void SaveChange();
    }
}
