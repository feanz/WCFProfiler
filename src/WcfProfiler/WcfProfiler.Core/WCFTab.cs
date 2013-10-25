using System.Linq;
using Glimpse.AspNet.Extensibility;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;

namespace WcfProfiler.Core
{
	/// <summary>
	/// Output profiler details of calls to WCF services.
	/// </summary>
	public class WCFTab : AspNetTab, ITabSetup, ITabLayout
	{
		private static readonly object layout = TabLayout.Create()
			.Row(r =>
			{
				r.Cell("operationName").WithTitle("Operation Name");
				r.Cell("durationMilliseconds").Suffix(" ms").Prefix("T+ ").Class("mono").WithTitle("Duration");
				r.Cell("fromFirst").Suffix(" ms").Prefix("T+ ").Class("mono").WithTitle("From Request Start");
			}).Build();

		public override object GetData(ITabContext context)
		{
			var items = context.GetMessages<WcfProfiler>().ToList();
			return items;
		}

		public override string Name
		{
			get { return "WCF"; }
		}

		public void Setup(ITabSetupContext context)
		{
			context.PersistMessages<WcfProfiler>();
		}

		public object GetLayout()
		{
			return layout;
		}
	}
}