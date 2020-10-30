# Cogworks.AzureSearch [![GitHub license](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE.md)

A wrapper to Azure Search allowing to easily setup Azure Search indexes, searchers and using it with DI/IoC approach (currently with support for Umbraco, LightInject and Autofac).

## Usage

#### Registration

```csharp

_composing.RegisterAzureSearch()
    .RegisterClientOptions("[AzureSearchServiceName]", "[AzureSearchCredentials]")
    .RegisterIndexOptions(false, false) // for now required
    .RegisterIndexDefinitions<FirstDocumentModel>("first-document-index-name")
    .RegisterIndexDefinitions<SecondDocumentModel>("second-document-index-name")
    .RegisterIndexDefinitions<ThirdDocumentModel>(new Index{
        Name = "third-document-index-name",
        ScoringProfiles = new List<ScoringProfile>()
                {
                    new ScoringProfile("global-sp", new TextWeights(new Dictionary<string, double>()
                    {
                        {"Name", 10}, {"Content", 0.1}, {"Tags/Name", 0.1}
                    }),new List<ScoringFunction>()
                    {
                        new FreshnessScoringFunction("PublishDate", 20, TimeSpan.FromDays(180), ScoringFunctionInterpolation.Quadratic)
                    })
                } //and more custom properties to add
    })
    .RegisterDomainSearcher<SomeDomainSearch, ISomeDomainSearch, FirstDocumentModel>();
```

#### Document Operations

```csharp
public class SomeDocumentService
{
    private readonly IAzureDocumentOperation<FirstDocumentModel> _documentOperation;

    public SomeDocumentService(IAzureDocumentOperation<FirstDocumentModel> documentOperation)
        => _documentOperation = documentOperation;

    public async Task AddOrUpdateItem()
        => await _documentOperation.AddOrUpdateDocumentAsync(new FirstDocumentModel
            {
                Id = "some-id",
                Content = "Some content"
            });

    
    public async Task AddOrUpdateItems()
    {
        var items = new List<FirstDocumentModel>
        {
            new FirstDocumentModel
            {
                Id = "first-id",
                Content = "First content"
            },
            new FirstDocumentModel
            {
                Id = "second-id",
                Content = "Second content"
            }
        };

        _ = await _documentOperation.AddOrUpdateDocumentsAsync(items);
    }


    public async Task RemoveItem()
        => await _documentOperation.TryRemoveDocumentAsync(new FirstDocumentModel
            {
                Id = "some-id"
            });

    
    public async Task RemoveItems()
    {
        var items = new List<FirstDocumentModel>
        {
            new FirstDocumentModel
            {
                Id = "first-id"
            },
            new FirstDocumentModel
            {
                Id = "second-id"
            }
        };

        _ = await _documentOperation.TryRemoveDocumentsAsync(items);
    }
}
```

#### Search operations
##### Default search for index

```csharp
public class SomeService
{
    private readonly IAzureSearch<FirstDocumentModel> _documentSearch;

    public SomeService(IAzureSearch<FirstDocumentModel> documentSearch)
        => _documentSearch = documentSearch;

    public void Search()
    {
        _ = _documentSearch.Search("Some text", new AzureSearchParameters
        {
            Top = 3
        });
    }

    public async Task SearchAsync()
    {
        _ = await _documentSearch.SearchAsync("Some text", new AzureSearchParameters
        {
            Top = 3
        });
    }
}
```

##### Domain search

``` csharp
public interface ISomeDomainSearch
{
    FirstDocumentModel GetLatestAuthorDocument(string author);
}

public class SomeDomainSearch : AzureSearch<FirstDocumentModel>, ISomeDomainSearch
{
    public SomeDomainSearch(IAzureDocumentSearch<FirstDocumentModel> azureSearchRepository) : base(azureSearchRepository)
    {
    }

    FirstDocumentModel GetLatestAuthorDocument(string author)
        => base.Search("*", new AzureSearchParameters
        {
            Filter = nameof(FirstDocumentModel.Author).EqualsValue(author)
            OrderBy = new List<string>() { nameof(FirstDocumentModel.PublishedDate) + "desc" }
            Top = 1
        }).Results.FirstOrDefault()?.Document;
}
```

#### Index Operations

```csharp
public class SomeIndexService
{
    private readonly IAzureIndexOperation<FirstDocumentModel> _indexOperation;
    public SomeIndexService(IAzureIndexOperation<FirstDocumentModel> indexOperation)
        => _indexOperation = indexOperation;

    public async Task Work()
    {
        _ = await _indexOperation.IndexExistsAsync();
        _ = await _azureIndexOperation.IndexCreateOrUpdateAsync();
        _ = await _azureIndexOperation.IndexClearAsync();
        await _indexOperation.IndexDeleteAsync()
    }
}
```

#### Repository Operations

All operations (API) that are available for: Document Operations, Search Operations, Index Operations.

```csharp
public class SomeAdvanceService
{
    private readonly IAzureSearchRepository<FirstDocumentModel> _documentRepository;
    public SomeAdvanceService(IAzureSearchRepository<FirstDocumentModel> documentRepository)
        => _documentRepository = documentRepository;

    public async Task Work()
    {
        // some work here
    }
}
```

#### Initializer

```csharp
public class SomeStartupService
{
    private readonly IAzureInitializer<FirstDocumentModel> _initializer;

    public SomeStartupService(IAzureInitializer<FirstDocumentModel> initializer)
        => _initializer = initializer;

    public async Task Initialize()
        => await initializer.InitializeAsync();
}
```

## License
  
- Cogworks.AzureSearch is licensed under the [Apache License, Version 2.0](https://opensource.org/licenses/Apache-2.0)

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/) to clarify expected behavior in our community.
For more information, see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## Blogs

* [How to Simplify Azure Search Implementations](https://www.wearecogworks.com/blog/how-to-simplify-azure-search-implementations/)

## How can you help?

Please... Spread the word, contribute, submit improvements and issues, unit tests, no input is too little. Thank you in advance <3
