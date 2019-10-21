namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to AND condition
    /// </summary>
    public class And : Condition
    {
        private const string AndValue = " AND ";

        public And(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, AndValue) { }
    }
}