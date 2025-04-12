using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();                     // Add services to the container.
builder.Services.AddSignalR();                                  // Add services to the container.
builder.Services.AddHostedService<KafkaConsumerService>();      // Add Kafka Consumer service

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
else {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapHub<KafkaHub>("/kafkahub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapFallbackToFile("index.html"); // Serves index.html if no route matches
});

app.Run();