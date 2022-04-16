using System;
using System.Collections.Generic;

namespace Bank.Utilities.Pagination
{
    public class PaginatedData<T> where T: class
    {
        public PaginatedData(IEnumerable<T> items, int pageNo, int pageSize, int totalItems) 
        {
            Items = items;
            PageNo = pageNo;
            PageSize = pageSize;
            TotalItems = totalItems;
        }
        public IEnumerable<T> Items { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }  
    }
}
