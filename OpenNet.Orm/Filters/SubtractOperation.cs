namespace OpenNet.Orm.Filters
{
    /// <summary>
    /// Encapsulate behaviour to manage subtract operation
    /// </summary>
    public class SubtractOperation : Condition
    {
        private const string Subtract = " - ";

        public SubtractOperation(IFilterFactory filterFactory, IFilter leftPart, IFilter rightPart)
            : base(filterFactory, leftPart, rightPart, Subtract) { }
    }
}