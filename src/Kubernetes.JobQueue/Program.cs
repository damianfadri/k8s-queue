using k8s;
using Kubernetes.JobQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services
            .AddHostedService<EnqueueWorker>()
            .AddHostedService<DequeueWorker>();

        services.AddSingleton(s => new k8s.Kubernetes(
            KubernetesClientConfiguration.BuildDefaultConfig()));

        services.Configure<KubernetesOptions>(
            context.Configuration.GetSection(nameof(KubernetesOptions)));
    })
    .Build()
    .Run();
