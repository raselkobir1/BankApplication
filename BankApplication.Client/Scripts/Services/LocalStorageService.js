import Axios from "axios"; 
import cookies from "vue-cookies";

const KEY_AUTH_TOKEN = "KEY_AUTH_TOKEN";

export default {
  setAuthenticationToken(token) {
    cookies.set(KEY_AUTH_TOKEN, token);
    if (token) {
      Axios.defaults.headers.common["Authorization"] = "Bearer " + token;
    }
  },
  getAuthenticationToken() {
    return cookies.get(KEY_AUTH_TOKEN);
  },
  clearAuthenticationToken() {
    cookies.remove(KEY_AUTH_TOKEN);
    Axios.defaults.headers.common["Authorization"] = null;
  }
};