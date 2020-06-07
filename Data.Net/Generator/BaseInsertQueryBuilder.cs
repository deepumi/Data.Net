using System;

namespace Data.Net.Generator
{
    internal abstract class BaseInsertQueryBuilder
    {
        protected readonly EntityMetaData MetaData;

        protected virtual string PrameterDelimiter => "@"; 
            
        protected internal BaseInsertQueryBuilder(EntityMetaData metaData) => MetaData = metaData;

        protected internal abstract InsertSqlResult BuildInsertQuery();
        
        protected DataParameters CreateDataParameters()
        {
            var dp = new DataParameters(MetaData.PropertiesList.Count);

            for (var i = 0; i < MetaData.PropertiesList.Count; i++)
            {
                if (MetaData.PropertiesList[i].Name == null || IsAutoIncrement(MetaData.PropertiesList[i].Name)) continue;

                dp.Add(PrameterDelimiter + MetaData.PropertiesList[i].Name, MetaData.PropertiesList[i].Value);
            }

            return dp;
        }
        
        protected bool IsAutoIncrement(string key) => MetaData.AutoIncrementInfo != null &&
                                                      key.Equals(MetaData.AutoIncrementInfo.ColumName, StringComparison.OrdinalIgnoreCase);

    }
}