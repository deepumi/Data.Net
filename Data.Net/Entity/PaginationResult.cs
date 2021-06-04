using System.Collections.Generic;

namespace Data.Net
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PaginationResult<TEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="paginationInfo"></param>
        public PaginationResult(List<TEntity> result, PaginationInfo paginationInfo)
        {
            Entities = result;
            PaginationInfo = paginationInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<TEntity> Entities { get; }

        /// <summary>
        /// 
        /// </summary>
        public PaginationInfo PaginationInfo { get; }
    }
}