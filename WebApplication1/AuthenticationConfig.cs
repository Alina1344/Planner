using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WebApplication1;

// В AuthenticationConfig.cs
public static class AuthenticationConfig
{
    public static void AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        // Конфигурация аутентификации
        services.AddAuthentication("BasicAuth")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuth", null);

        // Настройка авторизации
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });
    }
}


public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ILoggerFactory _loggerFactory;

    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, loggerFactory, encoder)
    {
        _loggerFactory = loggerFactory; // Сохраняем логгер фабрику
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Проверяем наличие заголовка Authorization
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            _loggerFactory.CreateLogger<BasicAuthenticationHandler>().LogWarning("Authorization header is missing.");
            return Task.FromResult(AuthenticateResult.Fail("Отсутствует заголовок Authorization"));
        }

        try
        {
            // Получаем и декодируем заголовок
            var authHeader = Request.Headers["Authorization"].ToString();
            _loggerFactory.CreateLogger<BasicAuthenticationHandler>().LogInformation($"Authorization header: {authHeader}");

            // Проверка на корректность формата заголовка
            if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                _loggerFactory.CreateLogger<BasicAuthenticationHandler>().LogWarning("Invalid authorization header format.");
                return Task.FromResult(AuthenticateResult.Fail("Неверный формат заголовка Authorization"));
            }

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Substring(6))).Split(':');
            if (credentials.Length != 2)
            {
                _loggerFactory.CreateLogger<BasicAuthenticationHandler>().LogWarning("Invalid authorization header content.");
                return Task.FromResult(AuthenticateResult.Fail("Неверное содержимое заголовка Authorization"));
            }

            var username = credentials[0];
            var password = credentials[1];

            // Логирование для проверки username и password
            _loggerFactory.CreateLogger<BasicAuthenticationHandler>().LogInformation($"Username: {username}, Password: {password}");

            // Простая проверка пользователя (замените на вашу логику аутентификации)
            if (username == "admin" && password == "password")
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, "Admin") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            _loggerFactory.CreateLogger<BasicAuthenticationHandler>().LogWarning("Invalid credentials.");
            return Task.FromResult(AuthenticateResult.Fail("Неверные учетные данные"));
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            var logger = _loggerFactory.CreateLogger<BasicAuthenticationHandler>();
            logger.LogError(ex, "Ошибка обработки заголовка Authorization");
            return Task.FromResult(AuthenticateResult.Fail("Ошибка обработки заголовка Authorization"));
        }
    }
}


