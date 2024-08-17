using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class TranslateHub : Hub
{
    public async Task Translate(string query)
    {
        await Clients.Client(Context.ConnectionId).SendAsync("UpdateTranslationResults",Guid.NewGuid().ToString());
    }
}