using System;
using System.IO;
using System.Text;

namespace GameMusicInfoReader
{
	/// <summary>
	/// A reader for Super Nintendo SPC files.
	/// </summary>
	public sealed class SpcReader : IDisposable
	{
		// Filestream representing the SPC file.
		private readonly FileStream spc;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to a SNES SPC file.</param>
		public SpcReader(string path)
		{
			spc = File.OpenRead(path);
			GetXid6Tags();
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
				return Encoding.UTF8.GetString(magic);
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
					return Encoding.UTF8.GetString(songName);
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
					return Encoding.UTF8.GetString(gameTitle);
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
					return Encoding.UTF8.GetString(dumperName);
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
					return Encoding.UTF8.GetString(comment);
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
					return Encoding.UTF8.GetString(dumpDate);
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
					return Encoding.UTF8.GetString(playFade);
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
					return Encoding.UTF8.GetString(artist);
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


		//
		//
		// XID6 Extended tag reading functions
		//
		//

		//
		// NOTE: Some things may be defined in the regular tags (if they fully fit)
		// and may not appear in the extended tags. So it's best to check both first (for length).
		//
		// ie). If the regular tags artist name length < extended tag artist name length, display the extended one, etc.
		//

		//
		// Data fields:
		//
		//       String  = Minimum: 4 characters in length, Maximum: 256 characters in length (including null terminating char in both cases).
		//       Integer = 4-bytes in length (after the sub-chunk header).
		//       Data    = 1 or 2 bytes, usually stored inside the sub-chunk's header.
		//

		//
		// Extended header info types
		//
		private const int Xid6_Song                = 0x01;
		private const int Xid6_Game                = 0x02;
		private const int Xid6_Artist              = 0x03;
		private const int Xid6_Dumper              = 0x04;
		private const int Xid6_Date                = 0x05;
		private const int Xid6_Emu                 = 0x06;
		private const int Xid6_Comment             = 0x07;
		private const int Xid6_OST                 = 0x10;
		private const int Xid6_Disc                = 0x11;
		private const int Xid6_Track               = 0x12;
		private const int Xid6_Publisher           = 0x13;
		private const int Xid6_Copyright           = 0x14;
		private const int Xid6_IntroLength         = 0x30;
		private const int Xid6_LoopLength          = 0x31;
		private const int Xid6_EndLength           = 0x32;
		private const int Xid6_FadeLength          = 0x33;
		private const int Xid6_MutedVoices         = 0x34; // NOTE: A bit is set for every voice muted.
		private const int Xid6_NumberOfTimesToLoop = 0x35;
		private const int Xid6_MixingPreAmpLevel   = 0x36;

		private const int XidMaxTicks    = 383999999; // Max num of allowed ticks. (99:59.99 * 64k)
		private const int XidTicksPerMin  = 3840000;
		private const int XidTicksPerSec  = 64000;     // Number of ticks per second.
		private const int XidTicksPerMsec = 64;        // Number of ticks per millisecond.


		/// <summary>
		/// Extended info: Song name.
		/// </summary>
		public string XidSong
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Game name.
		/// </summary>
		public string XidGame
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Artist name.
		/// </summary>
		public string XidArtist
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Dumper name
		/// </summary>
		public string XidDumper
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Dump Date
		/// </summary>
		public int XidDumpDate
		{
			get; 
			private set;
		}

		/// <summary>
		/// Extended info: What emulator this SPC file was dumped with.
		/// </summary>
		public int XidEmulator
		{
			get;
			set;
		}

		/// <summary>
		/// Extended info: Comments.
		/// </summary>
		public string XidComment
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: OST title.
		/// </summary>
		public string XidOstTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: OST disc number
		/// </summary>
		public int XidDiscNumber
		{
			get; 
			private set;
		}

		/// <summary>
		/// Extended info: Track number.
		/// </summary>
		public int XidTrackNumber
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Publisher name.
		/// </summary>
		public string XidPublisher
		{
			get; 
			private set;
		}

		/// <summary>
		/// Extended info: Copyright year
		/// </summary>
		public int XidCopyright
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Intro length (in ticks).
		/// </summary>
		public int XidIntroLength
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Loop length (in ticks).
		/// </summary>
		public int XidLoopLength
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: End length (in ticks).
		/// </summary>
		public int XidEndLength
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Fade length (in ticks).
		/// </summary>
		public int XidFadeLength
		{
			get; 
			private set;
		}

		/// <summary>
		/// Extended info: Muted channels (each set bit is a muted channel).
		/// </summary>
		public int XidMutedChannels
		{
			get; 
			private set;
		}

		/// <summary>
		/// Extended info: Number of times to loop.
		/// </summary>
		public int XidNumberOfTimesToLoop
		{
			get;
			private set;
		}

		/// <summary>
		/// Extended info: Mixing (preamp) level.
		/// </summary>
		public int XidMixingLevel
		{
			get;
			private set;
		}
		

		// Gets the Xid6 tags and assigns the results to their
		// respective properties.
		private void GetXid6Tags()
		{
			BinaryReader br = new BinaryReader(spc);

			// Seek to just after the 'xid6' characters in the header.
			br.BaseStream.Seek(0x10204, SeekOrigin.Begin);

			// XID6 chunk size excluding the header.
			int xid6ChunkSize = (br.ReadInt32() + 3) & ~3;
			
			// Chunk loop
			// Each chunk is 4 bytes per header, if we're above or equal to it, chances
			// are that we still have a tag to read. Length of stream - current position in stream = number of bytes left to read.
			// if we have less than 4, then there's no more chunks to read.
			while ((br.BaseStream.Length - br.BaseStream.Position) >= 4)
			{
				int chunkID = br.ReadByte();
				int type    = br.ReadByte();

				switch (chunkID)
				{
					case Xid6_Song:
					{
						int lengthOfSongName = br.ReadInt16();

						if (lengthOfSongName < 1 || lengthOfSongName > 0x100)
						{
							// Do nothing, invalid song name.
						}
						else
						{
							char[] songNameChars = new char[lengthOfSongName];
							br.Read(songNameChars, 0, lengthOfSongName);
							XidSong = new string(songNameChars);
						}
						break;
					}

					case Xid6_Game:
					{
						int lengthOfGameName = br.ReadInt16();

						if (lengthOfGameName < 1 || lengthOfGameName > 0x100)
						{
							// Do nothing, invalid length
						}
						else
						{
							char[] gameNameChars = new char[lengthOfGameName];
							br.Read(gameNameChars, 0, lengthOfGameName);

							XidGame = new string(gameNameChars);
						}
						break;
					}

					case Xid6_Artist:
					{
						int lengthOfArtistName = br.ReadInt16();

						if (lengthOfArtistName < 1 || lengthOfArtistName > 0x100)
						{
							// Do nothing, invalid length.
						}
						else
						{
							char[] artistNameChars = new char[lengthOfArtistName];
							br.Read(artistNameChars, 0, lengthOfArtistName);

							XidArtist = new string(artistNameChars);
						}
						break;
					}

					case Xid6_Dumper:
					{
						int lengthOfDumperName = br.ReadInt16();

						if (lengthOfDumperName < 1 || lengthOfDumperName > 0x11)
						{
							// Do nothing, invalid length.
						}
						else
						{
							char[] dumperNameChars = new char[lengthOfDumperName];
							br.Read(dumperNameChars, 0, lengthOfDumperName);

							XidDumper = new string(dumperNameChars);
						}
						break;
					}

					case Xid6_Date:
					{
						int dateLen = br.ReadInt16();
						XidDumpDate = br.ReadInt32();
						break;
					}

					case Xid6_Emu:
					{
						XidEmulator = br.ReadByte();
						br.ReadByte(); // Skip last byte in sub-chunk header.
						break;
					}

					case Xid6_Comment:
					{
						int lengthOfComment = br.ReadInt16();

						if (lengthOfComment < 1 || lengthOfComment > 0x100)
						{
							// Do nothing, invalid comment.
						}
						else
						{
							char[] commentChars = new char[lengthOfComment];
							br.Read(commentChars, 0, lengthOfComment);

							XidComment = new string(commentChars);
						}
						break;
					}

					case Xid6_OST:
					{
						int lengthOfOst = br.ReadInt16();

						if (lengthOfOst < 1 || lengthOfOst > 0x100)
						{
							// Do nothing, invalid OST title.
						}
						else
						{
							char[] ostChars = new char[lengthOfOst];
							br.Read(ostChars, 0, lengthOfOst);

							XidOstTitle = new string(ostChars);
						}
						break;
					}

					case Xid6_Disc:
					{
						int discNum = br.ReadInt16();

						if (discNum > 9)
							discNum = 9;

						XidDiscNumber = discNum;

						break;
					}

					case Xid6_Track:
					{
						int track = br.ReadInt16() >> 8; // Lower byte is an optional character, we don't need to care about this.

						if (track - 1 > 98)
							track = 0;

						XidTrackNumber = track;

						break;
					}

					case Xid6_Publisher:
					{
						int publisherNameLength = br.ReadInt16();

						if (publisherNameLength < 1 || publisherNameLength > 0x100)
						{
							// Do nothing, invalid publisher name.
						}
						else
						{
							char[] publisherNameChars = new char[publisherNameLength];
							br.Read(publisherNameChars, 0, publisherNameLength);
							XidPublisher = new string(publisherNameChars);
						}
						break;
					}

					case Xid6_Copyright:
					{
						XidCopyright = br.ReadInt16();
						break;
					}

					case Xid6_IntroLength:
					{
						br.ReadInt16(); // Skip last 2 bytes of the sub-chunk header.
						
						int introLen = br.ReadInt32();

						if (introLen > XidMaxTicks)
							introLen = XidMaxTicks;

						XidIntroLength = introLen;
						break;
					}

					case Xid6_LoopLength:
					{
						br.ReadInt16(); // Skip last 2 bytes of the sub-chunk header.

						int loopLen = br.ReadInt32();

						if (loopLen > XidMaxTicks)
							loopLen = XidMaxTicks;

						XidLoopLength = loopLen;
						break;
					}

					case Xid6_EndLength:
					{
						br.ReadInt16(); // Skip last 2 bytes of the sub-chunk header.

						int endLen = br.ReadInt32();

						if (endLen > XidMaxTicks)
							endLen = XidMaxTicks;

						XidEndLength = endLen;
						break;
					}

					case Xid6_FadeLength:
					{
						br.ReadInt16(); // Skip last 2 bytes of the sub-chunk header.

						int fadeLen = br.ReadInt32();

						if (fadeLen > XidTicksPerMin - 1)
							fadeLen = XidTicksPerMin - 1;

						XidFadeLength = fadeLen;
						break;
					}

					case Xid6_MutedVoices:
					{
						XidMutedChannels = br.ReadByte();
						br.ReadByte(); // Skip last byte in sub-chunk header.
						break;
					}

					case Xid6_NumberOfTimesToLoop:
					{
						int loopTimes = br.ReadByte();

						if (loopTimes < 1)
							loopTimes = 1;

						if (loopTimes > 9)
							loopTimes = 9;

						XidNumberOfTimesToLoop = loopTimes;

						br.ReadByte(); // Skip last byte in sub-chunk header.
						break;
					}

					case Xid6_MixingPreAmpLevel:
					{
						int mixLevel = br.ReadByte();

						if (mixLevel < 32768)
							mixLevel = 32768;

						if (mixLevel > 524288)
							mixLevel = 524288;

						XidMixingLevel = mixLevel;

						br.ReadByte(); // Skip last byte in sub-chunk header.
						break;
					}
				}


				xid6ChunkSize -= 4;
			}
		}

		#region IDisposable Methods

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				spc.Dispose();
			}
		}


		#endregion
	}
}
