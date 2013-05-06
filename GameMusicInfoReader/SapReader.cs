using System.IO;

namespace GameMusicInfoReader
{
	/// <summary>
	/// Reader for Atari SAP files
	/// </summary>
	public sealed class SapReader
	{
		// Filestream that represents an Atari SAP file.
		private readonly FileStream sap;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to the SAP file</param>
		public SapReader(string path)
		{
			sap = File.OpenRead(path);
		}

		/// <summary>
		/// The title of the song within the SAP file
		/// </summary>
		public string SongTitle
		{
			get
			{
				return GetInfo("NAME").Replace("\"", "");
			}
		}

		/// <summary>
		/// The artist of the track within the SAP file
		/// </summary>
		public string Artist
		{
			get
			{
				return GetInfo("AUTHOR").Replace("\"", "");
			}
		}

		/// <summary>
		/// The date the track was created
		/// </summary>
		public string Date
		{
			get
			{
				return GetInfo("DATE").Replace("\"", "");
			}
		}

		/// <summary>
		/// The length of the SAP track
		/// </summary>
		public string SongLength
		{
			get
			{
				return GetInfo("TIME").Replace("\"", "");
			}
		}

		/// <summary>
		/// Hexadecimal address of a 6502 routine that initializes the player. 
		/// </summary>
		public string InitAddress
		{
			get
			{
				return GetInfo("INIT");
			}
		}

		/// <summary>
		/// Number of scanlines between calls of the player routine.
		/// </summary>
		public string FastPlay
		{
			get
			{
				return GetInfo("FASTPLAY");
			}
		}

		/// <summary>
		/// Player type. Can be B, C, D or S
		/// </summary>
		public string PlayerType
		{
			get
			{
				return GetInfo("TYPE");
			}
		}

		/// <summary>
		/// The hexadecimal address where the player routine starts
		/// </summary>
		public string PlayerAddress
		{
			get
			{
				return GetInfo("PLAYER");
			}
		}

		/// <summary>
		/// Checks if the SAP file is NTSC
		/// </summary>
		public bool IsNtsc
		{
			get
			{
				// NTSC tag isnt present, therefore it's PAL
				if (GetInfo("NTSC") == "Tag not present within file")
					return false;

				return true;
			}
		}

		/// <summary>
		/// Checks if the SAP file uses dual POKEY configuration.
		/// </summary>
		public bool IsStereo
		{
			get
			{
				// Tag isn't present, therefore it's mono
				if (GetInfo("STEREO") == "Tag not present within file")
					return false;

				return true;
			}
		}

		/// <summary>
		/// Parses tags the same way the method in PSFReader does.
		/// </summary>
		private string GetInfo(string indexOf)
		{
			// Get a streamreader so we can read the file into a string
			StreamReader reader = new StreamReader(sap.Name);

			// Read the entire file to the end
			string entireFile = reader.ReadToEnd();

			// Get the index of the indexOf tag so we know where to start
			// within the file for getting the desired metadata.
			int index = entireFile.IndexOf(indexOf);

			// Error handling in case a tag isn't present in a file
			if (index == -1)
				return "Tag not present within file";

			// Get a new string (substring) of the entire original string.
			// this shortens up things for us.
			string news = entireFile.Substring(index);

			// Get the first occurrence of the 0xA character.
			// This signifies the end of a tag
			int nullIndex = news.IndexOf((char)0xA);

			// Perform another substring. This cuts off all text after the 0xA character.
			// It also removes the part of the string that is the 'indexOf' parameter
			return news.Substring(0, nullIndex).Remove(0, indexOf.Length);
		}
	}
}
