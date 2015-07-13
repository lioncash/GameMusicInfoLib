using System.IO;

namespace GameMusicInfoReader.Modules.Xm
{
	/// <summary>
	/// Reader for XM module files
	/// </summary>
	public sealed class XMReader
	{
		#region Constructor

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
				if ((freqType & 1) == 0)
					FreqTableType = FrequencyTableType.Amiga;
				else
					FreqTableType = FrequencyTableType.Linear;

				// Defaults
				DefaultTempo = xm.ReadUInt16();
				DefaultBPM = xm.ReadUInt16();

				PatternOrderTable = xm.ReadBytes(256);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// The header magic of this module file.
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of this XM module
		/// </summary>
		public string ModuleName
		{
			get;
			private set;
		}

		/// <summary>
		/// That name of the tracker this module was made in.
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
		/// The total number of channels in this XM file.
		/// </summary>
		/// <remarks>Max of 32 channels</remarks>
		public int TotalChannels
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of patterns in this XM file
		/// </summary>
		/// <remarks>Max of 256 patterns</remarks>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of instruments in this XM file
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
		public FrequencyTableType FreqTableType
		{
			get;
			private set;
		}

		/// <summary>
		/// The default tempo of this XM file
		/// </summary>
		public int DefaultTempo
		{
			get;
			private set;
		}

		/// <summary>
		/// The default BPM for this XM module.
		/// </summary>
		public int DefaultBPM
		{
			get;
			private set;
		}

		/// <summary>
		/// The pattern order table for this module.
		/// </summary>
		public byte[] PatternOrderTable
		{
			get;
			private set;
		}

		#endregion
	}
}
