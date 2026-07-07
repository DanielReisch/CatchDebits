using CatchDebits.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// BANCO DE DADOS: SQLite via Entity Framework Core
// Lê a connection string do appsettings.json
// ============================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// CORS: permite que o front-end React (porta 5173) acesse a API
// ============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "CatchDebits API",
        Version = "v1",
        Description = "API de gerenciamento financeiro por pessoa."
    });
});

var app = builder.Build();

// ============================================================
// INICIALIZAÇÃO DO BANCO DE DADOS
// EnsureCreated(): cria o arquivo .db e as tabelas se não existirem.
// Se o arquivo já existir, não faz nada (não perde dados).
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS deve vir ANTES de MapControllers
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();