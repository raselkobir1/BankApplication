﻿using Bank.Entity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Interface
{
    public interface IAuthinticationService
    {
        IEnumerable<ApplicationUser> GetApplicationUsers(bool trackChanges);
    }
}