using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from Polytracker PTM modules.
	/// </summary>
	public class PtmReader : IDisposable
	{
		// TODO: Read sample information

		// Filestream representing a PTM module
		private readonly FileStream ptm;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the PTM module</param>
		public PtmReader(string path)
		{
			ptm = File.OpenRead(path);
		}

		/// <summary>
		/// The song name stored in the PTM file
		/// </summary>
		public string SongName
		{
			get
			{
				byte[] songName = new byte[28];

				//Ensure we start at the beginning of the module.
				ptm.Seek(0, SeekOrigin.Begin);
				// Read 28 bytes
				ptm.Read(songName, 0, 28);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songName);
			}
		}

		/// <summary>
		/// The total number of orders within the PTM module
		/// </summary>
		/// <remarks>File can have between 1-256 orders</remarks>
		public int TotalOrders
		{
			get
			{
				// Seek 32 (0x20) bytes in
				ptm.Seek(0x20, SeekOrigin.Begin);

				return ptm.ReadByte();
			}
		}

		/// <summary>
		/// The total number of instruments used in the PTM module
		/// </summary>
		/// <remarks>Can have between 1 to 255 instruments</remarks>
		public int TotalInstruments
		{
			get
			{
				// Seek 34 (0x22) bytes in
				ptm.Seek(0x22, SeekOrigin.Begin);

				return ptm.ReadByte();
			}
		}

		/// <summary>
		/// The total number of patterns in the PTM module
		/// </summary>
		/// <remarks>Can have between 1 to 128 patterns</remarks>
		public int TotalPatterns
		{
			get
			{
				// Seek 36 (0x24) bytes in
				ptm.Seek(0x24, SeekOrigin.Begin);

				return ptm.ReadByte();
			}
		}

		/// <summary>
		/// The total number of channels/voices in the PTM module
		/// </summary>
		/// <remarks>Can have between 1 to 32 channels</remarks>
		public int Channels
		{
			get
			{
				// Seek 38 (0x26) bytes in
				ptm.Seek(0x26, SeekOrigin.Begin);

				return ptm.ReadByte();
			}
		}

		/// <summary>
		/// An integer that represents channel panning. 
		/// Integer can be between 1 to 15
		/// <para/>
		/// 0 = 3D Left
		/// <para/>
		/// 7 = 3D Middle
		/// <para/>
		/// 15 = 3D Right
		/// </summary>
		public int Panning
		{
			get
			{
				// Seek 64 (0x40) bytes in
				ptm.Seek(0x40, SeekOrigin.Begin);

				return ptm.ReadByte();
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
				ptm.Dispose();
			}
		}

		#endregion
	}
}
