using System;

namespace WcfProfiler.Core
{
	public class ServiceProfileData
	{
		public string MethodCall { get; set; }
		public TimeSpan TimeTaken { get; set; }
		public TimeSpan FromFirst { get; set; }
		public TimeSpan FromLast { get; set; }
	}
}