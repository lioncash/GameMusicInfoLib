using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from Polytracker PTM modules.
	/// </summary>
	public sealed class PtmReader
	{
		// TODO: Read sample information

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to the PTM module</param>
		public PtmReader(string path)
		{
			using (BinaryReader ptm = new BinaryReader(File.OpenRead(path)))
			{
				// Song name
				byte[] songName = new byte[28];
				ptm.Read(songName, 0, songName.Length);
				SongName = Encoding.UTF8.GetString(songName);

				// Totals
				ptm.BaseStream.Seek(0x20, SeekOrigin.Begin);
				TotalOrders = ptm.ReadUInt16();
				TotalInstruments = ptm.ReadUInt16();
				TotalPatterns = ptm.ReadUInt16();
				Channels = ptm.ReadUInt16();

				// Panning
				ptm.BaseStream.Seek(0x40, SeekOrigin.Begin);
				Panning = ptm.ReadByte();
			}
		}

		/// <summary>
		/// The song name stored in the PTM file
		/// </summary>
		public string SongName
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of orders within the PTM module
		/// </summary>
		/// <remarks>File can have between 1-256 orders</remarks>
		public int TotalOrders
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of instruments used in the PTM module
		/// </summary>
		/// <remarks>Can have between 1 to 255 instruments</remarks>
		public int TotalInstruments
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of patterns in the PTM module
		/// </summary>
		/// <remarks>Can have between 1 to 128 patterns</remarks>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of channels/voices in the PTM module
		/// </summary>
		/// <remarks>Can have between 1 to 32 channels</remarks>
		public int Channels
		{
			get;
			private set;
		}

		/// <summary>
		/// An integer that represents channel panning. 
		/// Integer can be between 1 to 15
		/// <para/>
		/// 0 = 3D Left
		/// <para/>
		/// 7 = 3D Middle
		/// <para/>
		/// 15 = 3D Right
		/// </summary>
		public int Panning
		{
			get;
			private set;
		}
	}
}
