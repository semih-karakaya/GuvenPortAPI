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
builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opt =>
  {
      opt.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
                                      Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
          //ClockSkew = TimeSpan.Zero
      };
      // Eğer token'ı cookie içinde koyduysanız bu kısmı ekleyin
      opt.Events = new JwtBearerEvents
      {
          OnMessageReceived = ctx =>
          {
              if (ctx.Request.Cookies.ContainsKey("authToken"))
                  ctx.Token = ctx.Request.Cookies["authToken"];
              return Task.CompletedTask;
          }
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



builder.Services.AddDbContext<isgportalContext>((sp, options) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var httpCtx = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var user = httpCtx?.User;

    // Burada illa kullanıcı authenticate edilmişse is_doctor claim’ine bakalım
    var isDoctor = user?
      .Claims
      .Any(c => c.Type == "is_doctor" && c.Value == "true")
      ?? false;

    var csName = isDoctor
      ? "DoctorFullAccessConnection"
      : "StaffReadOnlyConnection";

    // Debug için konsola yazdırıyoruz
    Console.WriteLine($"[DbContext] is_doctor = {isDoctor}. Using connection='{csName}'");

    var connStr = cfg.GetConnectionString(csName);
    options.UseNpgsql(connStr);

    // Eğer dilerseniz SQL ve Connection logunu bu şekilde açabilirsiniz:
    // options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
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
