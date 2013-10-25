using System;
using System.Runtime.Serialization;

namespace WcfProfiler.Core
{
	[DataContract]
	public class EFProfiler
	{
		public EFProfiler(int commandId, string method, string sqlExecuted)
		{
			Id = Guid.NewGuid();
			CommandId = commandId;
			Method = method;
			SqlExecuted = sqlExecuted;
			Started = DateTime.UtcNow;
		}

		[DataMember]
		public int CommandId { get; set; }

		[DataMember]
		public TimeSpan Duration { get; set; }

		[DataMember]
		public DateTime Finished { get; set; }

		[DataMember]
		public bool HasFailed { get; set; }

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string Method { get; set; }

		[DataMember]
		public string Result { get; set; }

		[DataMember]
		public string SqlExecuted { get; set; }

		[DataMember]
		public DateTime Started { get; set; }

		public bool Updated { get; set; }

		public void Failed(string result)
		{
			Finished = DateTime.UtcNow;
			HasFailed = true;
			Result = result;
		}

		public void Update(TimeSpan duration)
		{
			Duration = duration;
			Finished = DateTime.UtcNow;
			Updated = true;
		}
	}
}