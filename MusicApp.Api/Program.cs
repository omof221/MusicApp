using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicApp.Application.Abstractions;
using MusicApp.Application.Services;
using MusicApp.Infrastructure;
using MusicApp.Infrastructure.ML;
using MusicApp.Infrastructure.OpenAi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ?? 1. Controller ve Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ?? 2. Veritaban? (EF Core)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ?? 3. Infrastructure Extension (Dependency Injection + EF Configuration)
builder.Services.AddInfrastructure(builder.Configuration);

// ?? 4. Authentication (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// ?? 5. Session Servisleri
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ?? 6. Servis Ba??ml?l?klar?
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddSingleton<RecommendationModel>();
builder.Services.AddSingleton<OpenAiService>();
builder.Services.AddScoped<ListeningAnalysisService>();
builder.Services.AddScoped<ArtistService>();

var app = builder.Build();

// ?? 7. HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ?? Authentication ? Authorization s?ras? önemli!
app.UseAuthentication();
app.UseAuthorization();

// ?? Session Middleware aktif et
app.UseSession();

app.MapControllers();

app.Run();
