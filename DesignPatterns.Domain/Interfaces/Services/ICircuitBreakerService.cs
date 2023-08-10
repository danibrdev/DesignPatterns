namespace CircuitBreaker.Domain.Interfaces.Services;

public interface ICircuitBreakerService
{
    Task DoSomething(CancellationToken cancellationToken);
}