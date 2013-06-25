using System;
using System.IO;
using System.Text;

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
	public sealed class DbmReader : IDisposable
	{
		// Represents a DBM module.
		private readonly FileStream dbm;
		private readonly BinaryReader br;

		private const int NameChunkOffset = 8;
		private const int NameChunkSize   = 52;
		private const int InfoChunkSize   = 18;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="file">Path to the DBM module to open.</param>
		public DbmReader(string file)
		{
			dbm = new FileStream(file, FileMode.Open);
			br = new BinaryReader(dbm);
		}

		/// <summary>
		/// Header identifier for DBM modules.
		/// </summary>
		public string HeaderID
		{
			get
			{
				// Ensure we start from the beginning of the stream.
				dbm.Position = 0;

				byte[] magicBytes = new byte[4];
				dbm.Read(magicBytes, 0, magicBytes.Length);

				return Encoding.UTF8.GetString(magicBytes);
			}
		}

		/// <summary>
		/// Name of this module.
		/// </summary>
		public string ModuleName
		{
			get
			{
				// Go 16 bytes in.
				dbm.Position = 0x10;

				// Maximum number of characters a name can have is 44 characters.
				// Note that not all of them are required to be used.
				byte[] nameBytes = new byte[44];
				dbm.Read(nameBytes, 0, nameBytes.Length);

				// Get the name as a string and cut off any padded bytes.
				string name = Encoding.UTF8.GetString(nameBytes);
				return name.Substring(0, name.LastIndexOf((char) 0x00));
			}
		}

		/// <summary>
		/// Number of instruments in this module.
		/// </summary>
		public int NumInstruments
		{
			get
			{
				br.BaseStream.Position = NameChunkOffset + NameChunkSize + 8; // + 8 is to skip over the chunk name and length indicators.

				return br.ReadInt16() >> 8; // Shift because big-endian format.
			}
		}

		/// <summary>
		/// Number of samples in this module.
		/// </summary>
		public int NumSamples
		{
			get
			{
				br.BaseStream.Position = NameChunkOffset + NameChunkSize + 10;

				return br.ReadInt16() >> 8; // Shift because big-endian format.
			}
		}

		/// <summary>
		/// Number of songs in this module.
		/// </summary>
		public int NumSongs
		{
			get
			{
				br.BaseStream.Position = NameChunkOffset + NameChunkSize + 12;

				return br.ReadInt16() >> 8; // Shift because big-endian format.
			}
		}

		/// <summary>
		/// Number of patterns in this module.
		/// </summary>
		public int NumPatterns
		{
			get
			{
				br.BaseStream.Position = NameChunkOffset + NameChunkSize + 14;

				return br.ReadInt16() >> 8; // Shift because big-endian format.
			}
		}

		/// <summary>
		/// Number of channels in this module.
		/// </summary>
		public int NumChannels
		{
			get
			{
				br.BaseStream.Position = NameChunkOffset + NameChunkSize + 16;

				return br.ReadInt16() >> 8; // Shift because big-endian format.
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
				dbm.Close();
			}
		}

		#endregion
	}
}
