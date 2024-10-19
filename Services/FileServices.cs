using System.Text.Json;

namespace OneStreamFrontEndApi.Services
{
    public class FileServices : IFileServices
    {
        ///<inheritdoc/>
        public async Task WriteDataToFile(string? api1Response, string? api2Response, string? userData)
        {
            // Write the result to a file
            var outputFilePath = "results.txt";
            var resultData = new
            {
                Api1 = api1Response,
                Api2 = api2Response,
                InputData = userData
            };

            // Convert the result data to a JSON string
            var jsonResult = JsonSerializer.Serialize(resultData);

            // Write the JSON string to a file
            await File.WriteAllTextAsync(outputFilePath, jsonResult);
        }
    }
}
