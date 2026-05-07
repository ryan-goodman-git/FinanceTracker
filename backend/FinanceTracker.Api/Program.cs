using FinanceTracker.Application;
using FinanceTracker.Infrastructure;
using FinanceTracker.Api.Endpoints.Users;
using FinanceTracker.Api.Endpoints.OneOffTransactions;
using FinanceTracker.Api.Endpoints.RecurringTransactions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");
app.MapUserEndpoints();
app.MapOneOffTransactionEndpoints();
app.MapRecurringTransactionEndpoints();

app.Run();