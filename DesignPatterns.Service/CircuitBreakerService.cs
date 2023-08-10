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

public class CircuitBreakerService : ICircuitBreakerService
{
    private readonly ILogger<CircuitBreakerService> _logger;
    private readonly HttpClient _httpClient;

    public CircuitBreakerService(
        ILogger<CircuitBreakerService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task DoSomething(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _httpClient.GetAsync("http://localhost:5268/WeatherForecast", cancellationToken);
                
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(
                    $"# {DateTime.Now:HH:mm:ss} # "+
                    $"Falha ao invocar a API: {ex.GetType().FullName} | {ex.Message}");
            }
        }
    }
}