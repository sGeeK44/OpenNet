namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to manage less than condition
    /// </summary>
    public class LessThanOrEqualCondition : Condition
    {
        private const string LessThanOrEqual = " <= ";

        public LessThanOrEqualCondition(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, LessThanOrEqual) { }
    }
}