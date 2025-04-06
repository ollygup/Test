using FluentValidation;
using log4net.Config;
using Test.Application.DTO.Item;
using Test.Application.DTO.Transaction;
using Test.Application.Services;
using Test.Application.Services.IServices;
using Test.Application.Validator.Item;
using Test.Application.Validator.Transaction;
using Test.Core.IRepository;
using Test.Infrastructure.Logging;
using Test.Infrastructure.Middleware;
using Test.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Log4Net
Logger.ConfigureLogging();

builder.Services.AddTransient<RequestLogMiddleware>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Validators IOC
builder.Services.AddScoped<IValidator<TransactionInputDTO>, TransactionInputDTOValidator>();
builder.Services.AddScoped<IValidator<ItemDTO>, ItemDTOValidator>();

// Services IOC
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Repositories IOC
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();

var app = builder.Build();

app.UseMiddleware<RequestLogMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
