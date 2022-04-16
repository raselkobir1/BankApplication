import AccountAPI from "@scripts/API/AccountAPI";
export default {
    singin(email,isRemembeMe, password) {
        return AccountAPI.signin(email, isRemembeMe, password);
      },
      getAccounts(pageNo, pageSize, selectedItem, searchValue){
        return AccountAPI.getAccounts(pageNo, pageSize, selectedItem, searchValue);
      },
      getApplicationContext() {
        return AccountAPI.getApplicationContext();
      },
      singOut() {
        return AccountAPI.signOut();
      },
      CustomerRegistration(email, password, role) {
        return AccountAPI.CustomerRegistration(email, password, role);
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
      },
      resetPassword(model) {
        return AccountAPI.resetPassword(model);
      },
      changePassword(model) {
        return AccountAPI.changePassword(model);
      }
}


