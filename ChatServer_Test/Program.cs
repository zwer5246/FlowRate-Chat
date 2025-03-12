global using Serilog;
global using WebSocketSharp;
global using WebSocketSharp.Server;
using ChatServer_Test.Classes;

namespace ChatServer_Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Is(Serilog.Events.LogEventLevel.Debug)
                .WriteTo.Console()
                .CreateLogger();

            logger.Information("Server starting...");

            DataBaseClass dataBaseInstance = new DataBaseClass(logger);
            if (!await dataBaseInstance.ConnectToDataBase())
            {
                logger.Information("Server stoped due fatal error.");
                System.Environment.Exit(0);
            }

            WebSocketServer wsServer = new WebSocketServer("ws://127.0.0.1:8082");
            wsServer.AddWebSocketService("/TestServer", () => new TestServer(logger, dataBaseInstance));
            wsServer.Start();
            logger.Information("WebSocket waiting for connections on " + wsServer.Address.ToString() + ":" + wsServer.Port.ToString());
            logger.Information("Server loading done.");
            Console.ReadKey();
            wsServer.Stop();
        }
    }
}
