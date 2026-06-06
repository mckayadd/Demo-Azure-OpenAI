/*
1- Create an Azure AI Foundry Resource
2- Add Nuget Packages: Azure.AI.OpenAI, Microsoft.Agents.AI.OpenAI
3- Create an AzureOpenAIClient (API Key or Azure Identity)
4- Get a ChatClient and Create an AI Agent from it
5- Call RunAsync or RunStreamingAsync
*/

using System;
using System.IO;
using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;       
using Microsoft.Agents.AI.OpenAI; 
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

class Program
{
    private static string? _endpoint;
    private static string? _modelName;
    private static readonly string? apiKey = Environment.GetEnvironmentVariable("AZURE_API_KEY");

    static async Task Main(string[] args)
    {
        string baseDir = AppContext.BaseDirectory;
        string projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
        string jsonFolderPath = Path.GetFullPath(Path.Combine(projectDir, "..", "..", "..", "ConfigurationFiles"));
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(jsonFolderPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _endpoint = configuration["AzureOpenAI:Endpoint"];
        _modelName = configuration["AzureOpenAI:ModelName"];

        if(string.IsNullOrEmpty(apiKey))
        {
            System.Console.WriteLine("API Key not found!");
            return;
        }

        if(string.IsNullOrEmpty(_endpoint))
        {
            System.Console.WriteLine("Endpoint not found!");
            return;
        }

        if(string.IsNullOrEmpty(_modelName))
        {
            System.Console.WriteLine("Model not found!");
            return;
        }
/*
        Console.WriteLine("--- Azure AI Configuration (Through environment and JSON) ---");
        Console.WriteLine($"Folder: {jsonFolderPath}");
        Console.WriteLine($"Endpoint       : {_endpoint}");
        Console.WriteLine($"Model          : {_modelName}");
        Console.WriteLine($"ApiKey         : {apiKey}");
        Console.WriteLine("------------------------------------------------");
*/
        var client = new AzureOpenAIClient(new Uri(_endpoint), new ApiKeyCredential(apiKey));
        AIAgent agent = client.GetChatClient(_modelName).AsAIAgent(
            instructions: "You are a helpful assistant.",
            name: "MyAIApp"
        );
        AgentResponse response = await agent.RunAsync("What is the color of a banana?");
        System.Console.WriteLine(response);
    }
}



