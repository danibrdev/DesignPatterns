#region

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CircuitBreaker.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

#endregion

namespace CircuitBreaker.Service;

public class CircuitBreakerService2 : ICircuitBreakerService2
{
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILogger<CircuitBreakerService2> _logger;

    public CircuitBreakerService2(
        AsyncCircuitBreakerPolicy circuitBreaker,
        ILogger<CircuitBreakerService2> logger)
    {
        _circuitBreaker  = circuitBreaker;
        _logger = logger;
    }
    
    public async Task DoSomething(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var resultado = 
                    await _circuitBreaker.ExecuteAsync(() 
                        => new HttpClient().GetAsync("http://localhost:5268/WeatherForecast", cancellationToken));

                _logger.LogInformation($"* {DateTime.Now:HH:mm:ss} * Circuito = {{_circuitBreaker.CircuitState}} ");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"# {DateTime.Now:HH:mm:ss} # "+ 
                           $"Circuito = {_circuitBreaker.CircuitState} | " +
                           $"Falha ao invocar a API: {ex.GetType().FullName} | {ex.Message}");
            }
        }
    }
}