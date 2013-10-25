using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace WcfProfiler.Core
{
	[DataContract]
	public class WcfProfiler
	{
		private static WcfProfiler _current;
		private readonly Stopwatch _sw;

		private WcfProfiler(string operationName, DateTime requestSent)
		{
			Id = Guid.NewGuid();
			MachineName = Environment.MachineName;
			RequestSent = requestSent;
			EFProfilers = new List<EFProfiler>();
			OperationName = operationName;
			_sw = new Stopwatch();
		}

		public static WcfProfiler Current
		{
			get { return _current; }
		}

		[DataMember]
		public decimal DurationMilliseconds
		{
			get { return GetRoundedMilliseconds(_sw.ElapsedTicks); }
		}

		public List<EFProfiler> EFProfilers { get; set; }

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string OperationName { get; set; }

		[DataMember]
		public string MachineName { get; set; }

		[DataMember]
		public DateTime RequestEnded { get; set; }

		[DataMember]
		public DateTime RequestRecived { get; set; }

		[DataMember]
		public DateTime RequestReplied { get; set; }

		[DataMember]
		public DateTime RequestSent { get; set; }

		//request pipeline time properties
		[DataMember]
		public TimeSpan FromFirst { get; set; }

		[DataMember]
		public TimeSpan FromLast { get; set; }

		public static void AddEFProfiler(EFProfiler efProfiler)
		{
			AddEFProfiler(Current, efProfiler);
		}

		public static void FailEFProfiler(string result)
		{
			FailEFProfiler(Current, result);
		}

		public static WcfProfiler Start(string operationName, DateTime requestSent)
		{
			var result = new WcfProfiler(operationName, requestSent);

			SetCurrentProfiler(result);

			StartProfiler(Current);

			return result;
		}

		public static WcfProfiler Stop()
		{
			var current = Current;

			ClearCurrentProfiler();

			StopProfiler(current);

			return current;
		}

		public static void UpdateEFProfiler(TimeSpan duration)
		{
			UpdateEFProfiler(Current, duration);
		}

		public bool StopImpl()
		{
			if (!_sw.IsRunning)
				return false;

			RequestReplied = DateTime.UtcNow;

			_sw.Stop();

			return true;
		}

		internal decimal GetRoundedMilliseconds(long stopwatchElapsedTicks)
		{
			var z = 10000 * stopwatchElapsedTicks;
			decimal msTimesTen = (int)(z / Stopwatch.Frequency);
			return msTimesTen / 10;
		}

		protected static bool StopProfiler(WcfProfiler profiler)
		{
			if (profiler == null)
				throw new ArgumentNullException("profiler");

			return profiler.StopImpl();
		}

		private static void AddEFProfiler(WcfProfiler profiler, EFProfiler efProfiler)
		{
			if(profiler == null) throw new ArgumentNullException("profiler");

			profiler.AddEFProfilerImpl(efProfiler);
		}

		private static void ClearCurrentProfiler()
		{
			_current = null;
		}

		private static void FailEFProfiler(WcfProfiler profiler, string result)
		{
			if (profiler == null) throw new ArgumentNullException("profiler");

			profiler.FailEFProfilerImp(result);
		}

		private static void SetCurrentProfiler(WcfProfiler profiler)
		{
			_current = profiler;
		}

		private static void StartProfiler(WcfProfiler profiler)
		{
			if (profiler == null) throw new ArgumentNullException("profiler");

			profiler.StartImpl();
		}

		private static void UpdateEFProfiler(WcfProfiler profiler, TimeSpan duration)
		{
			if (profiler == null) throw new ArgumentNullException("profiler");

			profiler.UpdateEFProfilerImpl(duration);
		}

		private void AddEFProfilerImpl(EFProfiler efProfiler)
		{
			EFProfilers.Add(efProfiler);
		}

		private void FailEFProfilerImp(string result)
		{
			var lastProfiler = EFProfilers.First(profiler => !profiler.Updated);
			lastProfiler.Failed(result);
		}

		private void StartImpl()
		{
			_sw.Start();

			RequestRecived = DateTime.Now;
		}

		private void UpdateEFProfilerImpl(TimeSpan duration)
		{
			var lastProfiler = EFProfilers.First(profiler => !profiler.Updated);
			lastProfiler.Update(duration);
		}
	}
}