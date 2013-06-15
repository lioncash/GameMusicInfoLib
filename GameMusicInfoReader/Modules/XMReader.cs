using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for XM module files
	/// </summary>
	public class XMReader : IDisposable
	{
		// Filstream representing an XM module.
		private readonly FileStream xm;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an XM module.</param>
		public XMReader(string path)
		{
			xm = File.OpenRead(path);
		}

		/// <summary>
		/// The header magic of the module file.
		/// </summary>
		public string HeaderID
		{
			get
			{
				byte[] magic = new byte[17];

				// Ensure we start at the beginning of the file.
				xm.Seek(0, SeekOrigin.Begin);

				// Read 17 bytes
				xm.Read(magic, 0, 17);

				// Convert bytes to a string.
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(magic);
			}
		}

		/// <summary>
		/// The name of the XM module
		/// </summary>
		public string ModuleName
		{
			get
			{
				byte[] moduleName = new byte[20];

				// Seek 17 bytes in
				xm.Seek(0x11, SeekOrigin.Begin);
				// Read 20 bytes
				xm.Read(moduleName, 0, 20);

				// Convert bytes to a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(moduleName);
			}
		}

		/// <summary>
		/// That name of the tracker the module was made in.
		/// </summary>
		public string ModuleTracker
		{
			get
			{
				byte[] trackerName = new byte[20];

				// Seek 38 bytes in
				xm.Seek(0x26, SeekOrigin.Begin);

				// Read 20 bytes
				xm.Read(trackerName, 0, 20);

				// Convert bytes to a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(trackerName);
			}
		}

		/// <summary>
		/// The length of the song in patterns
		/// </summary>
		public int SongLength
		{
			get
			{
				// Seek 64 bytes in
				xm.Seek(0x40, SeekOrigin.Begin);
				return xm.ReadByte();
			}
		}

		/// <summary>
		/// The totoal number of channels in the XM file.
		/// </summary>
		public int TotalChannels
		{
			get
			{
				// Seek 68 bytes in
				xm.Seek(0x44, SeekOrigin.Begin);
				return xm.ReadByte();
			}
		}

		/// <summary>
		/// The total number of patterns in the XM file
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 70 bytes in
				xm.Seek(0x46, SeekOrigin.Begin);
				return xm.ReadByte();
			}
		}

		/// <summary>
		/// The total number of instruments in the XM file
		/// </summary>
		public int TotalInstruments
		{
			get
			{
				// Seek 72 bytes in
				xm.Seek(0x48, SeekOrigin.Begin);
				return xm.ReadByte();
			}
		}

		/// <summary>
		/// Type of frequency table being used.
		/// <para/>
		/// If bit 0 is set: Amiga frequency table is used.
		/// If bit 0 is not set: Linear frequency table is used.
		/// </summary>
		public int FreqTableType
		{
			get
			{
				// Make sure stream is left open
				using (BinaryReader br = new BinaryReader(xm, Encoding.Default, true))
				{
					// Seek 74 bytes in 
					br.BaseStream.Seek(0x4A, SeekOrigin.Begin);

					// Read short but only get lower 8 bytes
					int value = br.ReadInt16() & 0xFF;

					// If bit 0 is set
					if ((value & 1) != 0)
						return 1; // Linear frequency table is used
					else
						return 0; // Amiga frequency table is used
				}
			}
		}

		/// <summary>
		/// The default tempo of the XM file
		/// </summary>
		public int DefaultTempo
		{
			get
			{
				// Seek 76 bytes in
				xm.Seek(0x4C, SeekOrigin.Begin);
				return xm.ReadByte();
			}
		}

		/// <summary>
		/// The default BPM for the XM module.
		/// </summary>
		public int DefaultBPM
		{
			get
			{
				// Seek 78 bytes
				xm.Seek(0x4E, SeekOrigin.Begin);
				return xm.ReadByte();
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
				xm.Dispose();
			}
		}

		#endregion
	}
}
