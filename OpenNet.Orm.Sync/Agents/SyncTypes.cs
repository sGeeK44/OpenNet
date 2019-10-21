namespace OpenNet.Orm.Sync.Agents
{
    public enum SyncTypes
    {
        /// <summary>
        /// Only update made on remote will be upload to desktop
        /// </summary>
        OneWay,

        /// <summary>
        /// Both side will be updated
        /// </summary>
        TwoWay
    }
}