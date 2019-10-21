namespace OpenNet.Orm
{
    public static class OrmDebug
    {
        public static void Trace(string text)
        {
            // Debug.WriteLine(text);
        }

        public static void Info(string text)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(text);
#endif
        }
    }
}
