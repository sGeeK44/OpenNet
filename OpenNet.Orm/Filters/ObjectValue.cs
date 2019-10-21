using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using OpenNet.Orm.Queries;

// ReSharper disable ConvertPropertyToExpressionBody

namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to manage object value part filter
    /// </summary>
    public class ObjectValue : IFilter
    {
        private const string Null = "NULL";
        private readonly ISqlFactory _sqlFactory;
        private readonly object _value;

        public ObjectValue(ISqlFactory sqlFactory, object value)
        {
            _sqlFactory = sqlFactory;
            _value = value;
        }

        public bool IsNull { get { return _value == null || _value == DBNull.Value; } }

        public string ToStatement(List<IDataParameter> @params)
        {
            if (@params == null)
                @params = new List<IDataParameter>();

            if (IsNull)
                return Null;

            var asCollection = _value as ICollection;
            if (asCollection == null)
                return FormatParam(_value, @params);

            string result = null;
            foreach (var param in asCollection)
            {
                if (result == null)
                    result = FormatParam(param, @params);
                else 
                    result += ", " + FormatParam(param, @params);
            }
            return result;
        }

        private string FormatParam(object paramToAdd, ICollection<IDataParameter> @params)
        {
            return _sqlFactory.AddParam(paramToAdd, @params);
        }
    }
}