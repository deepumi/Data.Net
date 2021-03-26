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
        /// <param name="autoIncrementValue"></param>
        void Retrieve(object autoIncrementValue);
    }
}