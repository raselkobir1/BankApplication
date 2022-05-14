using Bank.Entity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Repository.Interfaces
{
    public interface IPromoCodeRepository
    {
        void CreatePromocode(List<PromoCode> promoCodes);
        IEnumerable<PromoCode> GetPromocodes(bool trackchanges);
    }
}
