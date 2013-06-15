using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// A reader for getting info from AMusic AMD files.
	/// </summary>
	public sealed class AmdReader : IDisposable
	{
		// A Filestream representing an AMD module.
		private readonly FileStream amd;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the AMD module.</param>
		public AmdReader(string path)
		{
			amd = File.OpenRead(path);
		}

		/// <summary>
		/// The name of the song within the module.
		/// </summary>
		public string SongName
		{
			get
			{
				byte[] songName = new byte[24];

				//Ensure we start at the beginning of the module.
				amd.Seek(0, SeekOrigin.Begin);
				// Read 18 bytes
				amd.Read(songName, 0, 24);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songName);
			}
		}

		/// <summary>
		/// The name of the artist who created the module file
		/// </summary>
		public string Artist
		{
			get
			{
				byte[] songName = new byte[24];

				// Seek 24 bytes in
				amd.Seek(24, SeekOrigin.Begin);

				// Read 20 bytes
				amd.Read(songName, 0, 24);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songName);
			}
		}

		/// <summary>
		/// Length of the track
		/// </summary>
		public int SongLength
		{
			get
			{
				// Seek 932 bytes in
				amd.Seek(0x3A4, SeekOrigin.Begin);

				return amd.ReadByte();
			}
		}

		/// <summary>
		/// Total patterns - 1
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 933 bytes in
				amd.Seek(0x3A5, SeekOrigin.Begin);

				return amd.ReadByte();
			}
		}

		/// <summary>
		/// If 0x10 (16) is returned, then the module is normal.
		/// <para/>
		/// If 0x11 (17) is returned, then the module is packed.
		/// </summary>
		public int Version
		{
			get
			{
				// Seek 1071 bytes in
				amd.Seek(0x42F, SeekOrigin.Begin);

				return amd.ReadByte();
			}
		}

		#region IDisposable Methods

		public void Dispose()
		{
			Dispose(true);
		}

		public void Dispose(bool disposing)
		{
			if (disposing)
			{
				amd.Dispose();
			}
		}

		#endregion
	}
}
