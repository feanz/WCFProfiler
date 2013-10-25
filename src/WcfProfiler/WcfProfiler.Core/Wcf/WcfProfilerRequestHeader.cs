using System;
using System.Runtime.Serialization;

namespace WcfProfiler.Core.Wcf
{
	[DataContract]
	public class WcfProfilerRequestHeader
	{
		public const string HeaderName = "WcfProfilerRequest";
		public const string HeaderNamespace = "Think.Formica.Resin.WebService.Profiler";

		public WcfProfilerRequestHeader()
		{
			RequestSent = DateTime.UtcNow;
		}

		[DataMember]
		public DateTime RequestSent { get; set; }
	}
}