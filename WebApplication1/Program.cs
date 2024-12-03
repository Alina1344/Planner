using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер зависимостей
builder.Services.AddControllers(); // Добавляем поддержку контроллеров

// Регистрация хранилищ (DI)
builder.Services.AddScoped<IUserStorage, UserStorage>();
builder.Services.AddScoped<ITodoStorage, TodoStorage>();
builder.Services.AddScoped<ITodoListStorage, TodoListStorage>();

// Настройка Swagger для документации API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Настройка конвейера обработки запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Добавление маршрутизации для контроллеров
app.UseRouting();

app.UseAuthorization();

app.MapControllers(); // Подключаем маршруты для всех контроллеров

app.Run();