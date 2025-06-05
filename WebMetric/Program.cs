using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddPrometheusExporter();

        metrics.AddMeter("Microsoft.AspNetCore.Hosting",
                         "Microsoft.AspNetCore.Server.Kestrel");

        metrics.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                       0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
            });

        // Adiciona seu Meter customizado
        metrics.AddMeter("WebMetric.App");
    });

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

// Criação do contador customizado
var meter = new Meter("WebMetric.App", "1.0.0");
var requestCounter = meter.CreateCounter<long>("app.request_counter");

// Endpoint que incrementa a métrica
app.MapGet("/track", () =>
{
    requestCounter.Add(1);
    return Results.Ok("Contador incrementado!");
});

app.MapGet("/", () => "Hello OpenTelemetry! ticks: " + DateTime.Now.Ticks.ToString()[^3..]);

app.Run();
