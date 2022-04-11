using Bank.Application.Repository.Implementation.Core;
using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Repository.Implementation
{
    public class BankTransactionRepository : RepositoryBase<Balance>, IBankTransactionRepository
    {
        public BankTransactionRepository(DatabaseContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateBankTransaction(Balance balanceTransaction) 
        {
            Create(balanceTransaction);
        }

        public IEnumerable<Balance> GetAllBalance(bool trackChanges)
        {
            return FindAll(trackChanges).ToList();
        }
    }
}
