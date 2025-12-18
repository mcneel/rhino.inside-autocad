using Microsoft.Extensions.Configuration;
using Rhino.Inside.AutoCAD.Core.Interfaces;
using Serilog;
using Serilog.Core;
using System.Text;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="ILoggerService"/>
public class LoggerService : ILoggerService
{
    private static ILoggerService? _instance;
    private static readonly object Lock = new();

    private const string _serilogConfigFileName = ApplicationConstants.LogConfigName;
    private const string _loggerServiceAlreadyInitialized = MessageConstants.LoggerServiceAlreadyInitialized;
    private const string _loggerServiceNotInitialized = MessageConstants.LoggerServiceNotInitialized;

    /// <summary>
    /// Returns the singleton instance of the <see cref="ILogger"/>.
    /// </summary>
    public static ILoggerService Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException(_loggerServiceNotInitialized);
            }

            return _instance;
        }
    }

    /// <summary>
    /// Private constructor for singleton pattern.
    /// </summary>
    private LoggerService()
    { }

    /// <summary>
    /// Constructs a new <see cref="LoggerService"/>.
    /// </summary>
    private LoggerService(IApplicationDirectories applicationDirectories)
    {
        var filePage = Path.Combine(applicationDirectories.Resources,
             _serilogConfigFileName);

        var json = File.ReadAllText(filePage);

        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var escapedAppDataPath = appDataPath.Replace(@"\", @"\\");
        var productName = applicationDirectories.ProductName;
        json = json.Replace("%AppData%", escapedAppDataPath);
        json = json.Replace("%ProductName%", productName);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    /// <summary>
    /// Initializes the singleton instance of the <see cref="Logger"/>.
    /// </summary>
    public static void Initialize(IApplicationDirectories applicationDirectories)
    {
        lock (Lock)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException(_loggerServiceAlreadyInitialized);
            }

            _instance = new LoggerService(applicationDirectories);
        }
    }

    /// <inheritdoc />
    public void LogMessage(string message)
    {
        Log.Logger.Information(message);
    }

    /// <inheritdoc />
    public void LogError(Exception ex, string optionalExtraMessage = "")
    {
        var message = optionalExtraMessage;

        Log.Logger.Error(ex, message);
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        Log.CloseAndFlush();
        _instance = null;
    }
}
