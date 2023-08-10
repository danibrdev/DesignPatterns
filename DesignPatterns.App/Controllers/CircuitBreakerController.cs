#region

using CircuitBreaker.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace CircuitBreaker.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CircuitBreakerController : Controller
{
    private readonly ICircuitBreakerService _circuitBreakerService;
    private readonly ICircuitBreakerService2 _circuitBreakerService2;

    public CircuitBreakerController(ICircuitBreakerService circuitBreakerService, ICircuitBreakerService2 circuitBreakerService2)
    {
        _circuitBreakerService = circuitBreakerService;
        _circuitBreakerService2 = circuitBreakerService2;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        await _circuitBreakerService.DoSomething(cancellationToken);
            
        return Ok(Task.CompletedTask);
    }
    
    [HttpGet("2")]
    public async Task<IActionResult> Get2(CancellationToken cancellationToken)
    {
        await _circuitBreakerService2.DoSomething(cancellationToken);
            
        return Ok(Task.CompletedTask);
    }
}