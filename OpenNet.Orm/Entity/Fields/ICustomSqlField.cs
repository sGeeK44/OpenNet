namespace OpenNet.Orm.Entity.Fields
{
    /// <summary>
    /// Use to extend default behavior for sql type conversion
    /// </summary>
    public interface ICustomSqlField
    {
        /// <summary>
        /// Get maximal number used to represent current custom field. Default should be zero
        /// </summary>
        byte Precision { get; }
        
        /// <summary>
        /// Get decimal number. Default should be zero
        /// </summary>
        byte Scale { get; }

        /// <summary>
        /// Convert current object to sql field value (should be in according with db type field)
        /// </summary>
        /// <returns>Converted value for db</returns>
        object ToSqlValue();

        /// <summary>
        /// Initialize current object from specified sql field value
        /// </summary>
        /// <param name="value">Value getting from db</param>
        void FromSqlValue(object value);
    }
}