using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from ScreamTracker 2.x modules.
	/// </summary>
	public sealed class StmReader : IDisposable
	{
		// TODO: Get instrument information

		// Filestream that represents an STM module
		private readonly FileStream stm;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an STM module</param>
		public StmReader(string path)
		{
			stm = File.OpenRead(path);
		}

		/// <summary>
		/// The song name of the track in the STM module
		/// </summary>
		public string SongName
		{
			get
			{
				byte[] songName = new byte[20];

				// Make sure we start at the beginning of the file.
				stm.Seek(0, SeekOrigin.Begin);
				// Read 20 bytes
				stm.Read(songName, 0, 20);

				// Convert retrieved bytes into a string
				return Encoding.UTF8.GetString(songName);
			}
		}

		/// <summary>
		/// The name of the tracker the module was created in
		/// </summary>
		public string TrackerName
		{
			get
			{
				byte[] trackerName = new byte[8];

				// Seek 20 (0x14) bytes in
				stm.Seek(0x14, SeekOrigin.Begin);

				// Read 8 bytes
				stm.Read(trackerName, 0, 8);

				// Convert retrieved bytes into a string
				return Encoding.UTF8.GetString(trackerName);
			}
		}

		/// <summary>
		/// The file type of the module file.
		/// <para/>
		/// If it returns 1, it's a song.
		/// <para/>
		/// If it returns 2, it's a module.
		/// </summary>
		public int FileType
		{
			get
			{
				// Seek 29 (0x1D) bytes in
				stm.Seek(0x1D, SeekOrigin.Begin);

				// If it returns 2, then it's a module
				if (stm.ReadByte() == 2)
					return 2;

				// If it returns 1, then it's a song
				return 1;
			}
		}

		/// <summary>
		/// The tempo of the module file
		/// </summary>
		public int Tempo
		{
			get
			{
				// Seek 32 (0x20) bytes in
				stm.Seek(0x20, SeekOrigin.Begin);

				return stm.ReadByte();
			}
		}

		/// <summary>
		/// The total number of patterns saved in the STM module.
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 33 (0x21) bytes in
				stm.Seek(0x21, SeekOrigin.Begin);

				return stm.ReadByte();
			}
		}

		/// <summary>
		/// The global volume of the STM module
		/// </summary>
		public int GlobalVolume
		{
			get
			{
				// Seek 34 (0x22) bytes in
				stm.Seek(0x22, SeekOrigin.Begin);

				return stm.ReadByte();
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
				stm.Dispose();
			}
		}

		#endregion
	}
}
