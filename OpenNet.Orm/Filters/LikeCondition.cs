namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to manage like condition
    /// </summary>
    public class LikeCondition : Condition
    {
        private const string Like = " LIKE ";

        public LikeCondition(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, Like) { }
    }
}