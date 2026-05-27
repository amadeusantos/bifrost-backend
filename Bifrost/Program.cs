using Bifrost.Config;
using Bifrost.Core.Adapter;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using Bifrost.Infrastructure.Persistence;
using Bifrost.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddTransient<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddTransient<IAssessmentSeasonRepository, AssessmentSeasonRepository>();
builder.Services.AddScoped<IAssessmentSeasonService, AssessmentSeasonService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
}

app.UseHandlerException();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();