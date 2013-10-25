using Glimpse.Core.Extensibility;

namespace WcfProfiler.Core
{
	/// <summary>
	/// Initialize the glimpse WCF profiler, giving it reference to the glimpse message broker and timing strategy. 
	/// </summary>
	public class GlimpseWcfProfilerInspector :IInspector
	{
		public void Setup(IInspectorContext context)
		{
			GlimpseWcfProfiler.Initialize(context.MessageBroker, context.TimerStrategy);
		}
	}
}