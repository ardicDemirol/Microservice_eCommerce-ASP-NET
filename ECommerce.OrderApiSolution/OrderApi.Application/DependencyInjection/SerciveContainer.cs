using ECommerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyInjection;

public static class SerciveContainer
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
    {
        // Register HttpClient Service
        // Create Dependency Injection
        services.AddHttpClient<IOrderService, OrderService>(options =>
        {
            options.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
            options.Timeout = TimeSpan.FromSeconds(5000);
        });

        // Create Retry Startegy
        var retryStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            OnRetry = args =>
            {
                string message = $"OnRetry, Attempt: {args.AttemptNumber}. Outcome {args.Outcome}";
                LogException.LogToConsole(message);
                LogException.LogToDebug(message);

                return ValueTask.CompletedTask;
            }
        };


        // Use Retry Strategy
        services.AddResiliencePipeline("my-retry-pipeline", builder =>
        {
            builder.AddRetry(retryStrategy);
        });

        return services;
    }
}
