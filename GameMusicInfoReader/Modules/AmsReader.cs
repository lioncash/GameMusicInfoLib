using System;
using System.IO;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// A reader for getting info from ExtremeTracker modules.
	/// </summary>
	public sealed class AmsReader : IDisposable
	{
		// TODO: Retrieving comments, sample info, etc

		// Filestream representing an AMS module
		private readonly FileStream ams;
		private readonly BinaryReader br;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an AMS module.</param>
		public AmsReader(string path)
		{
			ams = File.OpenRead(path);
			br = new BinaryReader(ams);
		}

		/// <summary>
		/// The total amount of samples stored
		/// within the AMS module.
		/// </summary>
		public int TotalSamples
		{
			get
			{
				// Seek 10 (0xA) bytes in
				ams.Seek(0xA, SeekOrigin.Begin);

				return ams.ReadByte();
			}
		}

		/// <summary>
		/// The total amount of patterns stored within the module
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 11 (0xB) bytes in
				br.BaseStream.Seek(0xB, SeekOrigin.Begin);

				return br.ReadUInt16();
			}
		}

		/// <summary>
		/// The total number of positions within the module
		/// </summary>
		public int TotalPositions
		{
			get
			{
				// Seek 13 (0xD) bytes in
				br.BaseStream.Seek(0xD, SeekOrigin.Begin);

				return br.ReadUInt16();
			}
		}

		/// <summary>
		/// The number of virtual MIDI channels within the module
		/// </summary>
		public int TotalVirtualMidiChannels
		{
			get
			{
				// Seek 15 (0xF) bytes in
				ams.Seek(0xF, SeekOrigin.Begin);

				return ams.ReadByte();
			}
		}

		// Make sure BinaryReader cleans up properly.
		public void Dispose()
		{
			br.Close();
			br.Dispose();
		}
	}
}
