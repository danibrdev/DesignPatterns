#region

using System.Net;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;

#endregion

namespace CircuitBreaker.CrossCutting.Extensions;

public static class PollyExtension
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAtempt => TimeSpan.FromSeconds(Math.Pow(2, retryAtempt)));
    
    public static AsyncCircuitBreakerPolicy CreatePolicy()
        => Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(10),
                onBreak: (_, _) => { ShowCircuitState("Circuit cut, requests will not flow.", ConsoleColor.Red); },
                onReset: () => { ShowCircuitState("Circuit closed, requests flow normally.", ConsoleColor.Green); },
                onHalfOpen: () => { ShowCircuitState("Circuit in test mode, one request will be allowed.", ConsoleColor.Yellow); });
    
    /* if 3 consecutive errors occur, the circuit is cut for 1 minute */
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10), 
                OnBreak, OnReset, OnHalfOpen); 
    
    /* the circuit will be cut if 25% of requests fail in a 60 second window, with a minimum of 7 requests in the 60 and the circuit is cut for 5 minutes*/
    public static IAsyncPolicy<HttpResponseMessage> GetAdvancedCircuitBreakerPolicy()
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .AdvancedCircuitBreakerAsync(0.25, TimeSpan.FromSeconds(60), 5, TimeSpan.FromMinutes((5))); 

    #region CONSOLE FORMAT
    
    private static void OnHalfOpen()
    {
        ShowCircuitState("Circuit in test mode, one request will be allowed.", ConsoleColor.Yellow);
    }

    private static void OnReset()
    {
        ShowCircuitState("Circuit closed, requests flow normally.", ConsoleColor.Green);
    }

    private static void OnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan ts)
    {
        ShowCircuitState("Circuit cut, requests will not flow.", ConsoleColor.Red);
    }

    private static void ShowCircuitState(
        string statusDesc, 
        ConsoleColor backgroundColor)
    {
        var previousBackgroundColor = Console.BackgroundColor;
        var previousForegroundColor = Console.ForegroundColor;

        Console.BackgroundColor = backgroundColor;
        Console.ForegroundColor = ConsoleColor.Black; 
        
        Console.Out.WriteLine($"***** Circuit Breaker Status Changed ****: { statusDesc }");

        Console.BackgroundColor = previousBackgroundColor;
        Console.ForegroundColor = previousForegroundColor; 
    }
    
    #endregion
}