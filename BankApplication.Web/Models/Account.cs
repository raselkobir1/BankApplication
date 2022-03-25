namespace BankApplication.Web.Models
{
    public class Account
    {
        public long Id { get; set; }
        public long BankId { get; set; }
        public long ApplicationUserId { get; set; }
        public string AccountNo { get; set; }
        public double OpeningBalance { get; set; }
        public double DepositeAmount { get; set; }
        public double WidthwrounAmount { get; set; }
        public bool AccountStatus { get; set; }
        public ApplicationUser  ApplicationUser { get; set; } 

    }
}

