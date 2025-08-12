using Polly;
using Polly.Contrib.WaitAndRetry;

namespace TaskService.WebApi.Clients;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomClients(
        this IServiceCollection services)
    {
        var amountAttempts = 3;
        services.AddHttpClient("jwt", client =>
            {
                client.BaseAddress = new Uri("");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddTransientHttpErrorPolicy(policy => policy
                .WaitAndRetryAsync(
                    Backoff.DecorrelatedJitterBackoffV2(
                        medianFirstRetryDelay: TimeSpan.FromSeconds(1),
                        retryCount:  amountAttempts)
                )
            )
            .AddTransientHttpErrorPolicy(policy => policy
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking:  amountAttempts,
                    durationOfBreak: TimeSpan.FromSeconds(15)
                )
            );
        return services;
    }
}