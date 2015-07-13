using System.IO;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for XM module files
	/// </summary>
	public sealed class XMReader
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an XM module.</param>
		public XMReader(string path)
		{
			using (var xm = new BinaryReader(File.OpenRead(path)))
			{
				// Header
				HeaderID = new string(xm.ReadChars(17));

				// Module name
				ModuleName = new string(xm.ReadChars(20));

				// Module tracker
				xm.BaseStream.Position += 1; // Skip garbage byte
				ModuleTracker = new string(xm.ReadChars(20));

				// Song length
				xm.BaseStream.Seek(0x40, SeekOrigin.Begin);
				SongLength = xm.ReadUInt16();

				// Restart position
				RestartPosition = xm.ReadUInt16();

				// Totals
				TotalChannels = xm.ReadUInt16();
				TotalPatterns = xm.ReadUInt16();
				TotalInstruments = xm.ReadUInt16();

				// Flags
				int freqType = xm.ReadInt16() & 0xFF;
				if ((freqType & 1) != 0)
					FreqTableType = 1;
				else
					FreqTableType = 0;

				// Defaults
				DefaultTempo = xm.ReadUInt16();
				DefaultBPM = xm.ReadUInt16();
			}
		}

		/// <summary>
		/// The header magic of the module file.
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the XM module
		/// </summary>
		public string ModuleName
		{
			get;
			private set;
		}

		/// <summary>
		/// That name of the tracker the module was made in.
		/// </summary>
		public string ModuleTracker
		{
			get;
			private set;
		}

		/// <summary>
		/// The length of the song in patterns
		/// </summary>
		public int SongLength
		{
			get;
			private set;
		}

		/// <summary>
		/// Restart position
		/// </summary>
		public int RestartPosition
		{
			get;
			private set;
		}

		/// <summary>
		/// The totoal number of channels in the XM file.
		/// </summary>
		/// <remarks>Max of 32 channels</remarks>
		public int TotalChannels
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of patterns in the XM file
		/// </summary>
		/// <remarks>Max of 256 patterns</remarks>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of instruments in the XM file
		/// </summary>
		/// <remarks>Maximum of 128 instruments</remarks>
		public int TotalInstruments
		{
			get;
			private set;
		}

		/// <summary>
		/// Type of frequency table being used.
		/// <para/>
		/// If bit 0 is set: Amiga frequency table is used.
		/// If bit 0 is not set: Linear frequency table is used.
		/// </summary>
		public int FreqTableType
		{
			get;
			private set;
		}

		/// <summary>
		/// The default tempo of the XM file
		/// </summary>
		public int DefaultTempo
		{
			get;
			private set;
		}

		/// <summary>
		/// The default BPM for the XM module.
		/// </summary>
		public int DefaultBPM
		{
			get;
			private set;
		}
	}
}
