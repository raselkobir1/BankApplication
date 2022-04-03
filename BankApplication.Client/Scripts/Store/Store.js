import Vuex from "vuex";
import Vue from "vue";

Vue.use(Vuex);
Vue.config.devtools = true
const store = new Vuex.Store({
    state: {
      loggedinUser: null,
    },
    mutations: {
      setLoggedinUser(state, payload) {
        state.loggedinUser = payload;
      },
    },
    getters: {
      getLoggedinUser(state) {
        return state.loggedinUser;
      },
    },
  });
  
  export default {
    setLoggedinUser(user) {
      store.commit("setLoggedinUser", user); 
    },
    getLoggedinUser() {
      return store.getters.getLoggedinUser;
    },
  };

