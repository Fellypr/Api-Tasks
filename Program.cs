var builder = WebApplication.CreateBuilder(args);

// Adiciona política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173","https://my-task-list-ten.vercel.app/")
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.AddControllers();
builder.Configuration.AddEnvironmentVariables();


var app = builder.Build();

// Usa a política de CORS
app.UseCors("PermitirFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();