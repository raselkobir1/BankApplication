using Bank.Application.Repository.Implementation.Core;
using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;

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

        //public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        //{
        //    return FindAll(trackChanges).OrderBy(c => c.Name).ToList();
        //}

        //public Company GetCompany(Guid companyId, bool trackingChanges)
        //{
        //    return FindByCondition(c => c.Id.Equals(companyId), trackingChanges).SingleOrDefault();
        //}
    }
}
