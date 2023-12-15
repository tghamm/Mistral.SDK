# Mistral.SDK

[![.NET](https://github.com/tghamm/Mistral.SDK/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tghamm/Mistral.SDK/actions/workflows/dotnet.yml)

Mistral.SDK is an unofficial C# client designed for interacting with the Mistral API. This powerful interface simplifies the integration of Mistral AI into your C# applications.  It targets netstandard2.0, and .net6.0.

## Table of Contents

- [Installation](#installation)
- [API Keys](#api-keys)
- [IHttpClientFactory](#ihttpclientfactory)
- [Usage](#usage)
- [Examples](#examples)
  - [Non-Streaming Call](#non-streaming-call)
  - [Streaming Call](#streaming-call)
  - [List Models](#list-models)
  - [Embeddings](#embeddings)
- [Contributing](#contributing)
- [License](#license)

## Installation

Install Mistral.SDK via the [NuGet](https://www.nuget.org/packages/Mistral.SDK) package manager:

```bash
PM> Install-Package Mistral.SDK
```

## API Keys

You can load the API Key from an environment variable named `MISTRAL_API_KEY` by default. Alternatively, you can supply it as a string to the `MistralClient` constructor.

## IHttpClientFactory

The `MistralClient` can optionally take an `IHttpClientFactory`, which allows you to control elements such as retries and timeouts.

## Usage

To start using the Mistral API, simply create an instance of the `MistralClient` class.

## Examples

### Non-Streaming Call

Here's an example of a non-streaming call to the mistral-medium completions endpoint (other options are available and documented, but omitted for brevity):

```csharp
var client = new MistralClient();
var request = new ChatCompletionRequest(ModelDefinitions.MistralMedium, new List<ChatMessage>()
{
    new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
    new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
});
var response = await client.Completions.GetCompletionAsync(request);
Console.WriteLine(response.Choices.First().Message.Content);
```

### Streaming Call

The following is an example of a streaming call to the mistral-medium completions endpoint:

```csharp
var client = new MistralClient();
var request = new ChatCompletionRequest(ModelDefinitions.MistralMedium, new List<ChatMessage>()
{
    new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
    new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
});
var results = new List<ChatCompletionResponse>();
await foreach (var res in client.Completions.StreamCompletionAsync(request))
{
    results.Add(res);
    Console.Write(res.Choices.First().Delta.Content);
}
```

### List Models

The following is an example of a call to list the available models:

```csharp
var client = new MistralClient();

var response = await client.Models.GetModelsAsync();
```

### Embeddings

The following is an example of a call to the mistral-embed embeddings model/endpoint:

```csharp
var client = new MistralClient();
var request = new EmbeddingRequest(ModelDefinitions.MistralEmbed, new List<string>() { "Hello world" }, EmbeddingRequest.EncodingFormatEnum.Float);
var response = await client.Embeddings.GetEmbeddingsAsync(request);
```

## Contributing

Pull requests are welcome. If you're planning to make a major change, please open an issue first to discuss your proposed changes.

## License

This project is licensed under the [MIT](https://choosealicense.com/licenses/mit/) License.
