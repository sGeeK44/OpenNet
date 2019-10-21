namespace OpenNet.Orm.Sync.Agents
{
    public enum SyncStates
    {
        Idle,
        ComputeLocalChange,
        SyncInProgress,
        EstablishNewSession,
        DetermineTypeOfSync,
        GettingRemoteChange,
        ApplyingRemoteChange,
        ResolvingConflicts,
        SendingConflictsSolved,
        ClosingSession,
        InError,
        CleanEntites,
        ComputeRemoteChange,
        ComputeServerChange,
        SendingServerChange,
        ApplyRemoteResolution
    }
}