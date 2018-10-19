using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFXV.Utilities
{
	public static class TaskExtensions
	{
		public async static Task WhenAll(this IEnumerable<Task> tasks, int maxTasksCount)
		{
			var executing = tasks
				.Take(maxTasksCount)
				.ToList();
			var remaining = tasks.Skip(maxTasksCount);

			while (true)
			{
				executing.Remove(await Task.WhenAny(executing));

				var nextTaskToExecute = remaining.FirstOrDefault();
				if (nextTaskToExecute != null)
				{
					executing.Add(nextTaskToExecute);
					remaining = remaining.Skip(1);
				}
				else
				{
					break;
				}
			}

			await Task.WhenAll(executing);
		}

		public static void WaitAll(this IEnumerable<Task> tasks, int maxTasksCount)
		{
			var executing = tasks
				.Take(maxTasksCount)
				.ToArray();
			var remaining = tasks.Skip(maxTasksCount);

			while (true)
			{
				var terminatedIndex = Task.WaitAny(executing);

				var nextTaskToExecute = remaining.FirstOrDefault();
				if (nextTaskToExecute != null)
				{
					executing[terminatedIndex] = nextTaskToExecute;
					remaining = remaining.Skip(1);
				}
				else
				{
					break;
				}
			}

			Task.WaitAll(executing);
		}
	}
}
