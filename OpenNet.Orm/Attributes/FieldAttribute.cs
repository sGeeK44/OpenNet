using System;
using OpenNet.Orm.Constants;

namespace OpenNet.Orm.Attributes
{
    public class FieldAttribute : EntityFieldAttribute, IDistinctable
    {
        public FieldAttribute()
        {
            // set up defaults
            AllowsNulls = true;
            SearchOrder = FieldSearchOrder.NotSearchable;
            Indexes = new string[0];
        }

        /// <summary>
        /// A unique string key to identify an object in collection
        /// </summary>
        public string Key { get { return FieldName; } }

        /// <summary>
        /// Field name in datastore
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Max lenght in db for char field
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Precision for floating number
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Scale for floating number
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// Add not Null constraint on field in database if value equal false.
        /// Default value is true.
        /// </summary>
        public bool AllowsNulls { get; set; }

        /// <summary>
        /// Add Primary key constraint on field in database if value equal true.
        /// Default value is false.
        /// </summary>
        public bool IsPrimaryKey { get; protected set; }

        /// <summary>
        /// Add Foreign key constraint on field in database if value equal true.
        /// Default value is false.
        /// </summary>
        public bool IsForeignKey { get; protected set; }

        /// <summary>
        /// Add unique constraint on field in database if value equal true.
        /// Default value is false.
        /// </summary>
        public bool RequireUniqueValue { get; set; }

        /// <summary>
        /// Create an indexe on field if value is different than FieldSearchOrder.NotSearchable.
        /// Default value is FieldSearchOrder.NotSearchable
        /// </summary>
        public FieldSearchOrder SearchOrder { get; set; }

        /// <summary>
        /// Add default value for null value if specified.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// rowversion or timestamp time for Sql Server
        /// </summary>
        public bool IsRowVersion { get; set; }

        /// <summary>
        /// Use to specific data type for ISqlCustomField
        /// </summary>
        public Type SpecificDataType { get; set; }

        /// <summary>
        /// Indicate if field should be used by sync framework to detect inserted row (False by default)
        /// </summary>
        public bool IsCreationTracking { get; set; }

        /// <summary>
        /// Indicate if field should be used by sync framework to detect updated row (False by default)
        /// </summary>
        public bool IsUpdateTracking { get; set; }

        /// <summary>
        /// Indicate if field should be used by sync framework to detect row in tombstone table (False by default)
        /// </summary>
        public bool IsDeletionTracking { get; set; }

        /// <summary>
        /// Indicate if field should be used by sync framework to detect updated row in lastsync (False by default)
        /// </summary>
        public bool IsLastSyncTracking { get; set; }

        /// <summary>
        /// Get or set indexes name that property is involve
        /// </summary>
        public string[] Indexes { get; set; }

        /// <summary>
        /// Return System.String that represent actual object
        /// </summary>
        /// <returns>System.String that represent actual object</returns>
        public override string ToString()
        {
            return FieldName;
        }
    }
}
