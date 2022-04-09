using Bank.Application.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Repository.Implementation
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly DatabaseContext _repositoryContext;
        private readonly Lazy<IBankAccRepository> _bankAccountRepository;  

        public RepositoryManager(DatabaseContext repositoryContext) 
        {
            _repositoryContext = repositoryContext;
            _bankAccountRepository = new Lazy<IBankAccRepository>(() => new BankAccRepository(_repositoryContext));
        }

        public IBankAccRepository BankAccount => _bankAccountRepository.Value;

        public void SaveChange()
        {
            _repositoryContext.SaveChanges();
        }
    }
}
