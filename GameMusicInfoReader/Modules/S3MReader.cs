using System.IO;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from S3M modules.
	/// </summary>
	public sealed class S3MReader
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an S3M module.</param>
		public S3MReader(string path)
		{
			using (BinaryReader s3m = new BinaryReader(File.OpenRead(path)))
			{
				// Song title
				SongTitle = new string(s3m.ReadChars(28));

				// Totals
				s3m.BaseStream.Seek(0x20, SeekOrigin.Begin);
				TotalOrders = s3m.ReadUInt16();
				TotalInstruments = s3m.ReadUInt16();
				TotalPatterns = s3m.ReadUInt16();

				// Global volume
				s3m.BaseStream.Seek(0x30, SeekOrigin.Begin);
				GlobalVolume = s3m.ReadByte();

				// Initial speed
				int ins = s3m.ReadByte();
				if (ins == 0)
					ins = 6;
				InitialSpeed = ins;

				// Initial tempo
				int it = s3m.ReadByte();
				if (it < 33)
					it = 125;
				InitialTempo = it;

				// Is Stereo
				IsStereo = ((s3m.ReadByte() & 0x80) != 0);
			}
		}

		/// <summary>
		/// The song title of the S3M file
		/// </summary>
		public string SongTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of orders within the S3M module
		/// </summary>
		public int TotalOrders
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of instruments within the S3M module
		/// </summary>
		public int TotalInstruments
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the total number of patterns within the S3M module
		/// </summary>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The global volume of the S3M module
		/// </summary>
		public int GlobalVolume
		{
			get;
			private set;
		}

		/// <summary>
		/// The initial speed of the S3M module
		/// </summary>
		public int InitialSpeed
		{
			get;
			private set;
		}

		/// <summary>
		/// The initial tempo of the S3M module
		/// </summary>
		public int InitialTempo
		{
			get;
			private set;
		}

		/// <summary>
		/// Check if the S3M module is stereo
		/// </summary>
		public bool IsStereo
		{
			get;
			private set;
		}
	}
}
