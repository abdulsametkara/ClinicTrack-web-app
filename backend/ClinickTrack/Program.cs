using ClinickCore;
using ClinickCore.Entities;
using ClinickDataAccess;
using ClinickDataAccess.Repository;
using ClinickService.Interfaces;
using ClinickService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Clinick Track API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddDbContext<DatabaseBaglanti>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Servis implamantosyonları
builder.Services.AddScoped<IGenericRepository<Doktor>, GenericRepository<Doktor>>();
builder.Services.AddScoped<IGenericRepository<Uzmanlık>, GenericRepository<Uzmanlık>>();
builder.Services.AddScoped<IGenericRepository<Randevu>, GenericRepository<Randevu>>();
builder.Services.AddScoped<IGenericRepository<Hasta>, GenericRepository<Hasta>>();
builder.Services.AddScoped<IGenericRepository<Kullanıcı>, GenericRepository<Kullanıcı>>();
builder.Services.AddScoped<IDoktorService, DoktorService>();
builder.Services.AddScoped<IUzmanlıkService, UzmanlıkService>();
builder.Services.AddScoped<IRandevuService, RandevuService>();
builder.Services.AddScoped<IHastaService, HastaService>();
builder.Services.AddScoped<IKullanıcıService, KullanıcıService>();

// --- CORS AYARI GÜNCELLENDİ ---
// Her yerden gelen isteğe izin verecek şekilde (AllowAll) değiştirdik.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

//JWT Yapılandırması
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}


// --- CORS MIDDLEWARE GÜNCELLENDİ ---
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Integration test için gerekli
public partial class Program { }