using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using StackExchange.Redis;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using APIContagem.Models;
using APIContagem.Logging;

namespace APIContagem.Controllers;

[ApiController]
[Route("[controller]")]
public class ContadorController : ControllerBase
{
    private readonly ILogger<ContadorController> _logger;
    private readonly ConnectionMultiplexer _connectionRedis;
    private readonly TelemetryConfiguration _telemetryConfig;

    public ContadorController(ILogger<ContadorController> logger,
        ConnectionMultiplexer connectionRedis,
        TelemetryConfiguration telemetryConfig)
    {
        _logger = logger;
        _connectionRedis = connectionRedis;
        _telemetryConfig = telemetryConfig;
    }

    [HttpGet]
    public ResultadoContador Get()
    {
        DateTimeOffset inicio = DateTime.Now;
        var watch = new Stopwatch();
        watch.Start();

        var valorAtualContador =
            (int)_connectionRedis.GetDatabase().StringIncrement("APIContagem");;
        
        watch.Stop();

        var client = new TelemetryClient(_telemetryConfig);
        client.TrackDependency(
            "Redis", "INCR", $"{nameof(valorAtualContador)} = {valorAtualContador}",
            inicio, watch.Elapsed, true);

        _logger.LogValorAtual(valorAtualContador);

        return new ()
        {
            ValorAtual = valorAtualContador,
            Producer = ContagemInfo.Local,
            Kernel = ContagemInfo.Kernel,
            Framework = ContagemInfo.Framework,
            Mensagem = ContagemInfo.Mensagem
        };
    }
}