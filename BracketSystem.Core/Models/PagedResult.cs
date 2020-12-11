using System;
using System.Collections.Generic;

namespace BracketSystem.Core.Models
{
    public class PagedResult<T>
    {
        #region Properties

        public IEnumerable<T> Items { get; private set; }
        public MetaData MetaData { get; private set; }

        #endregion


        public PagedResult(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData(count, pageNumber, pageSize);
            Items = items;
        }
    }
    
    public class MetaData
    {
        #region Properties

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        #endregion


        public MetaData(int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}