using System.IO;
using System.Text;

namespace GameMusicInfoReader
{
	/// <summary>
	/// A reader for Super Nintendo SPC files.
	/// </summary>
	public sealed class SpcReader
	{
		// Filestream representing the spc file.
		private readonly FileStream spc;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to a SNES SPC file.</param>
		public SpcReader(string path)
		{
			spc = File.OpenRead(path);
		}

		/// <summary>
		/// The header magic for the SPC file.
		/// </summary>
		public string HeaderID
		{
			get
			{
				byte[] magic = new byte[33];

				// Read 33 bytes to get header magic.
				spc.Read(magic, 0, 33);

				// Convert bytes to a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(magic);
			}
		}

		/// <summary>
		/// Checks if there is an ID666 info tag chunk present.
		/// </summary>
		public bool ID666TagInfoPresent
		{
			get
			{
				// Seek to byte 35
				spc.Seek(0x23, SeekOrigin.Begin);

				int tagPresent = spc.ReadByte();

				// We have a tag chunk
				if (tagPresent == 26)
					return true;

				// No tag chunk present.
				return false;
			}
		}

		/// <summary>
		/// The version number of the SPC spec that this SPC file uses.
		/// </summary>
		public int Version
		{
			get
			{
				// Seek to byte 36
				spc.Seek(0x24, SeekOrigin.Begin);
				return spc.ReadByte();
			}
		}
		
		/// <summary>
		/// The name of the song.
		/// </summary>
		public string SongName
		{
			get
			{
				if (ID666TagInfoPresent)
				{
					byte[] songName = new byte[32];
					
					// Seek to byte 46
					spc.Seek(0x2E, SeekOrigin.Begin);

					// Read 32 bytes
					spc.Read(songName, 0, 32);

					// Convert bytes to a string
					UTF8Encoding encoding = new UTF8Encoding();
					return encoding.GetString(songName);
				}

				// No info chunk
				return "No ID666 tag present in SPC file. Cannot get song name.";
			}
		}

		/// <summary>
		/// The game that the SPC music data is from
		/// </summary>
		public string GameTitle
		{
			get
			{
				if (ID666TagInfoPresent)
				{
					byte[] gameTitle = new byte[32];

					// Seek to byte 78
					spc.Seek(0x4E, SeekOrigin.Begin);

					// Read 32 bytes
					spc.Read(gameTitle, 0, 32);

					// Convert bytes to a string.
					UTF8Encoding encoding = new UTF8Encoding();
					return encoding.GetString(gameTitle);
				}
				
				// No info chunk
				return "No ID666 tag present in SPC file. Cannot get game title.";
			}
		}

		/// <summary>
		/// The name of the person who originally dumped the SPC file.
		/// </summary>
		public string DumperName
		{
			get
			{
				if (ID666TagInfoPresent)
				{
					byte[] dumperName = new byte[16];

					// Seek to byte 110
					spc.Seek(0x6E, SeekOrigin.Begin);

					// Read 16 bytes
					spc.Read(dumperName, 0, 16);

					// Convert bytes to a string
					UTF8Encoding encoding = new UTF8Encoding();
					return encoding.GetString(dumperName);
				}

				// No info chunk.
				return "No ID666 tag present in SPC file. Cannot get dumper name.";
			}
		}

		public string Comments
		{
			get
			{
				if (ID666TagInfoPresent)
				{
					byte[] comment = new byte[32];

					// Seek to byte 126
					spc.Seek(0x7E, SeekOrigin.Begin);

					// Read 32 bytes
					spc.Read(comment, 0, 32);

					// Convert bytes to a string
					UTF8Encoding encoding = new UTF8Encoding();
					return encoding.GetString(comment);
				}

				// No info chunk.
				return "No ID666 tag present in SPC file. Cannot get comments.";
			}
		}

		/// <summary>
		/// The date that the SPC file was dumped.<br/>
		/// Is in the format: MM/DD/YYYY
		/// </summary>
		public string DumpDate
		{
			get
			{
				if (ID666TagInfoPresent)
				{
					byte[] dumpDate = new byte[11];

					// Seek to byte 158
					spc.Seek(0x9E, SeekOrigin.Begin);

					// Read 11 bytes
					spc.Read(dumpDate, 0, 11);

					// Convert bytes to a string.
					UTF8Encoding encoding = new UTF8Encoding();
					return encoding.GetString(dumpDate);
				}

				// No info chunk
				return "No ID666 tag present in SPC file. Cannot get dump date.";
			}
		}

		/// <summary>
		/// The amount of seconds that the song will play for 
		/// before fading out.
		/// </summary>
		public string SecondsBeforeFade
		{
			get
			{
				if (ID666TagInfoPresent)
				{
					byte[] playFade = new byte[3];

					// Seek to byte 169
					spc.Seek(0xA9, SeekOrigin.Begin);

					// Read 3 bytes
					spc.Read(playFade, 0, 3);

					// Convert bytes to a string
					UTF8Encoding encoding = new UTF8Encoding();
					return encoding.GetString(playFade);
				}
				return "No ID666 Tag Present in SPC file. Cannot get fade in milliseconds.";	
			}
		}

		/// <summary>
		/// The artist of the SPC track
		/// </summary>
		public string Artist
		{
			get
			{
				if (ID666TagInfoPresent)
				{
					byte[] artist = new byte[32];

					// Seek 177 bytes in
					spc.Seek(0xB1, SeekOrigin.Begin);
					
					// Read 32 bytes
					spc.Read(artist, 0, 32);

					// Convert bytes to string.
					UTF8Encoding encoding = new UTF8Encoding();
					return encoding.GetString(artist);
				}

				// No info chunk
				return "No ID666 Tag Present in SPC file. Cannot get artist name.";
			}
		}

		/// <summary>
		/// Checks which emulator the SPC was dumped with
		/// 
		/// <para/>
		/// If 0, then it's unknown.
		/// <para/>
		/// If 1, then it's ZSNES.
		/// <para/>
		/// If 2, then it's Snes9x.
		/// <para/>
		/// etc.
		/// </summary>
		public string EmulatorDumpedWith
		{
			get
			{
				if (ID666TagInfoPresent)
				{
					// Seek to byte 210
					spc.Seek(0xD2, SeekOrigin.Begin);

					int result = spc.ReadByte();

					switch (result)
					{
						case 1:
							return "ZSNES";
						case 2:
							return "Snes9x";
						case 3:
							return "ZST2SPC";
						case 4:
							return "ETC";
						case 5:
							return "SNEShout";
						case 6:
							return "ZSNESW";
						default:
							return "Unknown";
					}
				}
				
				// No info chunk
				return "No ID666 chunk in SPC file, cannot retrieve emulator used to dump SPC.";
			}
		}

		// TODO: Read xid6 tags.
	}
}
