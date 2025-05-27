namespace Kubernetes.JobQueue;

public class KubernetesOptions
{
    public required string Namespace { get; set; }
    public required string Selector { get; set; }
}
