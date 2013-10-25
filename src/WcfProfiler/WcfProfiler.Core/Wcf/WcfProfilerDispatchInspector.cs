using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace WcfProfiler.Core.Wcf
{
	public class WcfProfilerDispatchInspector : IDispatchMessageInspector
	{
		public WcfProfilerDispatchInspector()
		{
			IsSoapRequest = false;
		}

		public bool IsSoapRequest { get; set; }

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			// Check to see if we have a request as part of this message
			var requestHeader = GetRequestHeader();
			if (requestHeader != null)
			{
				WcfProfiler.Start(GetName(request), requestHeader.RequestSent);
				return requestHeader;
			}

			return null;
		}

		private string GetName(Message request)
		{
			var opName = request.Properties["HttpOperationName"];

			return opName.IsNotNull() ? opName.ToString() : null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			var requestHeader = correlationState as WcfProfilerRequestHeader;
			var wcfProfiler = WcfProfiler.Stop(); 

			if (wcfProfiler != null && requestHeader != null)
			{
				wcfProfiler.RequestReplied = DateTime.UtcNow;
				AddResultsHeader(wcfProfiler);
			}
		}

		private void AddResultsHeader(WcfProfiler wcfProfiler)
		{
			var resultsHeader = new WcfProfilerResultsHeader()
			{
				ProfilerResults = wcfProfiler
			};
			if (IsSoapRequest)
			{
				var head = MessageHeader.CreateHeader(WcfProfilerResultsHeader.HeaderName, WcfProfilerResultsHeader.HeaderNamespace, resultsHeader.Serialize());
				OperationContext.Current.OutgoingMessageHeaders.Add(head);
			}
			else
			{
				if (WebOperationContext.Current != null)
					WebOperationContext.Current.OutgoingResponse.Headers.Add(WcfProfilerResultsHeader.HeaderName, resultsHeader.Serialize());
			}
		}

		private WcfProfilerRequestHeader GetRequestHeader()
		{
			if (IsSoapRequest)
			{
				var headerContent = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(WcfProfilerRequestHeader.HeaderName, WcfProfilerRequestHeader.HeaderNamespace);
				return headerContent.IsNotNull() ? headerContent.Deserialize<WcfProfilerRequestHeader>() : null;
			}
			if (WebOperationContext.Current != null)
			{
				var headerContent = WebOperationContext.Current.IncomingRequest.Headers.Get(WcfProfilerRequestHeader.HeaderName);
				return headerContent.IsNotNull() ? headerContent.Deserialize<WcfProfilerRequestHeader>() : null;
			}
			return null;
		}
	}
}