using Microsoft.EntityFrameworkCore;
using EmotionAnalyzeApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Circular reference sorununu çöz
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Add SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpClient
builder.Services.AddHttpClient();

// Add CORS - Production için özel yapılandırma
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("*");
    });
    
    // Development için ayrı policy
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    
    // Production için spesifik policy
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
                "https://emotion-analyze-app.vercel.app",
                "https://emotion-analyze-qcwihl6ia-s3limms-projects.vercel.app",
                "https://*.vercel.app"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .WithExposedHeaders("*");
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Production'da da Swagger'ı aktif et (API test için)
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Render için HTTP'yi kabul et
// app.UseHttpsRedirection();

// CORS middleware'i ekle (UseAuthorization'dan önce olmalı!)
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}
else
{
    // Production'da spesifik Vercel URL'lerine izin ver
    app.UseCors("Production");
}
app.UseAuthorization();
app.MapControllers();

// Migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();