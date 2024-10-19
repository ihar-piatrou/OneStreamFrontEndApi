# Project Overview

This is a backend API that integrates with two other APIs for data aggregation, focusing on reliability, performance, maintainability, and scalability. Key features include parallel API calls using multithreading, retry logic with Polly, in-memory caching, centralized error handling, and structured logging with Serilog.

For detailed information, visit the [full README](./OneStreamFrontEndApi/README.md).

Code analysis:
[Question about Cat and Dog classes](./CodeAnalysisQuestions/Animal.md).
[Question about A and B classes](./CodeAnalysisQuestions/ClassAandB.md).

## Running the Project

1. Clone the repository and restore dependencies.
2. Run the application:
   ```sh
   dotnet run
