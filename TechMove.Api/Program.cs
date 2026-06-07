using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Api.Repositories;
using TechMove.Api.Services;
using TechMove.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<CurrencyApiService>();

builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<ContractValidationService>();

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientApiService, ClientApiService>();

builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractApiService, ContractApiService>();

builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IServiceRequestApiService, ServiceRequestApiService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcClient", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); 

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowMvcClient");

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }