using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Api.Repositories;
using TechMove.Api.Services;
using TechMove.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

var firebaseServiceAccountPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "firebase-service-account.json");

if (File.Exists(firebaseServiceAccountPath))
{
    try
    {
        FirebaseApp.GetInstance("[DEFAULT]");
    }
    catch
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(firebaseServiceAccountPath)
        });
    }
}

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

builder.Services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            dbContext.Database.Migrate();
            break;
        }
        catch
        {
            if (attempt == maxRetries)
            {
                throw;
            }

            Thread.Sleep(delay);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI(); 

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowMvcClient");

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }