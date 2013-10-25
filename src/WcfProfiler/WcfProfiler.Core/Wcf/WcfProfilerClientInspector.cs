using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace WcfProfiler.Core.Wcf
{
	public class WcfProfilerClientInspector : IClientMessageInspector
	{
		public WcfProfilerClientInspector()
		{
			ProfilerEnabled = () => true;
			IsSoapRequest = false;
		}

		public static Func<bool> ProfilerEnabled { get; set; }

		public bool IsSoapRequest { get; set; }

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var profilerEnabled = ProfilerEnabled();
			if (profilerEnabled)
			{
				AddRequestHeader();

				return new SimpleProfilerStart {StartTime = DateTime.UtcNow};
			}

			return null;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			var profilerStart = correlationState as SimpleProfilerStart;
			var profilerEnabled = ProfilerEnabled();
			if (profilerEnabled)
			{
				// Check to see if we have a request as part of this message
				var header = GetResultsHeader(reply);
				if (header != null)
				{
					// Update timings of profiler results
					if (profilerStart != null)
						header.ProfilerResults.RequestEnded = DateTime.UtcNow;

					//add results to glimpse here
					GlimpseWcfProfiler.Profile(header.ProfilerResults);
				}
			}
		}

		private void AddRequestHeader()
		{
			var wcfProfilerRequestHeader = new WcfProfilerRequestHeader
			{
				RequestSent = DateTime.UtcNow
			};
			if (IsSoapRequest)
			{
				var head = MessageHeader.CreateHeader(WcfProfilerRequestHeader.HeaderName, WcfProfilerRequestHeader.HeaderNamespace, wcfProfilerRequestHeader.Serialize());
				OperationContext.Current.OutgoingMessageHeaders.Add(head);
			}
			else
			{
				if (WebOperationContext.Current != null) 
					WebOperationContext.Current.OutgoingRequest.Headers.Add(WcfProfilerRequestHeader.HeaderName, wcfProfilerRequestHeader.Serialize());
			}
		}

		private WcfProfilerResultsHeader GetResultsHeader(Message reply)
		{
			if (IsSoapRequest)
			{
				var headerContent = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(WcfProfilerResultsHeader.HeaderName, WcfProfilerResultsHeader.HeaderNamespace);
				return headerContent != null ? headerContent.Deserialize<WcfProfilerResultsHeader>() : null;
			}
			if (WebOperationContext.Current != null)
			{
				//var headerContent = WebOperationContext.Current.OutgoingRequest.Headers.Get(WcfProfilerResultsHeader.HeaderName);
				//return headerContent.IsNotNull() ? headerContent.Deserialize<WcfProfilerResultsHeader>() : null;
				var httpResponse = reply.Properties["httpResponse"] as HttpResponseMessageProperty;

				if (httpResponse != null)
				{
					var headerContent = httpResponse.Headers.Get(WcfProfilerResultsHeader.HeaderName);
					return headerContent != null ? headerContent.Deserialize<WcfProfilerResultsHeader>() : null;
				}
			}
			return null;
		}

		private class SimpleProfilerStart
		{
			public DateTime StartTime { get; set; }
		}
	}
}