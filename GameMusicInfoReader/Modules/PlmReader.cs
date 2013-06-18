using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for Disorder Tracker 2 modules
	/// </summary>
	public sealed class PlmReader : IDisposable
	{
		// Filestream representing a PLM module.
		private readonly FileStream plm;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to a PLM module</param>
		public PlmReader(string path)
		{
			plm = File.OpenRead(path);
		}

		/// <summary>
		/// Number of bytes in the header
		/// </summary>
		public int HeaderSize
		{
			get
			{
				// Seek 5 (0x5) bytes in
				plm.Seek(0x5, SeekOrigin.Begin);

				return plm.ReadByte();
			}
		}

		/// <summary>
		/// The name of the song stored in the 
		/// PLM module.
		/// </summary>
		public string SongName
		{
			get
			{
				byte[] songName = new byte[48];

				// Seek 6 bytes in
				plm.Seek(0x6, SeekOrigin.Begin);

				// Read 48 bytes
				plm.Read(songName, 0, 48);

				// Convert to string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songName);
			}
		}

		/// <summary>
		/// The amount of audio channels within the PLM module
		/// </summary>
		public int TotalChannels
		{
			get
			{
				// Seek 54 (0x36) bytes in
				plm.Seek(0x36, SeekOrigin.Begin);

				return plm.ReadByte();
			}
		}

		/// <summary>
		/// The maximum volume for vol slides.
		/// </summary>
		public int MaxSlideVolume
		{
			get
			{
				// Seek 56 (0x38) bytes in
				plm.Seek(0x38, SeekOrigin.Begin);

				return plm.ReadByte();
			}
		}

		/// <summary>
		/// Soundblaster Amplification, if it returns
		/// 0x40 or 64, then there is no amplification
		/// </summary>
		public int SoundblasterAmplification
		{
			get
			{
				// Seek 57 (0x39) bytes in
				plm.Seek(0x39, SeekOrigin.Begin);

				return plm.ReadByte();
			}
		}

		/// <summary>
		/// The initial BPM at the start of a track
		/// </summary>
		public int InitialBPM
		{
			get
			{
				// Seek 58 (0x3A) bytes in
				plm.Seek(0x3A, SeekOrigin.Begin);

				return plm.ReadByte();
			}
		}

		/// <summary>
		/// The initial speed within the PLM module
		/// </summary>
		public int InitialSpeed
		{
			get
			{
				// Seek 59 (0x3B) bytes in
				plm.Seek(0x3B, SeekOrigin.Begin);

				return plm.ReadByte();
			}
		}

		/// <summary>
		/// Total amount of samples within the PLM module
		/// </summary>
		public int TotalSamples
		{
			get
			{
				// Seek 92 (0x5C) bytes in
				plm.Seek(0x5C, SeekOrigin.Begin);

				return plm.ReadByte();
			}
		}

		/// <summary>
		/// Total amount of patterns within the PLM module
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 93 (0x5D) bytes in
				plm.Seek(0x5D, SeekOrigin.Begin);

				return plm.ReadByte();
			}
		}

		/// <summary>
		/// The total amount of orders within the PLM module
		/// </summary>
		public int TotalOrders
		{
			get
			{
				// Seek 94 (0x5E) bytes in
				plm.Seek(0x5E, SeekOrigin.Begin);

				return plm.ReadByte();
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
				plm.Dispose();
			}
		}

		#endregion
	}
}
