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
        public PaginationResult(IEnumerable<TEntity> result, PaginationInfo paginationInfo)
        {
            Entities = result;
            PaginationInfo = paginationInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TEntity> Entities { get; }

        /// <summary>
        /// 
        /// </summary>
        public PaginationInfo PaginationInfo { get; }
    }
}