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
    public class UserInvitationRepository : RepositoryBase<UserInvitation>, IUserInvitationRepsitory
    {
        public UserInvitationRepository(DatabaseContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateInvitation(UserInvitation userInvitation)
        {
            Create(userInvitation);
        }
    }
}
