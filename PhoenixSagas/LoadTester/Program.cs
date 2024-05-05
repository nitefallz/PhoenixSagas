using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Text;

public class Program
{
    private static List<TcpLoadTester> testers = new List<TcpLoadTester>();
    private static CancellationTokenSource cts = new CancellationTokenSource();

    public static async Task Main()
    {
        // Set up Ctrl-C handler to initiate graceful shutdown and reporting
        Console.CancelKeyPress += async (sender, e) =>
        {
            e.Cancel = true;  // Prevent the process from terminating immediately.
            Console.WriteLine("\nCtrl-C pressed. Ending tests and generating report...");

            cts.Cancel();  // Signal cancellation to all connections

            // Wait for all tasks to complete their current work
            foreach (var tester in testers)
            {
                await tester.CompleteCurrentWork();
            }

            // Generate and display the report
            foreach (var tester in testers)
            {
                var stats = tester.GetStatistics();
                Console.WriteLine($"Client {testers.IndexOf(tester)}: Min RTT={stats.min} ms, Max RTT={stats.max} ms, Average RTT={stats.average} ms");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        };


        int maxConnections = 100;
        string host = "localhost";
        int port = 4000;

        // Start all connections
        for (int i = 0; i < maxConnections; i++)
        {
            var tester = new TcpLoadTester(host, port, i);
            testers.Add(tester);
            tester.StartAsync(cts.Token).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Console.WriteLine($"Client {i} failed: {t.Exception.InnerException.Message}");
                }
            });
            await Task.Delay(500);  // Stagger connection start times to avoid surge
        }

        // Prevent main from exiting by waiting on the cancellation token indefinitely
        try
        {
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            // Expected when Ctrl-C is pressed and cancellation is requested
            Console.WriteLine("Shutdown initiated...");
        }
        foreach (var tester in testers)
        {
            var stats = tester.GetStatistics();
            Console.WriteLine($"Client {testers.IndexOf(tester)}: Min RTT={stats.min} ms, Max RTT={stats.max} ms, Average RTT={stats.average} ms");
        }
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
public class TcpLoadTester
{
    private readonly string _host;
    private readonly int _port;
    private readonly int _id;
    private readonly TcpClient _client = new TcpClient();
    private readonly ConcurrentBag<double> _responseTimes = new ConcurrentBag<double>();
    private Task communicationTask;

    public TcpLoadTester(string host, int port, int id)
    {
        _host = host;
        _port = port;
        _id = id;
    }

    public async Task StartAsync(CancellationToken token)
    {
        communicationTask = Task.Run(async () =>
        {
            try
            {
                await _client.ConnectAsync(_host, _port);
                Console.WriteLine($"Client {_id} connected.");
                var stream = _client.GetStream();

                while (!token.IsCancellationRequested)
                {
                    string messageID = Guid.NewGuid().ToString();
                    var message = Encoding.UTF8.GetBytes($"Message from client {_id}: {messageID}");
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    await stream.WriteAsync(message, 0, message.Length, token);

                    var buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), token);
                    stopwatch.Stop();
                    if (bytesRead > 0)
                    {
                        _responseTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
                    }

                    await Task.Delay(1000, token);  // Throttle message rate
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client {_id} error: {ex.Message}");
            }
            finally
            {
                _client.Close();
                Console.WriteLine($"Client {_id} disconnected.");
            }
        }, token);

        await communicationTask;  // Ensure the task is awaitable
    }

    public async Task CompleteCurrentWork()
    {
        // Ensures the communication task is completed before shutting down
        if (communicationTask != null && !communicationTask.IsCompleted)
        {
            try
            {
                await communicationTask;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Client {_id} cancellation completed.");
            }
        }
    }

    public (double min, double max, double average) GetStatistics()
    {
        double min = _responseTimes.Any() ? _responseTimes.Min() : 0;
        double max = _responseTimes.Any() ? _responseTimes.Max() : 0;
        double average = _responseTimes.Any() ? _responseTimes.Average() : 0;
        return (min, max, average);
    }
}
