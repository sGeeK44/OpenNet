namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to manage greather than or equal condition
    /// </summary>
    public class GreaterThanOrEqualCondition : Condition
    {
        private const string GreaterThanOrEqual = " >= ";

        public GreaterThanOrEqualCondition(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, GreaterThanOrEqual) { }
    }
}