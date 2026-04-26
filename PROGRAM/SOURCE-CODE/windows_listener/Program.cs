using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

class Program
{
    static readonly string firebaseUrl = "https://remotemessenger-f9059-default-rtdb.asia-southeast1.firebasedatabase.app/commands.json";

    static async Task Main()
    {
        Console.WriteLine("🔵 Windows Shutdown Listener Started...");
        Console.WriteLine("Waiting for command from Firebase...\n");

        HttpClient client = new HttpClient();

        while (true)
        {
            try
            {
                var response = await client.GetAsync(firebaseUrl);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[{DateTime.Now}] Firebase Response: {result}");

                var clean = result.Trim().Replace("\"", "");

                if (clean == "shutdown")
                {
                    Console.WriteLine("⚠️ Shutdown command received!");

                    // 🔥 Clear BEFORE shutdown
                    await ClearCommand(client);

                    ShutdownPC();
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            await Task.Delay(5000);
        }
    }

    static async Task ClearCommand(HttpClient client)
    {
        var content = new StringContent("null", Encoding.UTF8, "application/json");
        await client.PutAsync(firebaseUrl, content);

        Console.WriteLine("🧹 Command cleared from Firebase");
    }

    static void ShutdownPC()
    {
        Console.WriteLine("💻 Shutting down...");

        Process.Start(new ProcessStartInfo
        {
            FileName = "shutdown",
            Arguments = "/s /t 0",
            CreateNoWindow = true,
            UseShellExecute = false
        });
    }
}