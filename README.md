# Cogworks.AzureSearch &middot; [![GitHub license](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE.md) [![Github Build](https://img.shields.io/github/workflow/status/thecogworks/cogworks.azuresearch/Changelog%20generator%20and%20NuGet%20Releasing)](https://github.com/thecogworks/Cogworks.AzureSearch/actions?query=workflow%3A%22Changelog+generator+and+NuGet+Releasing%22) [![NuGet Version](https://img.shields.io/nuget/v/Cogworks.AzureSearch)](https://www.nuget.org/packages/Cogworks.AzureSearch/) [![codecov](https://codecov.io/gh/thecogworks/UmbracoAzureSearch/branch/master/graph/badge.svg?token=UMLJ5S8UJX)](undefined)

A wrapper to Azure Search allowing to easily setup Azure Search indexes, searchers and using it with DI/IoC approach (currently with support for Umbraco, LightInject and Autofac).

## Extension libraries

| Package Name                   | Release (NuGet) |
|--------------------------------|-----------------|
| `Cogworks.AzureSearch.Autofac.IocExtension` | [![NuGet Version](https://img.shields.io/nuget/v/Cogworks.AzureSearch.Autofac.IocExtension)](https://www.nuget.org/packages/Cogworks.AzureSearch.Autofac.IocExtension/) |
| `Cogworks.AzureSearch.LightInject.IocExtension` | [![NuGet Version](https://img.shields.io/nuget/v/Cogworks.AzureSearch.LightInject.IocExtension)](https://www.nuget.org/packages/Cogworks.AzureSearch.LightInject.IocExtension/) |
| `Cogworks.AzureSearch.Umbraco.IocExtension` | [![NuGet Version](https://img.shields.io/nuget/v/Cogworks.AzureSearch.Umbraco.IocExtension)](https://www.nuget.org/packages/Cogworks.AzureSearch.Umbraco.IocExtension/) |

## Usage

#### Registration

```csharp

_composing.RegisterAzureSearch()
    .RegisterClientOptions("[AzureSearchServiceName]", "[AzureSearchCredentials]")
    .RegisterIndexOptions(false, false) // for now required
    .RegisterIndexDefinitions<FirstDocumentModel>("first-document-index-name")
    .RegisterIndexDefinitions<SecondDocumentModel>("second-document-index-name")
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
