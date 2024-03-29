﻿namespace OpenNet.Orm.Entity.Constraints
{
    public enum KeyScheme
    {
        /// <summary>
        /// Entity has no primary key (not recommended)
        /// </summary>
        None,
        /// <summary>
        /// Entity has an auto-incrementing Primary Key
        /// </summary>
        Identity,
        /// <summary>
        /// Entity has a string GUID Primary Key
        /// </summary>
        GUID
    }
}
