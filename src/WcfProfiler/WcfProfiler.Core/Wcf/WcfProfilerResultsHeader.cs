using System.Runtime.Serialization;

namespace WcfProfiler.Core.Wcf
{
	[DataContract]
	public class WcfProfilerResultsHeader
	{
		public const string HeaderName = "WcfProfilerResults";
		public const string HeaderNamespace = "Think.Formica.Resin.WebService.Profiler";

		[DataMember]
		public WcfProfiler ProfilerResults { get; set; }
	}
}