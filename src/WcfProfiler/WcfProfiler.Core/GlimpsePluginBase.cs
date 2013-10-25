using System;
using System.Diagnostics;
using Glimpse.Core.Extensibility;

namespace WcfProfiler.Core
{
	public class GlimpsePluginBase
	{
		private static Stopwatch fromLastWatch;

		protected static TimeSpan CalculateFromLast(IExecutionTimer timer)
		{
			if (fromLastWatch == null)
			{
				fromLastWatch = Stopwatch.StartNew();
				return TimeSpan.FromMilliseconds(0);
			}

			// Timer started before this request, reset it
			if (DateTime.Now - fromLastWatch.Elapsed < timer.RequestStart)
			{
				fromLastWatch = Stopwatch.StartNew();
				return TimeSpan.FromMilliseconds(0);
			}

			var result = fromLastWatch.Elapsed;
			fromLastWatch = Stopwatch.StartNew();
			return result;
		}
	}
}