namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to not equal condition
    /// </summary>
    public class NotEqualCondition : Condition
    {
        private const string NotEqual = " <> ";
        private const string IsNot = " IS NOT ";

        public NotEqualCondition(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, NotEqual) { }

        public NotEqualCondition(IFilterFactory filterFactory, IFilter leftPart, ObjectValue rightPart)
            : base(filterFactory, leftPart, rightPart, rightPart.IsNull ? IsNot : NotEqual) { }
    }
}