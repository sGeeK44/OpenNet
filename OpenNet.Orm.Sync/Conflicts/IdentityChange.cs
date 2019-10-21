namespace OpenNet.Orm.Sync.Conflicts
{
    public class IdentityChange
    {
        public string EntityName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}