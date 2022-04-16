export default class Pagination {
    pageSize = 10;
    pageNo = 1;
    itemsTotal = 0;
    itemsFrom = 0;
    itemsTo = 0;
    pagesTotal = 1;
    pages = new Array();
  
    constructor(value) {
      Object.assign(this, value);
      if (this.itemsTotal > 0) {
        this.itemsFrom = (this.pageNo - 1) * this.pageSize + 1;
        this.itemsTo = Math.min(
          this.itemsFrom + this.pageSize - 1,
          this.itemsTotal
        );
  
        this.pagesTotal = Math.ceil(this.itemsTotal / this.pageSize);
        for (let page = 1; page <= this.pagesTotal; page++) {
          this.pages.push(page);
        }
      }
    }
  }
  