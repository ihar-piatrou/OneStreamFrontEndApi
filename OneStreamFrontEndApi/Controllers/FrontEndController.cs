using Microsoft.AspNetCore.Mvc;
using OneStreamFrontEndApi.Services;
using System.Net;

namespace OneStreamFrontEndApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FrontEndController : ControllerBase
    {
        private readonly IApiServices _frontEndServices;
        private readonly IConfiguration _configuration;
        private readonly IFileServices _fileServices;

        public FrontEndController(IApiServices frontEndServices, IConfiguration configuration, IFileServices fileServices)
        {
            _frontEndServices = frontEndServices;
            _configuration = configuration;
            _fileServices = fileServices;
        }

        // GET: api/v1/FrontEnd
        /// <summary>
        /// Gets data from API1 and API2.
        /// </summary>
        /// <returns>Returns the responses from API2 and API3.</returns>
        /// <response code="200">Returns the successful response from the API.</response>
        /// <response code="502">Bad Gateway - Indicates an error occurred when calling an external API.</response>
        /// <response code="401">Unauthorized - The request was not authorized.</response>
        /// <response code="400">Bad Request - Invalid input was provided.</response>
        /// <response code="404">Not Found - The resource was not found.</response>
        /// <response code="429">To many requests - Indicates to many requests.</response>
        /// <response code="500">Internal Server Error - A server error occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadGateway)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAsync()
        {
            var result = await GetResultsFrom2Apis();
            return Ok(new { Api1 = result.Api1Response, Api2 = result.Api2Response });
        }

        // POST: api/v1/FrontEnd
        /// <summary>
        /// Post user data and data from API1 and API2 to file.
        /// </summary>
        /// <returns>Returns the responses from API2 and API3.</returns>
        /// <response code="200">Returns the successful response from the API.</response>
        /// <response code="502">Bad Gateway - Indicates an error occurred when calling an external API.</response>
        /// <response code="401">Unauthorized - The request was not authorized.</response>
        /// <response code="400">Bad Request - Invalid input was provided.</response>
        /// <response code="404">Not Found - The resource was not found.</response>
        /// <response code="429">To many requests - Indicates to many requests.</response>
        /// <response code="500">Internal Server Error - A server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadGateway)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] string data)
        {
            // Validate the length of the input data
            if (string.IsNullOrEmpty(data) || data.Length < 5 || data.Length > 100)
            {
                return BadRequest(new { Error = "The data length must be between 5 and 100 characters." });
            }

            // Proceed with the rest of the logic if validation passes
            var result = await GetResultsFrom2Apis();
            await _fileServices.WriteDataToFile(result.Api1Response, result.Api2Response, data);

            return Ok(new { Api1 = result.Api1Response, Api2 = result.Api2Response, InputData = data });
        }

        private async Task<(string? Api1Response, string? Api2Response)> GetResultsFrom2Apis()
        {
            var api1Url = _configuration["ApiUrls:Api1"];
            var api2Url = _configuration["ApiUrls:Api2"];

            // Create tasks to call the APIs concurrently
            var api1Task = _frontEndServices.CallApiAsync("api1Data", api1Url);
            var api2Task = _frontEndServices.CallApiAsync("api2Data", api2Url);

            // Await both tasks in parallel using Task.WhenAll
            await Task.WhenAll(api1Task, api2Task);

            // Get the results after both tasks have completed
            return (await api1Task, await api2Task);
        }
    }
}
