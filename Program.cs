using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

namespace ds_challenge_05
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

            app.UseAuthorization();

            // Map the attribute-based controllers.
            app.MapControllers();

            // Minimal API endpoint for checking JSON.
            app.MapPost("/api/check", async (HttpContext context) =>
            {
                // Deserialize the JSON from the request body into a CheckRequest object.
                var request = await context.Request.ReadFromJsonAsync<CheckRequest>();

                // Validate that the request body exists and that the "amount" property is provided.
                if (request == null || !request.Amount.HasValue)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new { description = "'amount' key missing in JSON body" });
                    return;
                }

                double amt = request.Amount.Value;

                // If the amount is less than 25000, return an error.
                if (amt < 25000)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new { description = "Invalid" });
                }
                else
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsJsonAsync(new { message = "Valid", data = request });
                }
            });

            app.Run();
        }
    }

    // Model used for deserializing the JSON data.
    public class CheckRequest
    {
        [JsonPropertyName("amount")]
        public double? Amount { get; set; }
    }
}
