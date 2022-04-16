import PaginationModel from "@scripts/Models/Pagination";

export default class List {
  items = [];
  pagination = new PaginationModel();

  constructor(value) {
    Object.assign(this, value);
  }
}