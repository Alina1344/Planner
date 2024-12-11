using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Storage;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер зависимостей
builder.Services.AddControllers(); // Добавляем поддержку контроллеров
builder.Services.AddLogging(); // Добавление логирования

// Регистрация хранилищ (DI)
builder.Services.AddScoped<IUserStorage, UserStorage>();
builder.Services.AddScoped<ITodoStorage, TodoStorage>();
builder.Services.AddScoped<ITodoListStorage, TodoListStorage>();

// Настройка аутентификации для Basic Auth
builder.Services.AddAuthentication("BasicAuth")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuth", options => { });

// Настройка авторизации
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BasicAuth", policy => policy.RequireAuthenticatedUser());
});

// Добавляем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Настройка Swagger для документации API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Добавляем поддержку Basic Authentication в Swagger
    c.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Basic Authorization header using the username:password"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BasicAuth"
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Настройка конвейера обработки запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Подключаем CORS перед маршрутизацией
app.UseCors("AllowAll");

app.UseRouting(); // Настраиваем маршрутизацию

// Подключаем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

// Добавление маршрутизации для контроллеров
app.MapControllers(); // Подключаем маршруты для всех контроллеров

app.Run();
