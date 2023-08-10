namespace CircuitBreaker.Domain.Interfaces.Services;

public interface ICircuitBreakerService2
{
    Task DoSomething(CancellationToken cancellationToken);
}