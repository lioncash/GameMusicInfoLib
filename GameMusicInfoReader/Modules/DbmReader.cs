using System;
using System.IO;

// TODO: Song chunk reading.
// TODO: Instrument chunk reading.
// TODO: Pattern chunk reading.
// TODO: Sample chunk reading.
// TODO: Volume Envelope chunk reading.
namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Class for reading info from Digibooster Pro modules.
	/// </summary>
	public sealed class DbmReader
	{
		private const int NameChunkOffset = 8;
		private const int NameChunkSize   = 52;
		private const int InfoChunkSize   = 18;

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="file">Path to the DBM module to open.</param>
		public DbmReader(string file)
		{
			using (var dbm = new BinaryReader(File.OpenRead(file)))
			{
				// Header
				HeaderID = new string(dbm.ReadChars(4));

				// Skip to module name. Also cut off any padded bytes.
				dbm.BaseStream.Position = 0x10;
				string modName = new string(dbm.ReadChars(44));
				ModuleName = modName.Substring(0, modName.LastIndexOf('\0'));

				// Number of instruments.
				dbm.BaseStream.Position = NameChunkOffset + NameChunkSize + 8; // + 8 is to skip over the chunk name and length indicators.
				NumInstruments = BitConverter.IsLittleEndian ? dbm.ReadInt16()>>8 : dbm.ReadInt16();

				// Number of samples
				dbm.BaseStream.Position = NameChunkOffset + NameChunkSize + 10;
				NumSamples = BitConverter.IsLittleEndian ? dbm.ReadInt16()>>8 : dbm.ReadInt16();

				// Number of songs.
				dbm.BaseStream.Position = NameChunkOffset + NameChunkSize + 12;
				NumSongs = BitConverter.IsLittleEndian ? dbm.ReadInt16()>>8 : dbm.ReadInt16();

				// Number of patterns
				dbm.BaseStream.Position = NameChunkOffset + NameChunkSize + 14;
				NumPatterns = BitConverter.IsLittleEndian ? dbm.ReadInt16()>>8 : dbm.ReadInt16();

				// Number of channels
				dbm.BaseStream.Position = NameChunkOffset + NameChunkSize + 16;
				NumChannels = BitConverter.IsLittleEndian ? dbm.ReadInt16()>>8 : dbm.ReadInt16();
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Header identifier for DBM modules.
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// Name of this module.
		/// </summary>
		public string ModuleName
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of instruments in this module.
		/// </summary>
		public int NumInstruments
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of samples in this module.
		/// </summary>
		public int NumSamples
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of songs in this module.
		/// </summary>
		public int NumSongs
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of patterns in this module.
		/// </summary>
		public int NumPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of channels in this module.
		/// </summary>
		public int NumChannels
		{
			get;
			private set;
		}

		#endregion
	}
}
