using CatchDebits.API.Data;
using CatchDebits.API.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// BANCO DE DADOS
// ============================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// INJEÇÃO DE DEPENDÊNCIA DOS SERVICES
// AddScoped: uma instância por requisição HTTP
// ============================================================
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

// ============================================================
// FLUENTVALIDATION
// Registra automaticamente todos os Validators do projeto
// ============================================================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ============================================================
// CORS: libera o front-end React (porta 5173)
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
// INICIALIZAÇÃO DO BANCO
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

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();