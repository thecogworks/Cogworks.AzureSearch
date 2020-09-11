# Cogworks.AzureSearch [![GitHub license](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE.md)

A wrapper to Azure Search allowing to easily setup Azure Search indexes, searchers and using it with DI/IoC approach (currently with support for Umbraco, LightInject and Autofac).

## Example Usage

```csharp
var customTestSearch = Substitute.For<ICustomTestSearch>();

_composing.RegisterAzureSearch()
                .RegisterClientOptions("[AzureSearchServiceName]", "[AzureSearchCredentials]")
                .RegisterIndexOptions(false, false)
                .RegisterIndexDefinitions<FirstTestDocumentModel>(FirstDocumentIndexName)
                .RegisterIndexDefinitions<SecondTestDocumentModel>(SecondDocumentIndexName)
                .RegisterDomainSearcher<CustomTestSearch, ICustomTestSearch, FirstTestDocumentModel>(customTestSearch);
```

## License
  
- Cogworks.AzureSearch is licensed under the [Apache License, Version 2.0](https://opensource.org/licenses/Apache-2.0)

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org/) to clarify expected behavior in our community.
For more information, see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## How can you help?

Please... Spread the word, contribute, submit improvements and issues, unit tests, no input is too little. Thank you in advance <3
