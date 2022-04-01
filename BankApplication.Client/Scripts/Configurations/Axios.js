import Axios from "axios";
import LocalStorageService from "@scripts/Services/LocalStorageService";

var authToken = LocalStorageService.getAuthenticationToken();
if (authToken) {
  console.log("adding token");
  Axios.defaults.headers.common["Authorization"] = "Bearer " + authToken;
}
