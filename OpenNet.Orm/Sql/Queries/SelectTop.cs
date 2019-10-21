// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable UseStringInterpolation
namespace OpenNet.Orm.Sql.Queries
{
    public class SelectTop<TIEntity> : Select<TIEntity>
        where TIEntity : class
    {
        private readonly int _quantity;

        public SelectTop(Selectable<TIEntity> selectable, int quantity)
            : base(selectable)
        {
            _quantity = quantity;
        }

        protected override string SelectVerb
        {
            get { return string.Format("{0}TOP({1}) ", base.SelectVerb, _quantity); }
        }
    }
}