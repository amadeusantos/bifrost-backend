using Bifrost.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiWithSecurity();
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddDependencyInjection(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwaggerWithUi();

app.UseHandlerException();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
