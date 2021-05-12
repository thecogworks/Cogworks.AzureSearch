using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Builder;
using Cogworks.AzureSearch.Indexes;
using Cogworks.AzureSearch.Initializers;
using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Operations;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.Searchers;
using Cogworks.AzureSearch.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cogworks.AzureSearch.Microsoft.IocExtension.Builders
{
    public class AzureSearchBuilder : IAzureSearchBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public AzureSearchBuilder(IServiceCollection serviceCollection)
            => _serviceCollection = serviceCollection;

        internal AzureSearchBuilder RegisterInitializers()
        {
            _serviceCollection.TryAddScoped(typeof(IInitializer<>), typeof(Initializer<>));

            return this;
        }

        public IAzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _serviceCollection.TryAddSingleton(_ => new AzureSearchIndexOption(recreate, recreateOnUpdateFailure));

            return this;
        }

        public IAzureSearchBuilder RegisterClientOptions(string serviceName, string credentials, string serviceEndpointUrl)
        {
            _serviceCollection.TryAddSingleton(_ => new AzureSearchClientOption(
                serviceName,
                credentials,
                serviceEndpointUrl));

            return this;
        }

        public IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IModel, new()
        {
            _serviceCollection.TryAddSingleton(_ => new IndexDefinition<TDocument>(indexName));

            return this;
        }

        public IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IModel, new()
        {
            _serviceCollection.TryAddSingleton(_ => new IndexDefinition<TDocument>(customIndex));

            return this;
        }

        internal AzureSearchBuilder RegisterIndexes()
        {
            _serviceCollection.TryAddScoped(typeof(IIndex<>), typeof(Index<>));

            return this;
        }

        internal AzureSearchBuilder RegisterWrappers()
        {
            _serviceCollection.TryAddScoped(typeof(IDocumentOperationWrapper<>), typeof(DocumentOperationWrapper<>));

            _serviceCollection.TryAddScoped<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal AzureSearchBuilder RegisterRepositories()
        {
            _serviceCollection.TryAddScoped(
                typeof(IRepository<>),
                typeof(Repository<>));

            return this;
        }

        internal AzureSearchBuilder RegisterSearchers()
        {
            _serviceCollection.TryAddScoped(typeof(IAzureSearch<>), typeof(AzureSearch<>));

            return this;
        }

        internal AzureSearchBuilder RegisterOperations()
        {
            _serviceCollection.TryAddScoped(typeof(IDocumentOperation<>), typeof(DocumentOperation<>));

            _serviceCollection.TryAddScoped(typeof(IIndexOperation<>), typeof(IndexOperation<>));

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _serviceCollection.TryAddSingleton<TSearcherType, TSearcher>();

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _serviceCollection.TryAddSingleton(instance);

            return this;
        }
    }
}