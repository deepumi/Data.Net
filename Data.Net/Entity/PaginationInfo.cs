using System;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PaginationInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int TotalRecords { get; }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalRecords"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        public PaginationInfo(int totalRecords, int currentPage, int pageSize)
        {
            TotalRecords = totalRecords;
            CurrentPage = currentPage <= 0 ? 1 : currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
        }
    }
}