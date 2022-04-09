using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Entity.Core
{
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BranchName { get; set; }
        public string Location { get; set; }
    }
}
