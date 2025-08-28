using AutoMapper;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDoListApi.Entities;
using ToDoListApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRouting(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

builder.Services.AddAutoMapper(config =>
{
    config.CreateMap<Status, StatusDTO>();
    config.CreateMap<StatusDTO, Status>();
    config.CreateMap<Page, PageDTO>();
    config.CreateMap<PageDTO, Page>();
});

builder.Services.AddDbContext<ToDoListApi.Data.ToDoDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddLogging();

builder.Services.AddHttpLogging(options => 
{
    options.LoggingFields = HttpLoggingFields.All;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
