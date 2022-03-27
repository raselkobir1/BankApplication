import Axios from "axios";

const ACCOUNT_API_ROOT = SITE_API_ROOT + "api/Account";


export default {
signin(email, password) {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/signin"+ `?email=${email}&password=${password}`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  signOut() {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/signout")
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  },
  getAccounts(){
    return new Promise((resolve, reject) => {
      Axios.get(ACCOUNT_API_ROOT + "/get-accounts")
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
}