namespace BankApplication.Web.ContractModels
{
    public class BankAccountDto
    {
        public long Id { get; set; }
        public long ApplicationUserId { get; set; }
        public double OpeningBalance { get; set; }
        public string AccountType { get; set; }
    }
}
