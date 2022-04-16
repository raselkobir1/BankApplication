<template>
<body>
<div class="page page-admin container-fluid">
  <div class="row">
    <nav class="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse">
      <div class="position-sticky pt-3">
        <ul class="nav flex-column">
          <li class="nav-item">
            <a class="nav-link active" aria-current="page" href="#">
              <span data-feather="home"></span>
              Admin Dashboard
            </a>
          </li>
        </ul>
      </div>
    </nav>

    <main class="col-md-9 ms-sm-auto col-lg-10 px-md-4">
      <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <h1 class="h2">Dashboard</h1>
        <div class="btn-toolbar mb-2 mb-md-0">
          <div class="btn-group me-2">
            <button type="button" @click="OnClickSignOut" class="btn btn-sm btn-outline-secondary">Sign out</button>
          </div>
        </div>
      </div>

      <div class="d-flex bd-highlight">
        <div class="p-2 flex-grow-1 bd-highlight">Customers account list</div>
        <div class="p-2 bd-highlight">
               <Dropdown 
                :optionValues="searchItems" 
                v-model="selectedItem"
            ></Dropdown>
        </div>
        <div class="p-2 bd-highlight">
               <div class="search-filter">
            <div class="input-group">
              <input
                type="search"
                v-model="searchValue"
                class="form-control"
                placeholder="Search"
                aria-describedby="basic-addon2"
              />
              <span class="input-group-text" id="basic-addon2">
                <a @click.prevent="getCustomerAccounts()" href="#">
                  <i class="fa fa-search">Search</i>
                </a>
              </span>
            </div>
        </div>
        </div>
    </div>

      <div class="table-responsive">
        <table class="table table-striped table-sm">
          <thead>
            <tr>
              <th scope="col">SL</th>
              <th scope="col">User name</th>
              <th scope="col">Account No</th>
              <th scope="col">Balance</th>
              <th scope="col">A/C Type</th>
              <th scope="col">Status</th>
              <th scope="col">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(account, index) of accountList" :key="index">
              <td>{{index+1}}</td>
              <td>{{account.userName}}</td>
              <td>{{account.accountNo}}</td>
              <td>{{account.openingBalance}}</td>
              <td>{{account.accountType}}</td>
              <td>{{account.accountStatus == true ? "Active":"Inactive"}}</td>
              <td>
                <button type="button" @click.prevent="OnClickActiveAccount(account.id)" class="btn btn-sm btn-primary">A/C Active</button>
                <button type="button" @click.prevent="OnClickInActiveAccount(account.id)" class="btn btn-sm btn-primary">A/C Inactive</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
       <Pagination
        v-if="vmPaginationModel.itemsTotal > 10"
        label="Bank Account"
        :value="vmPaginationModel"
        @pageChanged="onPageChanged"
        @pageSizeChanged="onPageSizeChanged"
      >
      </Pagination>
    </main>
  </div>
</div>
</body>
</template>

<script>
import Dropdown from "@scripts/Components/Dropdown";
import Pagination from "@scripts/Components/Pagination";
import PaginationModel from "@scripts/Models/Pagination";
import AccountService from "@scripts/Services/AccountServices";

export default {
  components: { Pagination,Dropdown },
  data() {
    return {
      accountList: [],
      context: '',
       vmPaginationModel: new PaginationModel(),
      pageNo: 1,
      pageSize: 10,
      searchValue:'',
      selectedItem:'',
      searchItems:[{text:'Account No',value:'AccountNo'},{text:'Active Account',value:'ActiveAccount'},{text:'InActive Account',value:'InActiveAccount'},{text:'User Name',value:'UserName'}]
    };
  },
  mounted(){
    this.getCustomerAccounts();
    this.getApplicationContext()
  },
  methods: {
    async getCustomerAccounts() {
            console.log("req data for get acc :", this.pageNo,this.pageSize, this.selectedItem, this.searchValue);
        AccountService.getAccounts(this.pageNo,this.pageSize, this.selectedItem, this.searchValue)
          .then((response) => {
            this.accountList = response.items;
            this.vmPaginationModel = response.pagination;
            console.log("Response data :", response);
          })
          .catch((error) => {
            console.log(error);
          })
          .finally(() => {
            
          });
    },
    onPageChanged() {
    this.pageNo = this.vmPaginationModel.pageNo;
    console.log("onPage change:", this.vmPaginationModel);
    this.getCustomerAccounts();
  },
  onPageSizeChanged(pageSize) {
    this.pageSize = this.vmPaginationModel.pageSize;
    console.log("onPageSize change:", this.vmPaginationModel);
    this.getCustomerAccounts();
  },

    getApplicationContext() {
      AccountService.getApplicationContext()
        .then((response) => {
          this.context = response.data;
          console.log("App context data :",this.context)
        })
        .catch((error) => {
          this.serverError = error;
        });
      },
    OnClickSignOut() {
      AccountService.singOut(this.loginModel)
        .then((response) => {
          this.$router.push({ name: "login"});
        })
        .catch((error) => {
          console.log(error);
        })
     },
    OnClickActiveAccount(id) {
      AccountService.accountActivationProcess(id)
        .then((response) => {
          console.log("id for activation account :",id);
          this.getCustomerAccounts();
        })
        .catch((error) => {
          console.log(error);
        })
    },
    OnClickInActiveAccount(id) {
      AccountService.accountInActivationProcess(id)
        .then((response) => {
          console.log("id for Inactivation account :",id);
          this.getCustomerAccounts();
        })
        .catch((error) => {
          console.log(error);
        })
    }
     
  },
};
</script>