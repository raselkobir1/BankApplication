using Bank.Application.Repository.Implementation.Core;
using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using System.Collections.Generic;
using System.Linq;

namespace Bank.Application.Repository.Implementation
{
    public class BankAccRepository : RepositoryBase<BankAccount>, IBankAccRepository
    {
        public BankAccRepository(DatabaseContext repositoryContext) : base(repositoryContext) 
        {
        }

        public void CreateBankAccount(BankAccount bankAccount)
        {
             Create(bankAccount);  
        }

        public IEnumerable<BankAccount> GetAllBankAccounts(bool trackChanges)
        {
            return FindAll(trackChanges).ToList();
        }
        public void UpdateBankAccount(BankAccount bankAccount)
        {
            Update(bankAccount);    
        }
        //public Company GetCompany(Guid companyId, bool trackingChanges)
        //{
        //    return FindByCondition(c => c.Id.Equals(companyId), trackingChanges).SingleOrDefault();
        //}
    }
}
