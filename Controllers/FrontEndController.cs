using Microsoft.AspNetCore.Mvc;
using OneStreamFrontEndApi.Services;

namespace OneStreamFrontEndApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FrontEndController : ControllerBase
    {
        private readonly IApiServices _frontEndServices;
        
        public FrontEndController(IApiServices frontEndServices)
        {
            _frontEndServices = frontEndServices;
        }

        // GET: api/v1/FrontEnd
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            //var api2Responses = await _frontEndServices.CallApiAsync("api2Data429", "https://httpstat.us/502");

            var api2Response = await _frontEndServices.CallApiAsync("api2Data", "https://catfact.ninja/fact");
            var api3Response = await _frontEndServices.CallApiAsync("api3Data", "https://catfact.ninja/breeds");

            return Ok(new { Api2 = api2Response, Api3 = api3Response });
        }

        // POST: api/v1/FrontEnd
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] object data)
        {
            var api2Response = await _frontEndServices.CallApiAsync("api2Data", "https://catfact.ninja/fact");
            var api3Response = await _frontEndServices.CallApiAsync("api3Data", "https://catfact.ninja/breeds");

            return Ok(new { Api2 = api2Response, Api3 = api3Response, InputData = data });         
        }
    }
}
