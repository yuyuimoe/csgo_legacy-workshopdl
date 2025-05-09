using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SupremacyWorkshopDownloader.SteamHandler;

internal class SteamRunner : IAsyncDisposable
{
    private readonly ProcessStartInfo _startInfo;
    private readonly SteamDownloader _steamDownloader;
    private readonly SteamOutputInterpreter _steamOutputInterpreter;
    private Process? _steamProcess;
    private readonly CancellationTokenSource _cts;
    private ILogger _logger;

    internal SteamRunner(SteamDownloader steamDownloader, ProcessStartInfo startInfo, SteamOutputInterpreter steamOutputInterpreter, CancellationTokenSource cts)
    {
        _startInfo = startInfo;
        _steamDownloader = steamDownloader;
        _steamOutputInterpreter = steamOutputInterpreter;
        _cts = cts;
        _logger = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug)).CreateLogger("SteamRunner");
    }

    private async Task RunSteam(string args)
    {
        _logger.LogInformation("Starting Steam.");
        try
        {
            _steamProcess = new Process { StartInfo = _startInfo, EnableRaisingEvents = true };
            
            _steamProcess.StartInfo.Arguments = args;
            _steamProcess.OutputDataReceived += async (sender, arguments) => await _steamOutputInterpreter.HandleOutput(arguments.Data ?? string.Empty);
            _steamProcess.Exited += (sender, _) => _cts.Cancel();
            
            await _steamDownloader.Download(); //Ensure SteamCMD is downloaded.
            
            _steamProcess.Start();
            _steamProcess.BeginOutputReadLine();
            
            await _steamProcess.WaitForExitAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new Exception("Operation was cancelled.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to run SteamCMD: {ex.Message}", ex);
        }

    }

    internal async Task DownloadWorkshopMap(long workshopId)
    {
        _logger.LogInformation("Downloading workshop map {workshopId}", workshopId);
        string args = $"+login anonymous +workshop_download_item 730 {workshopId} +quit";
        await RunSteam(args);
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        if (_steamProcess is not null)
        {
            try
            {
                if (!_steamProcess.HasExited)
                {
                    _steamProcess.Kill();
                    await _steamProcess.WaitForExitAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
                }
            }
            finally
            {
                _steamProcess.Dispose();
                _steamProcess = null;
            }
        }

        _cts.Dispose();
    }
}