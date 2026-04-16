using eCommerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
    {
        // Add HttpClient
        services.AddHttpClient<IOrderService, OrderService>(opts =>
        {
            opts.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
            opts.Timeout = TimeSpan.FromSeconds(10);
        });

        // Create Retry Strategy
        var retryStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(500),
            OnRetry = args =>
            {
                string message = $"Retry attempt {args.AttemptNumber} failed. Outcome: {args.Outcome}";
                LogException.LogToConsole(message);
                LogException.LogToDebugger(message);
                return ValueTask.CompletedTask;
            }
        };

        // Use retry strategy
        services.AddResiliencePipeline("Retry", builder => builder.AddRetry(retryStrategy));

        return services;
    }
}