﻿using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// A reader for getting info from ExtremeTracker modules.
	/// </summary>
	public sealed class AmsReader
	{
		// TODO: Retrieving comments, sample info, etc

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to an AMS module.</param>
		public AmsReader(string path)
		{
			using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
			{
				// Header
				byte[] header = new byte[7];
				br.Read(header, 0, header.Length);
				HeaderID = Encoding.UTF8.GetString(header);

				// Num samples
				br.BaseStream.Seek(0xA, SeekOrigin.Begin);
				TotalSamples = br.ReadByte();

				// Num Patterns
				TotalPatterns = br.ReadUInt16();

				// Num positions
				TotalPositions = br.ReadUInt16();

				// Num virtual midi channels
				TotalVirtualMidiChannels = br.ReadUInt16();
			}
		}

		/// <summary>
		/// The header identifier string.
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// The total amount of samples stored within
		/// this AMS module (can be from 1-255).
		/// </summary>
		public int TotalSamples
		{
			get;
			private set;
		}

		/// <summary>
		/// The total amount of patterns stored within 
		/// this module (can be from 1-65535).
		/// </summary>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of positions within
		/// this module (can be from 1-65535).
		/// </summary>
		public int TotalPositions
		{
			get;
			private set;
		}

		/// <summary>
		/// The number of virtual MIDI channels within
		/// this module (can be from 0-31)
		/// </summary>
		public int TotalVirtualMidiChannels
		{
			get;
			private set;
		}
	}
}
