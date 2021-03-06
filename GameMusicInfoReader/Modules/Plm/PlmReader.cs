﻿using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules.Plm
{
	/// <summary>
	/// Reader for Disorder Tracker 2 modules
	/// </summary>
	public sealed class PlmReader
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to a PLM module</param>
		public PlmReader(string path)
		{
			using (var plm = new BinaryReader(File.OpenRead(path)))
			{
				// Header
				HeaderID = Encoding.UTF8.GetString(plm.ReadBytes(4));
				HeaderSize = plm.ReadByte();

				// Song name
				SongName = Encoding.UTF8.GetString(plm.ReadBytes(48));

				// Total channels
				TotalChannels = plm.ReadByte();

				// Max Slide Volume
				plm.BaseStream.Seek(0x38, SeekOrigin.Begin);
				MaxSlideVolume = plm.ReadByte();

				// Soundblaster amp
				SoundblasterAmplification = plm.ReadByte();

				// Initials
				InitialBPM = plm.ReadByte();
				InitialSpeed = plm.ReadByte();

				// Totals
				TotalSamples = plm.ReadByte();
				TotalPatterns = plm.ReadByte();
				TotalOrders = plm.ReadByte();
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Header identifier for PLM modules.
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of bytes in the header
		/// </summary>
		public int HeaderSize
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the song stored in the 
		/// PLM module.
		/// </summary>
		public string SongName
		{
			get;
			private set;
		}

		/// <summary>
		/// The amount of audio channels within the PLM module
		/// </summary>
		public int TotalChannels
		{
			get;
			private set;
		}

		/// <summary>
		/// The maximum volume for vol slides.
		/// </summary>
		public int MaxSlideVolume
		{
			get;
			private set;
		}

		/// <summary>
		/// Soundblaster Amplification, if it returns
		/// 0x40 or 64, then there is no amplification
		/// </summary>
		public int SoundblasterAmplification
		{
			get;
			private set;
		}

		/// <summary>
		/// The initial BPM at the start of a track
		/// </summary>
		public int InitialBPM
		{
			get;
			private set;
		}

		/// <summary>
		/// The initial speed within the PLM module
		/// </summary>
		public int InitialSpeed
		{
			get;
			private set;
		}

		/// <summary>
		/// Total amount of samples within the PLM module
		/// </summary>
		public int TotalSamples
		{
			get;
			private set;
		}

		/// <summary>
		/// Total amount of patterns within the PLM module
		/// </summary>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The total amount of orders within the PLM module
		/// </summary>
		public int TotalOrders
		{
			get;
			private set;
		}

		#endregion
	}
}
