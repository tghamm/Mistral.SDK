# Mistral.SDK

[![.NET](https://github.com/tghamm/Mistral.SDK/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tghamm/Mistral.SDK/actions/workflows/dotnet.yml) [![Nuget](https://img.shields.io/nuget/v/Mistral.SDK)](https://www.nuget.org/packages/Mistral.SDK/)

Mistral.SDK is an unofficial C# client designed for interacting with the Mistral API. This powerful interface simplifies the integration of Mistral AI into your C# applications.  It targets netstandard2.0, .net6.0 and .net8.0.

## Table of Contents

- [Installation](#installation)
- [API Keys](#api-keys)
- [HttpClient](#httpclient)
- [Usage](#usage)
- [Examples](#examples)
  - [Non-Streaming Call](#non-streaming-call)
  - [Streaming Call](#streaming-call)
  - [IChatClient](#ichatclient)
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

## HttpClient

The `MistralClient` can optionally take a custom `HttpClient` in the `MistralClient` constructor, which allows you to control elements such as retries and timeouts. Note: If you provide your own `HttpClient`, you are responsible for disposal of that client.

## Usage

There are two ways to start using the `MistralClient`.  The first is to simply new up an instance of the `MistralClient` and start using it, the second is to use the messaging/Embedding client with the new `Microsoft.Extensions.AI.Abstractions` builder.
Brief examples of each are below.

Option 1:

```csharp
var client = new MistralClient();
```

Option 2:

```csharp
//chat client
IChatClient client = new MistralClient().Completions;

//embeddings generator
IEmbeddingGenerator<string, Embedding<float>> client = new MistralClient().Embeddings;
```

Both support all the core features of the `MistralClient's` Messaging and Embedding capabilities, but the latter will be fully featured in .NET 9 and provide built in telemetry and DI and make it easier to choose which SDK you are using.

## Examples

### Non-Streaming Call

Here's an example of a non-streaming call to the mistral-medium completions endpoint (other options are available and documented, but omitted for brevity):

```csharp
var client = new MistralClient();
var request = new ChatCompletionRequest(
    //define model - required
    ModelDefinitions.MistralMedium,
    //define messages - required
    new List<ChatMessage>()
{
    new ChatMessage(ChatMessage.RoleEnum.System, 
        "You are an expert at writing sonnets."),
    new ChatMessage(ChatMessage.RoleEnum.User, 
        "Write me a sonnet about the Statue of Liberty.")
}, 
    //optional - defaults to false
    safePrompt: true, 
    //optional - defaults to 0.7
    temperature: 0, 
    //optional - defaults to null
    maxTokens: 500, 
    //optional - defaults to 1
    topP: 1, 
    //optional - defaults to null
    randomSeed: 32);
var response = await client.Completions.GetCompletionAsync(request);
Console.WriteLine(response.Choices.First().Message.Content);
```

### Streaming Call

The following is an example of a streaming call to the mistral-medium completions endpoint:

```csharp
var client = new MistralClient();
var request = new ChatCompletionRequest(
    ModelDefinitions.MistralMedium, 
    new List<ChatMessage>()
{
    new ChatMessage(ChatMessage.RoleEnum.System, 
        "You are an expert at writing sonnets."),
    new ChatMessage(ChatMessage.RoleEnum.User, 
        "Write me a sonnet about the Statue of Liberty.")
});
var results = new List<ChatCompletionResponse>();
await foreach (var res in client.Completions.StreamCompletionAsync(request))
{
    results.Add(res);
    Console.Write(res.Choices.First().Delta.Content);
}
```
### IChatClient

The `MistralClient` has support for the new `IChatClient` from Microsoft and offers a slightly different mechanism for using the `MistralClient`.  Below are a few examples.

```csharp
//non-streaming
IChatClient client = new MistralClient().Completions;

var response = await client.CompleteAsync(new List<ChatMessage>()
{
    new(ChatRole.System, "You are an expert at writing sonnets."),
    new(ChatRole.User, "Write me a sonnet about the Statue of Liberty.")
}, new() { ModelId = ModelDefinitions.OpenMistral7b });

Assert.IsTrue(!string.IsNullOrEmpty(response.Message.Text));

//streaming call
IChatClient client = new MistralClient().Completions;

var sb = new StringBuilder();
await foreach (var update in client.CompleteStreamingAsync(new List<ChatMessage>()
    {
        new(ChatRole.System, "You are an expert at writing Json."),
        new(ChatRole.User, "Write me a simple 'hello world' statement in a json object with a single 'result' key.")
    }, new() { ModelId = ModelDefinitions.MistralLarge, ResponseFormat = ChatResponseFormat.Json }))
{
    sb.Append(update);
}

//parse json
Assert.IsNotNull(JsonSerializer.Deserialize<JsonResult>(sb.ToString()));

//Embeddings call
EmbeddingGenerator<string, Embedding<float>> client = new MistralClient().Embeddings;
            var response = await client.GenerateEmbeddingVectorAsync("hello world", new() { ModelId = ModelDefinitions.MistralEmbed });
            Assert.IsTrue(!response.IsEmpty);

```
Please see the unit tests for even more examples.

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
var request = new EmbeddingRequest(
    ModelDefinitions.MistralEmbed, 
    new List<string>() { "Hello world" }, 
    EmbeddingRequest.EncodingFormatEnum.Float);
var response = await client.Embeddings.GetEmbeddingsAsync(request);
```

## Contributing

Pull requests are welcome. If you're planning to make a major change, please open an issue first to discuss your proposed changes.

## License

This project is licensed under the [MIT](https://choosealicense.com/licenses/mit/) License.
