using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using GameMusicInfoReader.Util;

namespace GameMusicInfoReader.Modules.Mod
{
	/// <summary>
	/// Reader for getting info from Protracker MOD modules.
	/// </summary>
	public sealed class ModReader
	{
		// TODO: More info?

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the MOD module.</param>
		public ModReader(string path)
		{
			using (var mod = new EndianBinaryReader(File.OpenRead(path), Endian.Big))
			{
				SongTitle  = Encoding.UTF8.GetString(mod.ReadBytes(20));
				Samples    = GetSamples(mod);
				SongLength = mod.ReadByte();

				// Skip byte. Unimportant
				mod.BaseStream.Position += 1;

				SongPositions = GetSongPositions(mod);
				NumPatterns = SongPositions.Max();
				ModuleID = Encoding.UTF8.GetString(mod.ReadBytes(4));
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// The song title of this MOD file
		/// </summary>
		public string SongTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// The song length in patterns for this MOD file
		/// </summary>
		public int SongLength
		{
			get;
			private set;
		}

		/// <summary>
		/// The 4 character module ID for this MOD file
		/// </summary>
		/// <remarks>
		/// If module has ID's M.K., 8CHN, 4CHN, 6CHN, FLT4 or FLT8, then
		/// the module has 31 instruments
		/// </remarks>
		public string ModuleID
		{
			get;
			private set;
		}

		/// <summary>
		/// Samples within this module.
		/// </summary>
		public ReadOnlyCollection<Sample> Samples
		{
			get;
			private set;
		}

		/// <summary>
		/// Song positions.
		/// </summary>
		/// <remarks>
		/// Each hold a number from 0-63 (or 0-127)
		/// that tell the tracker what pattern to play
		/// at that position.
		/// </remarks>
		public ReadOnlyCollection<byte> SongPositions
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of patterns within this module.
		/// </summary>
		/// <remarks>
		/// This is found by reading through the song
		/// positions and getting the largest number within it.
		/// </remarks>
		public int NumPatterns
		{
			get;
			private set;
		}

		#endregion

		#region Helper Functions

		private static ReadOnlyCollection<Sample> GetSamples(EndianBinaryReader reader)
		{
			var sampleArray = new Sample[31];

			for (int i = 0; i < sampleArray.Length; i++)
				sampleArray[i] = new Sample(reader);

			return new ReadOnlyCollection<Sample>(sampleArray);
		}

		private static ReadOnlyCollection<byte> GetSongPositions(EndianBinaryReader reader)
		{
			var positionArray = new byte[128];

			for (int i = 0; i < positionArray.Length; i++)
				positionArray[i] = reader.ReadByte();

			return new ReadOnlyCollection<byte>(positionArray);
		}

		#endregion
	}
}
