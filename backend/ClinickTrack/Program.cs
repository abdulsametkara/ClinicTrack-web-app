using ClinickCore;
using ClinickCore.Entities;
using ClinickDataAccess;
using ClinickDataAccess.Repository;
using ClinickService.Interfaces;
using ClinickService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseBaglanti>();

//Servis implamantosyonları
builder.Services.AddScoped<IGenericRepository<Doktor>, GenericRepository<Doktor>>();
builder.Services.AddScoped<IGenericRepository<Uzmanlık>, GenericRepository<Uzmanlık>>();
builder.Services.AddScoped<IGenericRepository<Randevu>, GenericRepository<Randevu>>();
builder.Services.AddScoped<IDoktorService, DoktorService>();
builder.Services.AddScoped<IUzmanlıkService, UzmanlıkService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
