using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;

namespace Kubernetes.JobQueue;

public class EnqueueWorker(k8s.Kubernetes client, IOptions<KubernetesOptions> options) : ThreadedHostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var jobList = client.BatchV1.ListNamespacedJobWithHttpMessagesAsync(
                namespaceParameter: options.Value.Namespace,
                labelSelector: options.Value.Selector,
                watch: true,
                cancellationToken: stoppingToken);

            await foreach (var (type, job) in 
                jobList.WatchAsync<V1Job, V1JobList>(cancellationToken: stoppingToken))
            {
                if (type != WatchEventType.Added)
                {
                    continue;
                }

                if (job.IsDone())
                {
                    continue;
                }

                try
                {
                    var patch = new V1Patch(new SuspendPatch(true), V1Patch.PatchType.MergePatch);

                    await client.BatchV1.PatchNamespacedJobAsync(
                        body: patch,
                        name: job.Metadata.Name,
                        namespaceParameter: options.Value.Namespace,
                        cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Enqueue error");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
