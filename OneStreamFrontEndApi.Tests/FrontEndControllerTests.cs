using Microsoft.AspNetCore.Mvc;
using Moq;
using OneStreamFrontEndApi.Controllers;
using OneStreamFrontEndApi.Services;
using Microsoft.Extensions.Configuration;

namespace OneStreamFrontEndApi.Tests.Controllers
{
    public class FrontEndControllerTests
    {
        private readonly Mock<IApiServices> _mockApiServices;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IFileServices> _mockFileServices;
        private readonly FrontEndController _controller;

        public FrontEndControllerTests()
        {
            _mockApiServices = new Mock<IApiServices>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockFileServices = new Mock<IFileServices>();
            _controller = new FrontEndController(_mockApiServices.Object, _mockConfiguration.Object, _mockFileServices.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnsOkResult_WithExpectedData()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["ApiUrls:Api1"]).Returns("https://api1.com");
            _mockConfiguration.Setup(c => c["ApiUrls:Api2"]).Returns("https://api2.com");
            _mockApiServices.Setup(s => s.CallApiAsync("api1Data", "https://api1.com")).ReturnsAsync("Api1 Response");
            _mockApiServices.Setup(s => s.CallApiAsync("api2Data", "https://api2.com")).ReturnsAsync("Api2 Response");

            // Act
            var result = await _controller.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Api1 Response", okResult.Value?.ToString());
            Assert.Contains("Api2 Response", okResult.Value?.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1234")]
        [InlineData("This string is definitely longer than one hundred characters and should trigger the validation error in the controller.")]
        public async Task PostAsync_InvalidInputData_ReturnsBadRequest(string inputData)
        {
            // Act
            var result = await _controller.PostAsync(inputData);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Contains("The data length must be between 5 and 100 characters.", badRequestResult.Value?.ToString());
        }

        [Fact]
        public async Task PostAsync_ValidInputData_ReturnsOkResult_WithExpectedData()
        {
            // Arrange
            var inputData = "Valid Data";
            _mockConfiguration.Setup(c => c["ApiUrls:Api1"]).Returns("https://api1.com");
            _mockConfiguration.Setup(c => c["ApiUrls:Api2"]).Returns("https://api2.com");
            _mockApiServices.Setup(s => s.CallApiAsync("api1Data", "https://api1.com")).ReturnsAsync("Api1 Response");
            _mockApiServices.Setup(s => s.CallApiAsync("api2Data", "https://api2.com")).ReturnsAsync("Api2 Response");

            // Act
            var result = await _controller.PostAsync(inputData);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Contains("Api1 Response", okResult.Value?.ToString());
            Assert.Contains("Api2 Response", okResult.Value?.ToString());
            Assert.Contains(inputData, okResult.Value?.ToString());

            // Verify that WriteDataToFile was called
            _mockFileServices.Verify(f => f.WriteDataToFile("Api1 Response", "Api2 Response", inputData), Times.Once);
        }
    }
}
