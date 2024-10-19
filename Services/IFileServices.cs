namespace OneStreamFrontEndApi.Services
{
    public interface IFileServices
    {
        /// <summary>
        /// Defines a service for writing data to a file.
        /// </summary>
        /// <param name="api1Response">The response data from the first API, which can be null.</param>
        /// <param name="api2Response">The response data from the second API, which can be null.</param>
        /// <param name="userData">The user-provided data, which can be null.</param>
        Task WriteDataToFile(string? api1Response, string? api2Response, string? userData);
    }
}
