﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mono.Security.Instrumentation.Framework
{
	public class StreamWrapper : ILineBasedStream
	{
		public Stream InnerStream {
			get;
			private set;
		}

		StreamReader reader;
		StreamWriter writer;

		public StreamWrapper (Stream innerStream)
		{
			InnerStream = innerStream;
			reader = new StreamReader (innerStream, Encoding.ASCII);
			writer = new StreamWriter (innerStream, Encoding.ASCII);
			writer.AutoFlush = true;
		}

		public Task<string> ReadLineAsync ()
		{
			return reader.ReadLineAsync ();
		}

		public Task WriteLineAsync (string line)
		{
			return writer.WriteLineAsync (line);
		}
	}
}

