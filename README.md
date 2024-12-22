# Azure OpenAI-DALL-E3-ConsoleApp

This entry is part of the **[AI Advent Calendar 2024](https://dev.to/roberto_navarro_mate/calendario-de-adviento-de-inteligencia-artificial-2024-en-espanol-bdb)**.

This console application demonstrates how to generate images using [Azure OpenAI’s DALL-E 3](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/concepts/models#dall%C3%A9) API from a .NET 9 console project.

## Table of Contents
1. [Prerequisites](#prerequisites)  
2. [Clone the Repository](#clone-the-repository)  
3. [Configure Environment Variables](#configure-environment-variables)  
4. [Install Dependencies](#install-dependencies)  
5. [Run the Application](#run-the-application)  
6. [Project Structure](#project-structure)  
7. [How the Code Works](#how-the-code-works)  
8. [Additional Resources](#additional-resources)  

---

## Prerequisites

1. **.NET 9** (or a compatible .NET version) installed on your machine.  
   - You can download it from [https://dotnet.microsoft.com/en-us/download](https://dotnet.microsoft.com/en-us/download).
2. An **Azure OpenAI Resource** that has a **DALL-E 3** deployment.  
   - You must have your resource name, deployment name, and API key from the Azure Portal.  
3. **Git** installed (to clone the repository).

---

## Clone the Repository

1. Open your terminal (or command prompt).
2. Navigate to the directory where you want to store the project.
3. Run the following command to clone the repo:

   ```bash
   git clone https://github.com/ppiova/AzureOpenAI-DALL-E-ConsoleApp.git
3. Navigate into the newly created folder:
```bash
cd AzureOpenAI-DALL-E-ConsoleApp
```

## Configure Environment Variables
This project uses a .env file to load your credentials (environment variables) at runtime. This approach helps keep secret data out of source control.

1. In the root of the project, create a file named .env.

2. Add the following lines, replacing the placeholder values with your actual Azure resource details:
```bash
AZURE_OPENAI_RESOURCE_NAME=myResourceName
AZURE_OPENAI_DEPLOYMENT_NAME=myDeploymentName
AZURE_OPENAI_API_KEY=mySuperSecretKey
```
3.Check the .gitignore file to confirm that the .env file is listed. This ensures it won’t be committed to the repo.

## Install Dependencies
From within the project folder, install dependencies (like DotNetEnv) and restore any NuGet packages:
```bash
dotnet restore
```
If you want to install DotNetEnv manually (in case it’s not already in the .csproj), run:
```bash
dotnet add package DotNetEnv
```
## Run the Application

After setting up your .env file and restoring packages:
```bash
dotnet run
```

## Console Prompts
1. Image prompt: A descriptive text prompt for the image you want to generate.
2. Image size: Choose from 1024x1024, 1792x1024, or 1024x1792.
3. Style: Choose between natural or vivid.
4. Quality: Choose between standard (faster) or hd (higher detail).
5. Response format:
    - url – returns a URL link to the generated image.
    - b64_json – returns a Base64-encoded image in JSON format.

The console will show either a URL to your generated image or the Base64-encoded output.

## Project Structure
A simplified view of the most important files and folders:
```bash
AzureOpenAI-DALL-E-ConsoleApp/
│
├─ .gitignore
├─ .env           (excluded from Git; contains your secrets)
├─ Program.cs     (main entry point of the console app)
├─ AzureOpenAI-DALL-E-ConsoleApp.csproj
└─ README.md

```
- .env: Local file to store environment variables (like resource name, deployment name, and API key).
- Program.cs: Main C# file that loads environment variables, prompts the user, builds the request, and sends it to Azure OpenAI’s DALL-E endpoint.
- .gitignore: Ensures .env is excluded from version control.

## How the Code Works

1. Load environment variables:

   - At the top of Main, we use DotNetEnv.Env.Load() to read key-value pairs from the .env file.
   - Environment.GetEnvironmentVariable(...) retrieves these values later in the code.

2. Prompt the user for image details:

   - We ask for a descriptive prompt, image size, style, quality, and response format.
   - Each choice sets the respective property in our DalleRequestBody.

3. Build the request:

   - A DalleRequestBody object is serialized to JSON and sent to the Azure OpenAI Image Generation endpoint.
   - We include the environment variable apiKey in the api-key header of the request.

4. Handle the response:

   - On success, we deserialize the JSON to a DalleResponse object, which contains an array of generated images (DalleResponseData).
   - On failure, we deserialize the error to DalleErrorResponse and display relevant messages in the console.

5. Display results:

   - If response_format is url, we print the URL for the generated image.
   - If response_format is b64_json, we print the Base64-encoded output returned.

![DALL-E 3 Generate Images](assets/DALL-E-3-Generate-Images.gif)

![DALL-E 3 Generate_00](assets/generated_00-dall-e3.png)

## Additional Resources

   - [How to work with the DALL-E models](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/dall-e?tabs=dalle3&WT.mc_id=AI-MVP-5004753)
   - [Quickstart: Generate images with Azure OpenAI Service](https://learn.microsoft.com/azure/ai-services/openai/dall-e-quickstart?tabs=dalle3%2Ccommand-line%2Cjavascript-keyless%2Ctypescript-keyless&pivots=programming-language-studio&WT.mc_id=AI-MVP-5004753)

## Contributing
If you wish to contribute to the project, please fork the repository and create a pull request with your changes.

## License
This project is licensed under the MIT License.

## Contact
For any questions or inquiries, please contact the repository owner.