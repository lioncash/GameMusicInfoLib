using System.IO;
using System.Text;

namespace GameMusicInfoReader
{
	/// <summary>
	/// Reader for Atari SAP files
	/// </summary>
	public sealed class SapReader
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to the SAP file</param>
		public SapReader(string path)
		{
			string file = File.ReadAllText(path, Encoding.UTF8);

			SongTitle     = GetInfo(file, "NAME").Replace("\"", "");
			Artist        = GetInfo(file, "AUTHOR").Replace("\"", "");
			Date          = GetInfo(file, "DATE").Replace("\"", "");
			SongLength    = GetInfo(file, "TIME").Replace("\"", "");
			InitAddress   = GetInfo(file, "INIT");
			FastPlay      = GetInfo(file, "FASTPLAY");
			PlayerType    = GetInfo(file, "TYPE");
			PlayerAddress = GetInfo(file, "PLAYER");
			IsNtsc        = (GetInfo(file, "NTSC") != "Tag not present within file");
			IsStereo      = (GetInfo(file, "STEREO") != "Tag not present within file");

		}

		/// <summary>
		/// The title of the song within the SAP file
		/// </summary>
		public string SongTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// The artist of the track within the SAP file
		/// </summary>
		public string Artist
		{
			get;
			private set;
		}

		/// <summary>
		/// The date the track was created
		/// </summary>
		public string Date
		{
			get;
			private set;
		}

		/// <summary>
		/// The length of the SAP track
		/// </summary>
		public string SongLength
		{
			get;
			private set;
		}

		/// <summary>
		/// Hexadecimal address of a 6502 routine that initializes the player. 
		/// </summary>
		public string InitAddress
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of scanlines between calls of the player routine.
		/// </summary>
		public string FastPlay
		{
			get;
			private set;
		}

		/// <summary>
		/// Player type. Can be B, C, D or S
		/// </summary>
		public string PlayerType
		{
			get;
			private set;
		}

		/// <summary>
		/// The hexadecimal address where the player routine starts
		/// </summary>
		public string PlayerAddress
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the SAP file is NTSC
		/// </summary>
		public bool IsNtsc
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if the SAP file uses dual POKEY configuration.
		/// </summary>
		public bool IsStereo
		{
			get;
			private set;
		}

		/// <summary>
		/// Parses tags the same way the method in PSFReader does.
		/// </summary>
		private static string GetInfo(string contents, string indexOf)
		{
			// Get the index of the indexOf tag so we know where to start
			// within the file for getting the desired metadata.
			int index = contents.IndexOf(indexOf);

			// Error handling in case a tag isn't present in a file
			if (index == -1)
				return "Tag not present within file";

			// Get a new string (substring) of the entire original string.
			// this shortens up things for us.
			string news = contents.Substring(index);

			// Get the first occurrence of the 0xA character.
			// This signifies the end of a tag
			int nullIndex = news.IndexOf((char)0xA);

			// Perform another substring. This cuts off all text after the 0xA character.
			// It also removes the part of the string that is the 'indexOf' parameter
			return news.Substring(0, nullIndex).Remove(0, indexOf.Length);
		}
	}
}
