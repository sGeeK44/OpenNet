namespace OpenNet.Orm.Sync.Entity
{
    /// <summary>
    /// Defines the direction that data changes flow, from the perspective of the client.
    /// </summary>
    public enum SyncDirection
    {
        /// <summary>
        /// During synchronization, the client typically downloads data set from the server.
        /// </summary>
        DownloadOnly,

        /// <summary>
        /// During the synchronization, the client typically uploads changes to the server.
        /// </summary>
        UploadOnly,

        /// <summary>
        /// During the synchronization, the client typically download and uploads changes to the server.
        /// </summary>
        TwoWay
    }
}