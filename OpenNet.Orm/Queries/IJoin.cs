using System;

namespace OpenNet.Orm.Queries
{
    public interface IJoin : IClause
    {
        /// <summary>
        /// get type of first entity involve
        /// </summary>
        Type EntityType1 { get; }

        /// <summary>
        /// get type of second entity involve
        /// </summary>
        Type EntityType2 { get; }
    }
}