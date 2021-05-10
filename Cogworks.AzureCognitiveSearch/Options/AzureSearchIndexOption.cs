namespace Cogworks.AzureCognitiveSearch.Options
{
    public class AzureSearchIndexOption
    {
        public bool Recreate { get; }

        public bool RecreateOnUpdateFailure { get; }

        public AzureSearchIndexOption(bool recreate, bool recreateOnUpdateFailure)
        {
            Recreate = recreate;
            RecreateOnUpdateFailure = recreateOnUpdateFailure;
        }
    }
}