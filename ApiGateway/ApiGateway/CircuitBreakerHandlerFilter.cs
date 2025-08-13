namespace ApiGateway;

using System;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

public class CircuitBreakerHandlerFilter : IHttpMessageHandlerBuilderFilter
{
    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    {
        return builder =>
        {
            next(builder);

            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(10)
                );
            builder.AdditionalHandlers.Add(new PolicyHttpMessageHandler(policy));
        };
    }
}