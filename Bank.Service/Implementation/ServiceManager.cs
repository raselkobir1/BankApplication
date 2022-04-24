﻿using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using Bank.Service.Interface;
using Bank.Utilities.EmailConfig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Implementation
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IBankAccountService> _bankAccountService; 
        private readonly Lazy<IAuthinticationService> _authinticationService;  

        public ServiceManager(IRepositoryManager repositoryManager, IEmailSender email, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) 
        {
            _bankAccountService = new Lazy<IBankAccountService>(() => new BankAccountService(repositoryManager, email, httpContextAccessor, userManager,signInManager)); 
            _authinticationService = new Lazy<IAuthinticationService>(() => new AuthinticationService(repositoryManager));
        }

        public IBankAccountService BankAccountService => _bankAccountService.Value;

        public IAuthinticationService AuthinticationService => _authinticationService.Value;
    }
}
