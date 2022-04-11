using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using Bank.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Implementation
{
    internal sealed class AuthinticationService : IAuthinticationService
    {
        private readonly IRepositoryManager _repositoryManager;
        public AuthinticationService(IRepositoryManager repositoryManager) 
        {
            _repositoryManager = repositoryManager;
        }
        public IEnumerable<ApplicationUser> GetApplicationUsers(bool trackChanges)
        {
            return _repositoryManager.AuthinticationRepository.GetApplicationUsers(trackChanges);
        }
    }
}
