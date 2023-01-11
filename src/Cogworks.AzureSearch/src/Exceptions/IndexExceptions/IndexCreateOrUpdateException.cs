using System;

namespace Cogworks.AzureSearch.Exceptions.IndexExceptions
{
    public class IndexCreateOrUpdateException :DomainException
    {
        public override string Code => "index_create_or_update_error";

        public IndexCreateOrUpdateException(string message) : base(message)
        {
        }

        public IndexCreateOrUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}