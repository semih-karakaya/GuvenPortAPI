using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;
using GuvenPortAPI.Service;
using GuvenPortAPI.Services.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SwaggerThemes;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// Context registration



builder.Services.AddScopedService();
builder.Services.AddScoped<IOfficeService, OfficeService>(); // Register the service with the DI container
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:44387") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
        policy.WithOrigins("https://localhost:5078") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        //ClockSkew = TimeSpan.Zero
    };
});

// Yetkilendirme Servislerini Ekleme (bu kısım kalacak, [Authorize] attribute'ları için)
builder.Services.AddAuthorization(options =>
{
    // Global Yetkilendirme Politikası: AllowAnonymous olmayan tüm endpoint'ler kimlik doğrulaması gerektirir.
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});



builder.Services.AddDbContext<isgportalContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

    string connectionString;

    var user = httpContextAccessor.HttpContext?.User;

    // Kullanıcı kimliği doğrulanmış mı kontrol et
    if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
    {
        // ÖZEL CLAIM OLUŞTURUYORUZ: 'is_doctor' adında bir Claim arayacağız
        // ClaimTypes.Role yerine kendi özel claim'imizi arayacağız.
        var isDoctorClaim = user.Claims.FirstOrDefault(c => c.Type == "is_doctor");

        if (isDoctorClaim != null && isDoctorClaim.Value.ToLower() == "true")
        {
            connectionString = configuration.GetConnectionString("DoctorFullAccessConnection");
        }
        else // is_doctor claim'i yoksa, false ise, veya diğer durumlarda Staff Connection String'i kullanılsın
        {
            connectionString = configuration.GetConnectionString("StaffReadOnlyConnection");
        }
    }
    else
    {
        // Anonim kullanıcılar için (örneğin AuthController'daki login metodu için)
        // StaffReadOnlyConnection'ı kullanalım. Neden IdentityConnection'a ihtiyaç duymuyoruz?
        // Çünkü artık Identity tabloları yok, sadece kendi staff tablolarımız var.
        connectionString = configuration.GetConnectionString("StaffReadOnlyConnection");
    }

    options.UseNpgsql(connectionString);
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "GuvenPort API", Version = "v1" });

    // JWT Bearer için güvenlik tanımı ekle
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    // Bu güvenlik tanımını tüm operasyonlara uygula
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(Theme.UniversalDark);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
