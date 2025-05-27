using Microsoft.Extensions.Hosting;

namespace Kubernetes.JobQueue;

public abstract class ThreadedHostedService : IHostedService
{
    private Task _executingTask = Task.CompletedTask;
    private CancellationTokenSource _cts = new();

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        _executingTask = ExecuteAsync(_cts.Token);

        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken stoppingToken)
    {
        if (_executingTask == null)
        {
            return;
        }

        _cts.Cancel();

        await Task.WhenAny(_executingTask, Task.Delay(-1, stoppingToken));

        stoppingToken.ThrowIfCancellationRequested();
    }

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);
}
