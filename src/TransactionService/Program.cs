using Microsoft.EntityFrameworkCore;
using TransactionService.Services;
using TransactionService.Data;
using AntiFraudService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<AntiFraudService.Services.AntiFraudService>();
builder.Services.AddHostedService<TransactionStatusConsumer>();
builder.Services.AddScoped<ITransactionService, TransactionService.Services.TransactionService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();