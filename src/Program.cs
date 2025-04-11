using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace ds_challenge_04
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers and OpenAPI services.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseHttpsRedirection();
            }
            else
            {
                // In production, you may choose to skip HTTPS redirection
                // or configure it properly if you have an SSL certificate set up.
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Minimal API: map a GET endpoint at the root ("/")
            // Define the translations dictionary.
            var translations = new Dictionary<string, string>
            {
                { "EN", "Hello World!" },
                { "SP", "¡Hola Mundo!" },
                { "FR", "Bonjour le monde!" },
                { "DE", "Hallo Welt!" },
                { "IT", "Ciao Mondo!" },
                { "SW", "Hej världen!" }
            };

            app.MapGet("/", (HttpContext context) =>
            {
                // Get the COUNTRY environment variable.
                var country = Environment.GetEnvironmentVariable("COUNTRY");

                // If COUNTRY is defined and is exactly 2 letters...
                if (!string.IsNullOrEmpty(country) && country.Length == 2)
                {
                    var key = country.ToUpper();
                    if (translations.TryGetValue(key, out var translation))
                    {
                        // Return the translation.
                        return Results.Text(translation);
                    }
                    else
                    {
                        // Return a 404 if translation is not available.
                        return Results.Text("Translation not available for this country.", statusCode: 404);
                    }
                }

                // Default response.
                return Results.Text("Hello World!");
            });

            // Map the attribute-based controllers.
            app.MapControllers();

            app.Run();
        }
    }
}
