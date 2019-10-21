using System.Collections.Generic;
using System.Reflection;

namespace OpenNet.Orm.Interfaces
{
    public interface IEntity
    {
        /// <summary>
        /// Get property of current entity persist in db
        /// </summary>
        List<PropertyInfo> DbField { get; }

        /// <summary>
        /// Get property of current entity that hold relation to another entity
        /// </summary>
        List<PropertyInfo> DbReference { get; }
    }
}
