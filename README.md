# Kubernetes Queue
## Overview
Kubernetes Queue is an operator that adds a queueing mechanism to Kubernetes Jobs. It allows Jobs, whether deployed directly or spawned by CronJobs, to be run sequentially.

## Prerequisites
* .NET 9.0
* Kubernetes 

## Running the Project
1. Clone this project locally
2. Restore dependencies

```powershell
dotnet restore
```

3. Set the namespace and label to monitor.

```powershell
$env:KubernetesOptions__Namespace="default"
$env:KubernetesOptions__Selector="jobs/queue-enabled=true"
```

4. Run the application

```powershell
dotnet run --project src/Kubernetes.JobQueue
```

## Usage
The queue operates by monitoring all jobs in a namespace that matches the label selector.

For example, a selector like `jobs/queue-enabled=true` would match a Job with the following manifest:

```yaml
apiVersion: batch/v1
kind: Job
metadata:
  name: sample-job
  labels:
    jobs/queue-enabled: "true"
```

Once deployed, the application should log the state of the job. If you deploy multiple jobs, you should be able to see it being run in succession.

```
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Users\damian\source\repos\k8s-queue

yes-label-1 is next, running.
yes-label-2 is next, running.
yes-label-3 is next, running.
```