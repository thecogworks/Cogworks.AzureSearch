namespace Cogworks.AzureSearch.Options
{
    public class IndexOption
    {
        public bool Recreate { get; }

        public bool RecreateOnUpdateFailure { get; }

        public IndexOption(bool recreate, bool recreateOnUpdateFailure)
        {
            Recreate = recreate;
            RecreateOnUpdateFailure = recreateOnUpdateFailure;
        }
    }
}