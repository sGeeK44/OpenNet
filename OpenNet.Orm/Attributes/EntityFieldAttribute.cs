using System;
using JetBrains.Annotations;

namespace OpenNet.Orm.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFieldAttribute : Attribute
    {
    }
}