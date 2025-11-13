using IICUTechServiceReference;
using IWSDLPublishServiceReference;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IICUTech, ICUTechClient>();
builder.Services.AddScoped<IWSDLPublish, WSDLPublishClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("http://localhost", 
                           "https://your-frontend-site.netlify.app") // Пример для Netlify // Замените на ваш URL GitHub Pages или Netlify
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("default");

app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/yandex_7f04207747b87669.html", "yandex_7f04207747b87669.html");
app.MapFallbackToFile("/robots.txt", "robots.txt");
app.MapFallbackToFile("/{path?}", "index.html");

app.Run();
