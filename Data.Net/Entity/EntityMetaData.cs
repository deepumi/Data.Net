using System;
using System.Collections.Generic;
using System.Reflection;

namespace Data.Net
{
    internal sealed class EntityMetaData
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;

        private readonly object _entity;

        internal string TableName { get; }

        internal AutoIncrementInfo AutoIncrementInfo { get; private set; }

        internal List<PropertyPair> PropertiesList { get; private set; }

        internal KeyInfo KeyInfo { get; private set; }
        
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
            var properties = type.GetProperties(Flags);

            PropertiesList = new List<PropertyPair>(properties.Length);

            for (var i = 0; i < properties.Length; i++)
            {
                var columnProperty = properties[i].GetCustomAttribute<ColumnAttribute>(false);

                if (properties[i].GetSetMethod() == null || columnProperty != null && columnProperty.IgnoreColumn) continue;

                var columnName = columnProperty?.Name ?? properties[i].Name;

                PropertiesList.Add(new PropertyPair(columnName, properties[i].GetValue(_entity, null)));

                KeyInfo ??= CreateKeyInfo(properties[i], columnName);
                
                if(AutoIncrementInfo != null) continue;

                var autoIncrementInfo = CreateAutoIncrementInfo(properties[i], columnName);

                if (autoIncrementInfo != null) AutoIncrementInfo = autoIncrementInfo;
            }
        }

        private static KeyInfo CreateKeyInfo(PropertyInfo property, string columnName)
        {
            var keyInfo = property.GetCustomAttribute<KeyAttribute>(false);

            if (keyInfo != null)
            {
                return new KeyInfo(columnName, property.PropertyType);
            }

            return null;
        }

        private AutoIncrementInfo CreateAutoIncrementInfo(PropertyInfo property, string columnName)
        {
            var autoIncrement = property.GetCustomAttribute<AutoIncrementAttribute>(false);

            var sequence = autoIncrement as OracleSequenceAttribute;

            if (autoIncrement == null) return null;

            Action<object> autoIncrementSetter = default;

            IAutoIncrementRetriever retrieve = default;

            if (_entity is IAutoIncrementRetriever retriever)
            {
                retrieve = retriever;
            }
            else
            {
                autoIncrementSetter = b => property.SetValue(_entity, b);
            }

            return new AutoIncrementInfo(columnName, sequence?.SequenceName, retrieve, autoIncrementSetter,
                property.PropertyType);
        }
    }
}