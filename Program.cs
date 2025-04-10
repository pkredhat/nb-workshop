using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
string? countryCode = Environment.GetEnvironmentVariable("COUNTRY_CODE") ?? "en";

Console.WriteLine($"Running on PORT: {port}");
Console.WriteLine($"countryCode: {countryCode}");


app.MapGet("/api/version", async (HttpContext context) =>
{
    await context.Response.WriteAsync("0.0.1");
}); 

app.MapGet("/api/health", () =>
{
    try
    {
        return Results.Ok("OK");
    }
    catch
    {
        return Results.Problem("Service Unhealthy", statusCode: 500);
    }
});

// Admin panel endpoint
app.MapGet("/api/admin", async (HttpContext context, IConfiguration config, HttpClient http) =>
{
    var password = context.Request.Query["password"].ToString();

    if (password != "opensesame")
    {
        return Results.StatusCode(403);
    }

    var adminUrl = config["ADMIN"];
    if (string.IsNullOrEmpty(adminUrl))
    {
        return Results.Problem("ADMIN env variable not set");
    }

    try
    {
        var response = await http.GetAsync(adminUrl);
        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("There was a problem calling the API, please review your parameters");
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonString);
        var formattedJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });

        return Results.Content($"<pre>{formattedJson}</pre>", "text/html");
    }
    catch
    {
        return Results.Problem("Internal server error");
    }
});


app.MapGet("/", async (HttpContext context) =>
{
    if (string.IsNullOrEmpty(countryCode))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Country code parameter is required.");
        return;
    }
    try
    {
        string json = await File.ReadAllTextAsync("translations.json");
        // Deserialize into a dictionary that includes "translations"
        var jsonObject = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

        if (jsonObject != null && jsonObject.TryGetValue("translations", out var translations))
        {
            if (translations.TryGetValue(countryCode.ToUpper(), out var translation))
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(translation);
            }
            else
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Translation not found for the specified country code.");
            }
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Invalid JSON format.");
        }
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"Error: {ex.Message}");
    }
});

app.Run($"http://0.0.0.0:{port}");