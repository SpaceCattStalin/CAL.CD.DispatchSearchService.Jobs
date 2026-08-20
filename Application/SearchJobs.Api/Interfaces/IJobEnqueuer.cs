using System.Linq.Expressions;
using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public interface IJobEnqueuer<T>
{
    public string Enqueue(Expression<Func<T, Task>> job);
}
