using System;
// ReSharper disable MergeConditionalExpression

namespace OpenNet.Orm.Sync.Changes
{
    public class FieldValue
    {
        private bool _alreadyComputed;
        private object _value;

        public TypeCode Type { get; set; }

        public object Value
        {
            get
            {
                if (_alreadyComputed || Type == TypeCode.Empty)
                    return _value;

                _alreadyComputed = true;

                if (_value == null)
                    return null;

                _value = Convert.ChangeType(_value, Type, null);
                return _value;
            }
            set { _value = value; }
        }

        public bool IsIncludeInSyncSession(ISyncSessionInfo syncSession)
        {
            if (Value == null
                || Value == DBNull.Value)
                return false;

            var lastUpdate = (DateTime)Value;
            return lastUpdate > syncSession.LowBoundaryAnchor && lastUpdate <= syncSession.HighBoundaryAnchor;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FieldValue);
        }

        protected bool Equals(FieldValue other)
        {
            return other != null && Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public static FieldValue Create(object value)
        {
            var result = new FieldValue();
            var contertible = value as IConvertible;
            result.Type = contertible != null && !(value is DBNull) ? contertible.GetTypeCode() : TypeCode.Empty;
            result._value = value;
            return result;
        }
    }
}