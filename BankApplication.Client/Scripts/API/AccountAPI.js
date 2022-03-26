import Axios from "axios";

const ACCOUNT_API_ROOT = SITE_API_ROOT + "api/Account";


export default {
signin(email, password) {
    return new Promise((resolve, reject) => {
      Axios.post(ACCOUNT_API_ROOT + "/signin"+ `?email=${email}&password=${password}`)
        .then((response) => resolve(response))
        .catch((error) => reject(error.response.data));
    });
  }
}