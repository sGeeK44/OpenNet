namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to manage less than condition
    /// </summary>
    public class LessThanCondition : Condition
    {
        private const string LessThan = " < ";

        public LessThanCondition(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, LessThan) { }
    }
}