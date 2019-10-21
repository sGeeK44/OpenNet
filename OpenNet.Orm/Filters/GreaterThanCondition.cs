namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to manage greather than condition
    /// </summary>
    public class GreaterThanCondition : Condition
    {
        private const string GreaterThan = " > ";

        public GreaterThanCondition(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, GreaterThan) { }
    }
}