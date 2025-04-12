using Microsoft.AspNetCore.SignalR;
public class KafkaHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("SignalR connection established.");
        return base.OnConnectedAsync();
    }


    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}
