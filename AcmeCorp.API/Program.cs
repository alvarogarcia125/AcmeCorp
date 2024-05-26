using AcmeCorp.API.Helpers;
using AcmeCorp.Application.Interfaces;
using AcmeCorp.Application.Mappings;
using AcmeCorp.Application.Services;
using AcmeCorp.Infrastructure.Authentication;
using AcmeCorp.Infrastructure.Context;
using AcmeCorp.Infrastructure.Interfaces;
using AcmeCorp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Environment.EnvironmentName.ToLower().Equals("development") 
                     ? builder.Configuration.GetConnectionString("DefaultConnection")
                     : ParameterStoreHelper.RetrieveApiKeyFromParameterStore(builder.Configuration, builder.Environment.EnvironmentName, "ConnectionStringParameterName").GetAwaiter().GetResult();
   
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var apiKey = ParameterStoreHelper.RetrieveApiKeyFromParameterStore(builder.Configuration, builder.Environment.EnvironmentName, "ApiKeyParameterName").GetAwaiter().GetResult();
builder.Services.AddSingleton(new ApiKeyProvider(apiKey));

builder.Services.AddAuthentication("ApiKey")
        .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AcmeCorp API V1");
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

