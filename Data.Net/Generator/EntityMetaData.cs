using System;
using System.Collections.Generic;
using System.Reflection;

namespace Data.Net.Generator
{
    internal sealed class EntityMetaData
    {
        private static readonly BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;
        
        private readonly object _entity;

        internal string TableName { get; }

        internal AutoIncrementInfo AutoIncrementInfo { get; private set; }

        internal List<PropertyPair> PropertiesList { get; private set; }

        internal EntityMetaData(object entity)
        {
            var type = entity.GetType();

            _entity = entity;

            var tableInfo = type.GetCustomAttribute<TableNameAttribute>(false);

            TableName = string.IsNullOrEmpty(tableInfo?.TableName) ? type.Name : tableInfo.TableName;

            BindProperties(type);
        }

        private void BindProperties(IReflect type)
        {
            var properties = type.GetProperties(_flags);

            PropertiesList = new List<PropertyPair>(properties.Length);

            for (var i = 0; i < properties.Length; i++)
            {
                var columnProperty = properties[i].GetCustomAttribute<ColumnAttribute>(false);

                if (properties[i].GetSetMethod() == null || columnProperty != null && columnProperty.IgnoreColumn) continue;

                var columnName = columnProperty?.Name ?? properties[i].Name;

                PropertiesList.Add(new PropertyPair(columnName, properties[i].GetValue(_entity, null)));

                if(AutoIncrementInfo != null) continue;
                
                var autoIncrement = properties[i].GetCustomAttribute<AutoIncrementAttribute>(false);

                var sequence = autoIncrement as OracleSequenceAttribute;

                if (autoIncrement == null) continue;

                var index = i;

                Action<object> autoIncrementSetter = default;

                IAutoIncrementRetriever retrieve = default;

                if (_entity is IAutoIncrementRetriever retriever)
                {
                    retrieve = retriever;
                }
                else
                {
                    autoIncrementSetter = b => properties[index].SetValue(_entity, b);
                }

                AutoIncrementInfo = new AutoIncrementInfo(columnName, sequence?.SequenceName, retrieve, autoIncrementSetter,properties[i].PropertyType);
            }
        }
    }
}