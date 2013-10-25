using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WcfProfiler.Core.Wcf
{
	public class WcfProfilerBehaviourExtension : BehaviorExtensionElement, IEndpointBehavior
	{
		protected override object CreateBehavior()
		{
			return this;
		}

		public override Type BehaviorType
		{
			get { return typeof (WcfProfilerBehaviourExtension); }
		}

		public void Validate(ServiceEndpoint endpoint)
		{

		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{

		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			var inspector = new WcfProfilerDispatchInspector();
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			var inspector = new WcfProfilerClientInspector();
			clientRuntime.MessageInspectors.Add(inspector);
		}
	}
}