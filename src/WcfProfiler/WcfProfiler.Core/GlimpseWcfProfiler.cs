using System;
using Glimpse.Core.Extensibility;

namespace WcfProfiler.Core
{
	/// <summary>
	/// Profile WCF service calls 
	/// </summary>
	public class GlimpseWcfProfiler : GlimpsePluginBase
	{
		private static IMessageBroker _messageBroker;
		private static Func<IExecutionTimer> _timerStrategy;

		public static void Initialize(IMessageBroker messageBroker, Func<IExecutionTimer> timerStrategy)
		{
			_messageBroker = messageBroker;
			_timerStrategy = timerStrategy;
		}

		/// <summary>
		/// Add profile data to the glimpse message broker
		/// </summary>
		/// <param name="wcfProfiler"></param>
		public static void Profile(WcfProfiler wcfProfiler)
		{
			if (_timerStrategy != null && _messageBroker != null)
			{
				var timer = _timerStrategy();
				if (timer != null)
				{
					wcfProfiler.FromFirst = timer.Point().Offset;
					wcfProfiler.FromLast = CalculateFromLast(timer);
					_messageBroker.Publish(wcfProfiler);
				}
			}
		}
	}
}