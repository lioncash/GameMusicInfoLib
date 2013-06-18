using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// A reader for getting info from 669 modules.
	/// </summary>
	public sealed class SixSixNineReader : IDisposable
	{
		// Filestream representing a 669 module.
		private readonly FileStream ssn;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to the 669 module.</param>
		public SixSixNineReader(string path)
		{
			ssn = File.OpenRead(path);
		}

		/// <summary>
		/// The header magic of the 669 file.
		/// <para/>
		/// Should get the string 'if' if it's a normal 669 module.
		/// <para/>
		/// You should get 'JN' if it's an extended 669 module
		/// </summary>
		public string HeaderID
		{
			get
			{
				byte[] magic = new byte[2];

				// Make sure we start at the beginning of the module.
				ssn.Seek(0, SeekOrigin.Begin);
				// Read 2 bytes
				ssn.Read(magic, 0, 2);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(magic);
			}
		}

		/// <summary>
		/// The embedded comment in the 669 file (if any)
		/// </summary>
		public string Comment
		{
			get
			{
				byte[] commentBytes = new byte[108];

				// Seek 2 bytes in
				ssn.Seek(2, SeekOrigin.Begin);

				// Read 108 bytes
				ssn.Read(commentBytes, 0, 108);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(commentBytes);
			}
		}

		/// <summary>
		/// The total number or samples saved.
		/// <para/>
		/// <remarks>Is between or equal to 0-64 sample(s)</remarks>
		/// </summary>
		public int TotalSamples
		{
			get
			{
				// Seek 110 (0x6E) bytes in
				ssn.Seek(0x6E, SeekOrigin.Begin);

				return ssn.ReadByte();
			}
		}

		/// <summary>
		/// The total number of patterns saved.
		/// <para/>
		/// <remarks>Is between or equal to 0-128 pattern(s)</remarks>
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 111 (0x6F) bytes in
				ssn.Seek(0x6F, SeekOrigin.Begin);

				return ssn.ReadByte();
			}
		}

		/// <summary>
		/// The loop order number within the 669 module.
		/// </summary>
		public int LoopOrder
		{
			get
			{
				// Seek 112 (0x70) bytes in
				ssn.Seek(0x70, SeekOrigin.Begin);

				return ssn.ReadByte();
			}
		}

		#region IDisposable Methods

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				ssn.Dispose();
			}
		}

		#endregion
	}
}
