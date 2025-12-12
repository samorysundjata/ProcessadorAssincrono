using FluentValidation;
using ProcessadorAssincrono.Application.Interfaces;
using ProcessadorAssincrono.Application.Validators;
using ProcessadorAssincrono.Domain.DTOs;
using ProcessadorAssincrono.Domain.Entities;
using ProcessadorAssincrono.Infrastructure.BackgroundServices;
using ProcessadorAssincrono.Infrastructure.Data;
using ProcessadorAssincrono.Infrastructure.Interfaces;
using ProcessadorAssincrono.Infrastructure.Persistence;
using ProcessadorAssincrono.Infrastructure.Services;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAprovacaoService, AprovacaoService>();
builder.Services.AddSingleton<IDbConnectionFactory, DapperContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton(Channel.CreateUnbounded<Aprovacao>());

builder.Services.AddSingleton<ProcessadorQueueService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ProcessadorQueueService>());
builder.Services.AddSingleton<IProcessadorQueue>(sp => sp.GetRequiredService<ProcessadorQueueService>());

builder.Services.AddScoped<IValidator<AprovacaoRequest>, AprovacaoRequestValidator>();
builder.Services.AddScoped<IValidator<LoteAprovacaoRequest>, LoteAprovacaoRequestValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPut("/api/diarias/{id:guid}/inserir", async (
    IAprovacaoService aprovacaoService,
    Guid id,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    var aprovacao = new Aprovacao
    {
        Id = id,
        Pep = "123456",
        ComentariosAdicionais = "Comentário de teste",
        DataAprovacao = DateTime.UtcNow
    };

    await aprovacaoService.InserirAsync(aprovacao);
    logger.LogInformation("Diária {Id} inserida com sucesso.", id);

    return Results.Created($"/api/diarias/{id}", new
    {
        mensagem = $"Diária {id} inserida com sucesso."
    });
});

app.MapPut("/api/diarias/{id:guid}/aprovar", async (
    Guid id,
    AprovacaoRequest request,
    IProcessadorQueue queue,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    await queue.EnfileirarAsync(id, request.Pep, request.ComentariosAdicionais, DateTime.UtcNow);
    logger.LogInformation("Diária {Id} enfileirada para aprovação.", id);

    return Results.Accepted($"/api/diarias/{id}/aprovar", new
    {
        mensagem = $"Diária {id} enfileirada para aprovação."
    });
});

app.MapPost("/aprovar-em-lote", async (
    LoteAprovacaoRequest request,
    IValidator<LoteAprovacaoRequest> validator,
    IProcessadorQueue queue,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    var validationResult = await validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(new
        {
            erros = validationResult.Errors.Select(e => e.ErrorMessage)
        });
    }

    foreach (var id in request.Solicitacoes)
    {
        await queue.EnfileirarAsync(id, "PEP_PADRAO", "Aprovação em lote", DateTime.UtcNow);
        logger.LogInformation("Diária {Id} enfileirada para aprovação em lote.", id);
    }

    return Results.Accepted("/aprovar-em-lote", new
    {
        mensagem = $"{request.Solicitacoes.Count} solicitações enfileiradas para aprovação."
    });
});

app.Run();