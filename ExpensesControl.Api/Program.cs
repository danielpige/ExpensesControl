using ExpensesControl.Api.DependencyInjection;
using ExpensesControl.Api.Extensions;
using ExpensesControl.Application.DependencyInjection;
using ExpensesControl.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

var app = builder.Build();

app.UseApiPipeline();

app.Run();
