# Architecture Decision Record (ADR)

## Context and Problem Statement

The project is a backend API that integrates with two other APIs for data aggregation and caching.

**Key challenges:**
- Multithreading for parallel API calls to reduce response time.
- Retry policies for transient failures.
- Caching to reduce unnecessary API calls.
- Handling failures and providing appropriate responses.
- Logging for debugging and monitoring.

## Decision Drivers

- **Reliability**: Robust handling of API failures and transient errors.
- **Performance**: Reduce response times and API calls with caching.
- **Maintainability**: Modular services and clean coding practices.
- **Scalability**: Future integrations and increased traffic.

## Considered Options

### Multithreading for Parallel API Calls

Using multithreading in `FrontEndController` to call two external APIs in parallel, reducing the overall response time.

- **Pros**: Significant reduction in response time by calling APIs concurrently.
- **Cons**: Complexity in managing thread safety and handling potential concurrency issues.

### 1. Polly for Resilience

Polly is used for retry policies, including handling transient errors like 429 errors, with exponential backoff.

- **Pros**: Resilient against temporary failures, flexible configuration, handles common transient errors.
- **Cons**: Increases response time for retries.

### 2. In-Memory Caching

Cache API responses with IMemoryCache to minimize redundant calls and reduce latency.

- **Pros**: Reduces API load and latency, simple to implement.
- **Cons**: Not suitable for multi-server deployments; distributed cache needed for scaling.

### 3. Centralized Error Handling

Custom middleware catches unhandled exceptions and provides consistent error responses.

- **Pros**: Catches and logs all exceptions, simplifies controller code.
- **Cons**: Adds minor processing overhead.

### 4. Logging with Serilog

Serilog is used for structured logging, writing to both console and rolling files.

- **Pros**: Comprehensive logging, supports multiple sinks.
- **Cons**: Needs scaling consideration for distributed systems.

## Decision Outcome

**Chosen Option**: Polly for retry logic, in-memory caching, centralized error handling, and Serilog logging.

**Reasoning**: Provides resilience, performance improvement, and effective error handling and logging.

## Detailed Approaches

### 0. Parallel API Calls

In `FrontEndController`, two APIs are called in parallel using multithreading to reduce the total response time and improve performance.

### 1. API Service with Polly Retry and Caching

The `ApiServices` class calls external APIs while using caching to minimize requests. Polly handles transient HTTP errors (e.g., 5xx, 429) with exponential backoff retries.

### 2. Centralized Error Handling

The `ExceptionHandlingMiddleware` is registered in the pipeline to handle all exceptions, mapping them to appropriate HTTP status codes. This middleware helps provide a unified error handling approach, and all exceptions are logged through the middleware.

### 3. Logging with Serilog

Logging is set up with Serilog, writing to both console and file. This provides easy-to-understand, structured logging that can help during development, troubleshooting, or even in production environments.
