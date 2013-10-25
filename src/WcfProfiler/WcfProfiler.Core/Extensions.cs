using System;
using System.IO;
using System.Runtime.Serialization;

namespace WcfProfiler.Core
{
	public static class Extensions
	{
		/// <summary>
		/// Determines if the source instance is null
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsNull(this object instance)
		{
			return (instance == null);
		}

		/// <summary>
		/// Determines if the specified instance is not null
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsNotNull(this object instance)
		{
			return !instance.IsNull();
		}

		public static string Serialize(this object obj)
		{
			using (var memoryStream = new MemoryStream())
			using (var reader = new StreamReader(memoryStream))
			{
				var serializer = new DataContractSerializer(obj.GetType());
				serializer.WriteObject(memoryStream, obj);
				memoryStream.Position = 0;
				return reader.ReadToEnd();
			}
		}

		public static object Deserialize(this string text, Type toType)
		{
			using (Stream stream = new MemoryStream())
			{
				var data = System.Text.Encoding.UTF8.GetBytes(text);
				stream.Write(data, 0, data.Length);
				stream.Position = 0;
				var deserializer = new DataContractSerializer(toType);
				return deserializer.ReadObject(stream);
			}
		}

		public static T Deserialize<T>(this string text)
		{
			var result = text.Deserialize(typeof(T));
			if (result != null)
			{
				return (T)result;
			}
			return default(T);
		}
	}
}