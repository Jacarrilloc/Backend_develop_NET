var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5294);
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("SecurityLogger");

// Middleware: HTTPS simulado, autenticación y validación
app.Use(async (context, next) =>
{
    var request = context.Request;

    // Simular HTTPS
    var isSecure = request.Query.TryGetValue("secure", out var secureVal) && secureVal == "true";
    if (!isSecure)
    {
        logger.LogWarning("Simulated HTTPS Required - {Path}", request.Path);
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Simulated HTTPS Required");
        return;
    }

    // Ruta especial para simular acceso no autorizado
    if (request.Path == "/unauthorized")
    {
        logger.LogWarning("Unauthorized access attempt to {Path}", request.Path);
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized Access");
        return;
    }

    // Validación de entrada peligrosa
    if (request.Query.TryGetValue("input", out var inputVal))
    {
        var unsafeInput = inputVal.ToString();
        if (unsafeInput.Contains("<script>", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Input validation failed: {Input}", unsafeInput);
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid Input");
            return;
        }
    }

    // Autenticación (parámetro de query en vez de header)
    var isAuthenticated = request.Query.TryGetValue("authenticated", out var authVal) && authVal == "true";
    if (!isAuthenticated)
    {
        logger.LogWarning("Access denied due to missing authentication.");
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access Denied");
        return;
    }

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ruta raíz que simula respuesta asíncrona
app.MapGet("/", async (HttpContext context) =>
{
    var response = context.Response;
    response.ContentType = "text/plain";

    await response.WriteAsync("Processed Asynchronously\n");
    await Task.Delay(500); // Simulación de procesamiento asincrónico
    await response.WriteAsync("Final Response from Application");
});

app.Run();
