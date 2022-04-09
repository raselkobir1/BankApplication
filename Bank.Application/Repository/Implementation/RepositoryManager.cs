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
        private readonly Lazy<IBankTransactionRepository> _transactionRepository;      

        public RepositoryManager(DatabaseContext repositoryContext) 
        {
            _repositoryContext = repositoryContext;
            _bankAccountRepository = new Lazy<IBankAccRepository>(() => new BankAccRepository(_repositoryContext));
            _transactionRepository = new Lazy<IBankTransactionRepository>(() => new BankTransactionRepository(_repositoryContext));
        }

        public IBankAccRepository BankAccount => _bankAccountRepository.Value;

        public IBankTransactionRepository BankTransaction => _transactionRepository.Value;


        public void SaveChange()
        {
            _repositoryContext.SaveChanges();
        }
    }
}
