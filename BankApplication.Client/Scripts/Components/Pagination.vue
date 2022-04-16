<template>
  <div class="section-pagination" v-if="vmPagination.itemsTotal >= 10">
    <nav aria-label="Page navigation example" class="pagination-navigation">
      <ul v-if="vmPagination.itemsTotal > 10" class="pagination pagination-area">
        <span class="page-size">View </span>
        <select v-model="vmPagination.pageSize" @change="onPageSizeChange">
          <option v-for="page in vmPageSizes" :key="page" :value="page">
            {{ page }}
          </option>
        </select>
        <span class="page-size"> {{ itemLabel }}</span>
        <li :class="['page-item', { disabled: vmPagination.pageNo <= 1 }]">
          <a class="page-link" @click="onPageNumberClicked(vmPagination.pageNo - 1)" href="#" aria-label="Previous" >
            <span aria-hidden="true">&laquo;</span>
          </a>
        </li>
        <li
          v-for="page in vmPagination.pages" :key="page" :class="['page-item', { active: page == vmPagination.pageNo }]">
          <a class="page-link" @click="onPageNumberClicked(page)">{{ page }}</a>
        </li>
        <li ref="" :class="['page-item',{ disabled: vmPagination.pageNo == vmPagination.pagesTotal,}]">
          <a class="page-link" @click="onPageNumberClicked(vmPagination.pageNo + 1)" href="#" aria-label="Next" >
            <span aria-hidden="true">&raquo;</span>
          </a>
        </li>
      </ul>
    </nav>
    <div class="paginate-text">
      Showing {{ vmPagination.itemsFrom }} to {{ vmPagination.itemsTo }} of
      {{ vmPagination.itemsTotal }} {{ itemLabel }}
    </div>
  </div>
</template>

<script>
export default {
  props: {
    label: {
      type: String,
      default: "items",
    },
    value: Object,
  },
  components: {},
  data() {
    return {
      vmPreviousPageText: "Previous",
      vmNextPageText: "Next",
      vmPageSizes: [10, 20, 30, 40, 50, 100],
      itemLabel: this.label,
      vmPagination: this.value,
    };
  },
  mounted() {
    console.log("pagination page:", this.vmPagination);
  },
  methods: {
    onPageNumberClicked(pageNo) {
      this.vmPagination.pageNo = pageNo;
      this.$emit("pageChanged", pageNo);
    },
    onPageSizeChange() {
      this.$emit("pageSizeChanged", this.vmPagination.pageSize);
    },
  },
  watch: {
    value: function(newValue) {
      this.vmPagination = newValue;
    },
  },
};
</script>
