using k8s.Models;

namespace Kubernetes.JobQueue;

public static class JobExtensions
{
    public static bool IsDone(this V1Job job)
    {
        return job.Status.Failed == job.Spec.BackoffLimit + 1
            || job.Status.Succeeded == job.Spec.Completions;
    }
}
