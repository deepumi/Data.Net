namespace Data.Net
{ 
    /// <summary>
    /// 
    /// </summary>
    public interface IAutoIncrementRetriever
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoIcrementValue"></param>
        void Retrieve(object autoIcrementValue);
    }
}