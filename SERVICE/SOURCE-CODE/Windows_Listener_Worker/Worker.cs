using System.Diagnostics;
using System.Text;

public class Worker : BackgroundService
{
    private readonly string firebaseUrl ="https://remotemessenger-f9059-default-rtdb.asia-southeast1.firebasedatabase.app/commands.json";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        HttpClient client = new HttpClient();

        await Task.Delay(2000, stoppingToken); // 🔥 important: allow service startup time

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await client.GetAsync(firebaseUrl, stoppingToken);
                var result = await response.Content.ReadAsStringAsync();

                var clean = result.Trim().Replace("\"", "");

                if (clean == "shutdown")
                {
                    await ClearCommand(client);
                    Console.WriteLine("SHUTDOWN!");
                    ShutdownPC();
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ClearCommand(HttpClient client)
    {
        var content = new StringContent("null", Encoding.UTF8, "application/json");
        await client.PutAsync(firebaseUrl, content);
    }

    private void ShutdownPC()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "shutdown",
            Arguments = "/s /t 0",
            CreateNoWindow = true,
            UseShellExecute = false
        });
    }
}