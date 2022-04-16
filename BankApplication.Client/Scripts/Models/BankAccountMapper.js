import PaginationModel from "@scripts/Models/Pagination";
import List from "@scripts/Models/List";

export default {
  mapToClient(data) {
    if (!!data) {
      return new List({
        items: data.items,
        pagination: new PaginationModel({
          pageSize: data.pageSize,
          pageNo: data.pageNo,
          itemsTotal: data.totalItems,
        }),
      });
    } else {
      return new List({
        items: [],
        pagination: new PaginationModel(),
      });
    }
  },
};
