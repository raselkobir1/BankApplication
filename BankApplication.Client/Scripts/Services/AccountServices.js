import AccountAPI from "@scripts/API/AccountAPI";
export default {
    singin(email, password) {
        return AccountAPI.signin(email, password);
      },
}