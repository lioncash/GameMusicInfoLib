using System;
using System.IO;
using System.Text;

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
		/// The header identifier string.
		/// </summary>
		public string HeaderID
		{
			get
			{
				// Ensure we start at the beginning of the stream.
				ams.Position = 0;

				byte[] magicBytes = new byte[7];
				ams.Read(magicBytes, 0, magicBytes.Length);

				return Encoding.UTF8.GetString(magicBytes);
			}
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

		#region IDisposable Methods

		// Make sure BinaryReader cleans up properly.
		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				br.Close();
				br.Dispose();
			}
		}

		#endregion
	}
}
