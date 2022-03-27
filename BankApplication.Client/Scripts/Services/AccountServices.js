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
}