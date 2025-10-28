using Microsoft.EntityFrameworkCore;
using ProductManagement.Application;
using ProductManagement.Infrastructure;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ProductManagement.API.Filters.AuthHeaderFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Management API", Version = "v1" });
    c.AddSecurityDefinition("X-Auth-Token", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Auth-Token",
        Type = SecuritySchemeType.ApiKey,
        Description = "أدخل القيمة: static_secret"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-Auth-Token"
                }
            }, new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductManagement.Infrastructure.Persistence.ApplicationDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ProductManagement.API.Middlewares.GlobalExceptionMiddleware>();

app.UseCors("AllowWeb");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
