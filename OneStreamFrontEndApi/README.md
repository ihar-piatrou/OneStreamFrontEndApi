# Project Architecture Decision Record (ADR) - README

## Overview

This project is a backend API that integrates with two other APIs to aggregate data and provide caching to enhance performance. The key goals are reliability, performance, maintainability, and scalability.

## Unit Tests

This project includes a unit tests project to ensure the reliability and correctness of individual components. Unit tests are written to verify core functionalities, edge cases, and expected behavior under various conditions.

## Key Features

### 1. Multithreading for Parallel API Calls

- **Description**: The `FrontEndController` uses multithreading to call two external APIs in parallel, minimizing overall response time.
- **Benefit**: Reduces latency by executing multiple API calls concurrently.

### 2. Resilience with Polly

- **Description**: The project uses Polly to implement retry policies, handling transient HTTP errors (like 429 or 5xx) with exponential backoff.
- **Benefit**: Increases the resilience of the system against temporary external API failures.

### 3. In-Memory Caching

- **Description**: Responses from external APIs are cached using `IMemoryCache` to reduce redundant calls.
- **Benefit**: Reduces response times and external API load.
- **Note**: This approach is suitable for a single-server environment. If scaling to multiple servers, a distributed caching mechanism is recommended.

### 4. Centralized Error Handling

- **Description**: A custom middleware (`ExceptionHandlingMiddleware`) is used to catch and handle unhandled exceptions, providing consistent error responses.
- **Benefit**: Simplifies controller code and ensures consistent error reporting throughout the application.

### 5. Structured Logging with Serilog

- **Description**: Serilog is used for structured logging, writing logs to both the console and rolling files.
- **Benefit**: Provides comprehensive and flexible logging that aids in debugging and monitoring.

## How It Works

1. **Parallel API Calls**: The `FrontEndController` makes concurrent calls to the two external APIs to aggregate data faster, improving response time.

2. **Polly for Resilience**: External API calls are wrapped with Polly retry logic to gracefully handle transient errors, applying exponential backoff strategies.

3. **Caching**: The `ApiServices` class uses in-memory caching to store responses, reducing the frequency of external API requests and improving overall performance.

4. **Error Handling Middleware**: Custom middleware is registered in the pipeline to catch and process all exceptions, mapping them to appropriate HTTP status codes and logging them for easier debugging.

5. **Logging**: Serilog is configured to log all important information and errors, providing insights for debugging, development, and production monitoring.

## Decision Drivers

- **Reliability**: Robust handling of API failures and transient errors.
- **Performance**: Reduction in response times and the number of external API calls.
- **Maintainability**: A modular and clean approach that ensures the code is easy to update and extend.
- **Scalability**: Architecture designed to support future integrations and increased traffic.

## Getting Started

To run the project:

1. Clone the repository and restore dependencies.
2. Ensure that the external APIs used for data aggregation are accessible.
3. Run the application with `dotnet run`.

### Requirements

- .NET 6 or later
- External API credentials/configurations

### Running the Project

1. Clone the repository:
   ```sh
   git clone <repository-url>
   ```
2. Navigate to the project directory and restore the packages:
   ```sh
   cd <project-directory>
   dotnet restore
   ```
3. Run the application:
   ```sh
   dotnet run
   ```

## Additional Notes

- For multi-server deployments, consider replacing `IMemoryCache` with a distributed cache like Redis.
- Polly retry logic adds to the response time when retries are performed, but provides resilience against transient errors.
- Serilog configuration can be customized to include additional logging sinks, such as cloud logging services.

## Contributions

When contributing to the project, please ensure that you add or update unit tests for any new or modified functionality.


Contributions are welcome! Please ensure that your code follows the project guidelines and that you include tests for any new functionality.

## License

This project is licensed under the MIT License.
