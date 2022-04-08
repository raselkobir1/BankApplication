import AccountAPI from "@scripts/API/AccountAPI";
export default {
    singin(email, password) {
        return AccountAPI.signin(email, password);
      },
      getAccounts(){
        return AccountAPI.getAccounts();
      },
      getApplicationContext() {
        return AccountAPI.getApplicationContext();
      },
      singOut() {
        return AccountAPI.signOut();
      },
      CustomerRegistration(email, password) {
        return AccountAPI.CustomerRegistration(email, password);
      },
      customerApplayForAccount(accApplayModel) {
        return AccountAPI.customerApplayForAccount(accApplayModel);
      },
      accountActivationProcess(accountid){
        return AccountAPI.accountActivationProcess(accountid);
      },
      accountInActivationProcess(accountid){
        return AccountAPI.accountInActivationProcess(accountid);
      },
      getActiveAccountList(){
        return AccountAPI.getActiveAccountList();
      },
      
      transactionAmount(transactionModel){
        return AccountAPI.transactionAmount(transactionModel);
      },
      customerTransactionHistory(){
        return AccountAPI.customerTransactionHistory();
      },

      CustomerRegistrationFormSubmit(fd) {
        return AccountAPI.CustomerRegistrationFormSubmit(fd); 
      } ,
      forgotPassword(email) {
        return AccountAPI.forgotPassword(email);
      }
}


