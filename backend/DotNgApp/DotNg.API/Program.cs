var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();

//Настраиваем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Разрешить запросы с любых источников
              .AllowAnyMethod()   // Разрешить все методы (GET, POST, PUT, DELETE и т. д.)
              .AllowAnyHeader();  // Разрешить любые заголовки
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Включаем CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();