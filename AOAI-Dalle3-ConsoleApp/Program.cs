using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetEnv; // <-- 1) import DotNetEnv

namespace AOAI_Dalle3_ConsoleApp
{
    public class DalleRequestBody
    {
        public string prompt { get; set; }
        public string size { get; set; }
        public int n { get; set; } = 1;
        public string quality { get; set; }
        public string style { get; set; }
        public string response_format { get; set; }
    }

    public class DalleResponseData
    {
        public string url { get; set; }
        public string revised_prompt { get; set; }
    }

    public class DalleResponse
    {
        public long created { get; set; }
        public DalleResponseData[] data { get; set; }
    }

    public class DalleError
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class DalleErrorResponse
    {
        public long created { get; set; }
        public DalleError error { get; set; }
    }

    internal class Program
    {
        // Adjust the API version as needed for your Azure OpenAI resource
        private const string apiVersion = "2024-02-01";

        static async Task Main(string[] args)
        {
            // 2) Load environment variables from the .env file
            DotNetEnv.Env.Load();

            // Retrieve the environment variables
            string resourceName = Environment.GetEnvironmentVariable("AZURE_OPENAI_RESOURCE_NAME");
            string deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");
            string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            
            // Check if all environment variables are present
            if (string.IsNullOrWhiteSpace(resourceName) ||
                string.IsNullOrWhiteSpace(deploymentName) ||
                string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("Error: One or more environment variables are missing:");
                Console.WriteLine("  AZURE_OPENAI_RESOURCE_NAME");
                Console.WriteLine("  AZURE_OPENAI_DEPLOYMENT_NAME");
                Console.WriteLine("  AZURE_OPENAI_API_KEY");
                Console.WriteLine("\nPlease ensure these are set in your .env file (and .env is not committed to source control).");
                return;
            }

            Console.WriteLine("=== AOAI DALL-E 3 Console App ===");

            // Ask the user for a prompt
            Console.Write("Enter an image prompt: ");
            string prompt = Console.ReadLine() ?? "A beautiful sunset over the mountains";

            // Ask the user for Size
            Console.WriteLine("\nSelect image size:");
            Console.WriteLine("1) 1024x1024 (Square, faster to generate)");
            Console.WriteLine("2) 1792x1024 (Widescreen)");
            Console.WriteLine("3) 1024x1792 (Vertical)");
            Console.Write("Enter your choice (1,2,3): ");
            string sizeChoice = Console.ReadLine() ?? "1";

            string size = sizeChoice switch
            {
                "2" => "1792x1024",
                "3" => "1024x1792",
                _   => "1024x1024",  // Default
            };

            // Ask the user for Style
            Console.WriteLine("\nSelect style:");
            Console.WriteLine("1) natural (similar to DALL-E 2 default)");
            Console.WriteLine("2) vivid (hyper-real, cinematic)");
            Console.Write("Enter your choice (1 or 2): ");
            string styleChoice = Console.ReadLine() ?? "2";

            string style = styleChoice switch
            {
                "1" => "natural",
                _   => "vivid"  // Default
            };

            // Ask the user for Quality
            Console.WriteLine("\nSelect quality:");
            Console.WriteLine("1) standard (faster)");
            Console.WriteLine("2) hd (finely-detailed, more consistent)");
            Console.Write("Enter your choice (1 or 2): ");
            string qualityChoice = Console.ReadLine() ?? "1";

            string quality = qualityChoice switch
            {
                "2" => "hd",
                _   => "standard"  // Default
            };

            // Ask the user for Response Format
            Console.WriteLine("\nSelect response format:");
            Console.WriteLine("1) url (returns a URL to download the image)");
            Console.WriteLine("2) b64_json (returns a Base64-encoded string)");
            Console.Write("Enter your choice (1 or 2): ");
            string formatChoice = Console.ReadLine() ?? "1";

            string responseFormat = formatChoice switch
            {
                "2" => "b64_json",
                _   => "url", // Default
            };

            // Build the request body
            var requestBody = new DalleRequestBody
            {
                prompt = prompt,
                size = size,
                quality = quality,
                style = style,
                n = 1,
                response_format = responseFormat
            };

            // Prepare the URL
            string endpoint = $"https://{resourceName}.openai.azure.com/" +
                              $"openai/deployments/{deploymentName}/images/generations?api-version={apiVersion}";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Set the API key in the headers
                    httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

                    // Serialize the JSON body
                    var jsonContent = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Make the POST request
                    HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize success response
                        var dalleResponse = JsonSerializer.Deserialize<DalleResponse>(responseString);
                        if (dalleResponse?.data != null && dalleResponse.data.Length > 0)
                        {
                            Console.WriteLine("\n=== Image Generation Successful! ===");
                            if (responseFormat == "url")
                            {
                                Console.WriteLine("URL to generated image:");
                                Console.WriteLine(dalleResponse.data[0].url);
                                Console.WriteLine("Revised prompt (if any):");
                                Console.WriteLine(dalleResponse.data[0].revised_prompt);
                            }
                            else // b64_json
                            {
                                Console.WriteLine("Base64 JSON for generated image:");
                                Console.WriteLine(dalleResponse.data[0].url);
                                Console.WriteLine("Revised prompt (if any):");
                                Console.WriteLine(dalleResponse.data[0].revised_prompt);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No image data returned in the response.");
                        }
                    }
                    else
                    {
                        // Deserialize error response
                        var errorResponse = JsonSerializer.Deserialize<DalleErrorResponse>(responseString);
                        Console.WriteLine("\n=== Image Generation Failed ===");
                        if (errorResponse?.error != null)
                        {
                            Console.WriteLine($"Error Code: {errorResponse.error.code}");
                            Console.WriteLine($"Message: {errorResponse.error.message}");
                        }
                        else
                        {
                            Console.WriteLine("An unknown error occurred.");
                            Console.WriteLine(responseString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n=== Exception Occurred ===");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
