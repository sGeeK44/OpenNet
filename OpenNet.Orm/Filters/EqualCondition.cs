namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to equal condition
    /// </summary>
    public class EqualCondition : Condition
    {
        private const string Equal = " = ";
        private const string Is = " IS ";

        public EqualCondition(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, Equal) { }

        public EqualCondition(IFilterFactory filterFactory, IFilter leftPart, ObjectValue rightPart)
            : base(filterFactory, leftPart, rightPart, rightPart.IsNull ? Is : Equal) { }
    }
}