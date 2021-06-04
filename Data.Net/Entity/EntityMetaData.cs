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

        internal string KeyInfo { get; private set; }

        internal EntityMetaData(object entity)
        {
            var type = entity.GetType();

            _entity = entity;

            var tableInfo = type.GetCustomAttribute<TableNameAttribute>(false);

            TableName = string.IsNullOrEmpty(tableInfo?.TableName) ? type.Name : tableInfo.TableName;

            BindProperties(type);
        }

        internal bool IsAutoIncrement(string key) => AutoIncrementInfo != null &&
                                            key.Equals(AutoIncrementInfo.ColumnName,
                                                StringComparison.OrdinalIgnoreCase);
        
        private void BindProperties(IReflect type)
        {
            var properties = type.GetProperties(Flags);

            PropertiesList = new List<PropertyPair>(properties.Length);

            for (var i = 0; i < properties.Length; i++)
            {
                var columnProperty = properties[i].GetCustomAttribute<ColumnAttribute>(false);

                if (properties[i].GetSetMethod() == null ||
                    columnProperty != null && columnProperty.IgnoreColumn) continue;

                var columnName = columnProperty?.Name ?? properties[i].Name;

                PropertiesList.Add(new PropertyPair(columnName, properties[i].GetValue(_entity, null)));

                if (string.IsNullOrEmpty(KeyInfo) && properties[i].GetCustomAttribute<KeyAttribute>(false) != null)
                    KeyInfo = columnName;

                if (AutoIncrementInfo != null) continue;

                var autoIncrementInfo = CreateAutoIncrementInfo(properties[i], columnName);

                if (autoIncrementInfo != null) AutoIncrementInfo = autoIncrementInfo;
            }
        }

        private AutoIncrementInfo CreateAutoIncrementInfo(PropertyInfo property, string columnName)
        {
            var autoIncrement = property.GetCustomAttribute<AutoIncrementAttribute>(false);

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

            return new AutoIncrementInfo(columnName, autoIncrement?.SequenceName, retrieve, autoIncrementSetter,
                property.PropertyType);
        }
    }
}