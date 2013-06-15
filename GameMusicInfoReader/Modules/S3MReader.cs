using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from S3M modules.
	/// </summary>
	public class S3MReader : IDisposable
	{
		// Filestream representing an S3M module.
		private readonly FileStream s3m;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an S3M module.</param>
		public S3MReader(string path)
		{
			s3m = File.OpenRead(path);
		}

		/// <summary>
		/// The song title of the S3M file
		/// </summary>
		public string SongTitle
		{
			get
			{
				byte[] songTitle = new byte[20];

				// Ensure we start at the beginning of the module.
				s3m.Seek(0, SeekOrigin.Begin);
				// Read first 20 bytes
				s3m.Read(songTitle, 0, 20);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(songTitle);
			}
		}

		/// <summary>
		/// The total number of orders within the S3M module
		/// </summary>
		public int TotalOrders
		{
			get
			{
				// Seek 32 bytes in (0x20)
				s3m.Seek(0x20, SeekOrigin.Begin);

				// Read a byte
				return s3m.ReadByte();
			}
		}

		/// <summary>
		/// The total number of instruments within the S3M module
		/// </summary>
		public int TotalInstruments
		{
			get
			{
				// Seek 34 bytes in (0x22)
				s3m.Seek(0x22, SeekOrigin.Begin);

				// Read a byte
				return s3m.ReadByte();
			}
		}

		/// <summary>
		/// Gets the total number of patterns within the S3M module
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 36 bytes in (0x24)
				s3m.Seek(0x24, SeekOrigin.Begin);

				// Read a byte
				return s3m.ReadByte();
			}
		}

		/// <summary>
		/// The global volume of the S3M module
		/// </summary>
		public int GlobalVolume
		{
			get
			{
				// Seek 48 bytes in (0x30)
				s3m.Seek(0x30, SeekOrigin.Begin);

				return s3m.ReadByte();
			}
		}

		/// <summary>
		/// The initial speed of the S3M module
		/// </summary>
		public int InitialSpeed
		{
			get
			{
				// Seek 49 bytes in (0x31)
				s3m.Seek(0x31, SeekOrigin.Begin);

				int retVal = s3m.ReadByte();

				// If zero, return 6 as the default 
				// initial speed
				if (retVal == 0)
					return 6;

				return retVal;
			}
		}

		/// <summary>
		/// The initial tempo of the S3M module
		/// </summary>
		public int InitialTempo
		{
			get
			{
				// Seek 50 bytes in (0x32)
				s3m.Seek(0x32, SeekOrigin.Begin);

				int retVal = s3m.ReadByte();

				// If tempo is less than 33, then
				// by default, return 125
				if (retVal < 33)
					return 125;

				return retVal;
			}
		}

		/// <summary>
		/// Check if the S3M module is stereo
		/// </summary>
		public bool IsStereo
		{
			get
			{
				// Seek 51 bytes in (0x33)
				s3m.Seek(0x33, SeekOrigin.Begin);

				byte retVal = (byte)s3m.ReadByte();

				if ((retVal & 0x80) != 0)
					return true;

				return false;
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
				s3m.Dispose();
			}
		}

		#endregion
	}
}
