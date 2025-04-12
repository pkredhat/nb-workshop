using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)  // ✅ Change Main to async
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run <produce|consume>");
            return;
        }

        if (args[0] == "produce")
        {
            await Producer.Run();  // ✅ Await the async method
        }
        else if (args[0] == "consume")
        {
            Consumer.Run();
        }
        else
        {
            Console.WriteLine("Invalid argument. Use 'produce' or 'consume'.");
        }
    }
} 