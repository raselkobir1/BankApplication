import Axios from "axios";
import LocalStorageService from "../Services/LocalStorageService";
import BankAccountMapper from "@scripts/Models/BankAccountMapper";

const ACCOUNT_API_ROOT = SITE_API_ROOT + "api/Account";
const Bank_API_ROOT = SITE_API_ROOT + "api/Bank"; 


export default {
  CustomerRegistration(email, password, role) {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/register"+ `?email=${email}&password=${password}&role=${role}`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
signin(email,isRemembeMe, password) {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/signin"+ `?email=${email}&isRemembeMe=${isRemembeMe}&password=${password}`)
        .then((response) => {
          console.log("login-response: ", response.data);
          LocalStorageService.setAuthenticationToken(response.data.token);
          resolve(response);
        })
        .catch((error) => reject(error.response.data));
    });
  },
  signOut() {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/signout")
        .then((response) => {
          LocalStorageService.clearAuthenticationToken();
          resolve(response);
        })
        .catch((error) => reject(error.response.data));
    });
  },
  forgotPassword(email) {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/forgot-password" + `?email=${email}`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  resetPassword(model) {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/reset-password", model)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  changePassword(model) {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/change-password", model)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  
  getApplicationContext() {
    return new Promise((resolve, reject) => {
      Axios.get(ACCOUNT_API_ROOT + `/app-context`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
//---------Bank Operation--------
  getAccounts(pageNo, pageSize, selectedItem, searchValue){
    return new Promise((resolve, reject) => {
      Axios.get(Bank_API_ROOT + "/get-accounts"+ `?pageNo=${pageNo}&pageSize=${pageSize}&searchValue=${searchValue}&selectedItem=${selectedItem}`)
      .then((response) => {
        console.log("api data page", response.data.accounts);
        resolve(BankAccountMapper.mapToClient(response.data.accounts)) 
      })
      .catch((error) => reject(error.response.data));
    });
  },
  customerApplayForAccount(accApplayModel) {
    return new Promise((resolve, reject) => {
      Axios.post(Bank_API_ROOT + "/bankaccount-create", accApplayModel)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  accountActivationProcess(accountid) {
    return new Promise((resolve, reject) => {
      Axios.put(Bank_API_ROOT + "/active-account" + `?accountid=${accountid}`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  accountInActivationProcess(accountid) {
    return new Promise((resolve, reject) => {
      Axios.put(Bank_API_ROOT + "/inactive-account" + `?accountid=${accountid}`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  getActiveAccountList() {
    return new Promise((resolve, reject) => {
      Axios.get(Bank_API_ROOT + `/active-acc-list`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  transactionAmount(transactionModel) {
    return new Promise((resolve, reject) => {
      Axios.post(Bank_API_ROOT + "/transaction", transactionModel)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  customerTransactionHistory() {
    return new Promise((resolve, reject) => {
      Axios.get(Bank_API_ROOT + `/transaction-history`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  CustomerRegistrationFormSubmit(fd) {
    return new Promise((resolve, reject) => {
      Axios.post(Bank_API_ROOT + "/register-form",fd)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },

  invitationSend(email,role){
    return new Promise((resolve, reject) => {
      Axios.post(Bank_API_ROOT + "/invite"+ `?email=${email}&userType=${role}`)
      .then((response) => {
        resolve()
      })
      .catch((error) => reject(error.response.data));
    });
  }, 
}



