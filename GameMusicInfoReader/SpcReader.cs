using System.IO;

namespace GameMusicInfoReader
{
	/// <summary>
	/// A reader for Super Nintendo SPC files.
	/// </summary>
	public sealed class SpcReader
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to a SNES SPC file.</param>
		public SpcReader(string path)
		{
			using (var spc = new BinaryReader(File.OpenRead(path)))
			{
				// Header
				HeaderID = new string(spc.ReadChars(33));
				spc.BaseStream.Position += 2; // Skip 2 bytes (not needed)

				// ID666 present
				ID666TagInfoPresent = (spc.ReadByte() == 26);

				// Version
				Version = spc.ReadByte();

				// Populate tags
				GetID666Tags(spc);
				GetXid6Tags(spc);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// The header magic for the SPC file.
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// Checks if there is an ID666 info tag chunk present.
		/// </summary>
		public bool ID666TagInfoPresent
		{
			get;
			private set;
		}

		/// <summary>
		/// The version number of the SPC spec that this SPC file uses.
		/// </summary>
		public int Version
		{
			get;
			private set;
		}
		
		/// <summary>
		/// The name of the song.
		/// </summary>
		public string SongName
		{
			get;
			private set;
		}

		/// <summary>
		/// The game that the SPC music data is from
		/// </summary>
		public string GameTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the person who originally dumped the SPC file.
		/// </summary>
		public string DumperName
		{
			get;
			private set;
		}

		/// <summary>
		/// Comment string
		/// </summary>
		public string Comments
		{
			get;
			private set;
		}

		/// <summary>
		/// The date that the SPC file was dumped.
		/// Is in the format: MM/DD/YYYY
		/// </summary>
		public string DumpDate
		{
			get;
			private set;
		}

		/// <summary>
		/// The amount of seconds that the song will play for 
		/// before fading out.
		/// </summary>
		public string SecondsBeforeFade
		{
			get;
			private set;
		}

		/// <summary>
		/// Length of fade in milliseconds
		/// </summary>
		public string FadeLength
		{
			get;
			private set;
		}

		/// <summary>
		/// The artist of the SPC track
		/// </summary>
		public string Artist
		{
			get;
			private set;
		}

		#endregion

		#region ID666 Tag Reading

		private void GetID666Tags(BinaryReader spc)
		{
			if (ID666TagInfoPresent)
			{
				// Seek to beginning.
				spc.BaseStream.Seek(0x2E, SeekOrigin.Begin);

				// Read info now
				SongName          = new string(spc.ReadChars(32));
				GameTitle         = new string(spc.ReadChars(32));
				DumperName        = new string(spc.ReadChars(16));
				Comments          = new string(spc.ReadChars(32));
				DumpDate          = new string(spc.ReadChars(11));
				SecondsBeforeFade = new string(spc.ReadChars(3));
				FadeLength        = new string(spc.ReadChars(5));
				Artist            = new string(spc.ReadChars(32));

			}
			else
			{
				SongName          = "N/A";
				GameTitle         = "N/A";
				DumperName        = "N/A";
				Comments          = "N/A";
				DumpDate          = "N/A";
				SecondsBeforeFade = "N/A";
				FadeLength        = "N/A";
				Artist            = "N/A";
			}
		}

		#endregion

		#region XID6 Tag Reading

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
		private void GetXid6Tags(BinaryReader spc)
		{
			// Seek to just after the 'xid6' characters in the header.
			spc.BaseStream.Seek(0x10204, SeekOrigin.Begin);

			// XID6 chunk size excluding the header.
			int xid6ChunkSize = (spc.ReadInt32() + 3) & ~3;
			
			// Chunk loop
			// Each chunk is 4 bytes per header, if we're above or equal to it, chances
			// are that we still have a tag to read. Length of stream - current position in stream = number of bytes left to read.
			// if we have less than 4, then there's no more chunks to read.
			while ((spc.BaseStream.Length - spc.BaseStream.Position) >= 4)
			{
				int chunkID = spc.ReadByte();
				int type    = spc.ReadByte();

				switch (chunkID)
				{
					case Xid6_Song:
					{
						int lengthOfSongName = spc.ReadInt16();

						if (lengthOfSongName >= 1 && lengthOfSongName <= 0x100)
						{
							XidSong = new string(spc.ReadChars(lengthOfSongName));
						}

						break;
					}

					case Xid6_Game:
					{
						int lengthOfGameName = spc.ReadInt16();

						if (lengthOfGameName >= 1 && lengthOfGameName <= 0x100)
						{
							XidGame = new string(spc.ReadChars(lengthOfGameName));
						}

						break;
					}

					case Xid6_Artist:
					{
						int lengthOfArtistName = spc.ReadInt16();

						if (lengthOfArtistName >= 1 && lengthOfArtistName <= 0x100)
						{
							XidArtist = new string(spc.ReadChars(lengthOfArtistName));
						}

						break;
					}

					case Xid6_Dumper:
					{
						int lengthOfDumperName = spc.ReadInt16();

						if (lengthOfDumperName >= 1 && lengthOfDumperName <= 0x11)
						{
							XidDumper = new string(spc.ReadChars(lengthOfDumperName));
						}

						break;
					}

					case Xid6_Date:
					{
						int dateLen = spc.ReadInt16();
						XidDumpDate = spc.ReadInt32();
						break;
					}

					case Xid6_Emu:
					{
						XidEmulator = spc.ReadByte();
						spc.ReadByte(); // Skip last byte in sub-chunk header.
						break;
					}

					case Xid6_Comment:
					{
						int lengthOfComment = spc.ReadInt16();

						if (lengthOfComment >= 1 && lengthOfComment <= 0x100)
						{
							XidComment = new string(spc.ReadChars(lengthOfComment));
						}

						break;
					}

					case Xid6_OST:
					{
						int lengthOfOst = spc.ReadInt16();

						if (lengthOfOst >= 1 && lengthOfOst <= 0x100)
						{
							XidOstTitle = new string(spc.ReadChars(lengthOfOst));
						}

						break;
					}

					case Xid6_Disc:
					{
						int discNum = spc.ReadInt16();

						if (discNum > 9)
							discNum = 9;

						XidDiscNumber = discNum;

						break;
					}

					case Xid6_Track:
					{
						int track = spc.ReadInt16() >> 8; // Lower byte is an optional character, we don't need to care about this.

						if (track - 1 > 98)
							track = 0;

						XidTrackNumber = track;

						break;
					}

					case Xid6_Publisher:
					{
						int publisherNameLength = spc.ReadInt16();

						if (publisherNameLength >= 1 && publisherNameLength <= 0x100)
						{
							XidPublisher = new string(spc.ReadChars(publisherNameLength));
						}

						break;
					}

					case Xid6_Copyright:
					{
						XidCopyright = spc.ReadInt16();
						break;
					}

					case Xid6_IntroLength:
					{
						spc.ReadInt16(); // Skip last 2 bytes of the sub-chunk header.
						
						int introLen = spc.ReadInt32();

						if (introLen > XidMaxTicks)
							introLen = XidMaxTicks;

						XidIntroLength = introLen;
						break;
					}

					case Xid6_LoopLength:
					{
						spc.ReadInt16(); // Skip last 2 bytes of the sub-chunk header.

						int loopLen = spc.ReadInt32();

						if (loopLen > XidMaxTicks)
							loopLen = XidMaxTicks;

						XidLoopLength = loopLen;
						break;
					}

					case Xid6_EndLength:
					{
						spc.ReadInt16(); // Skip last 2 bytes of the sub-chunk header.

						int endLen = spc.ReadInt32();

						if (endLen > XidMaxTicks)
							endLen = XidMaxTicks;

						XidEndLength = endLen;
						break;
					}

					case Xid6_FadeLength:
					{
						spc.ReadInt16(); // Skip last 2 bytes of the sub-chunk header.

						int fadeLen = spc.ReadInt32();

						if (fadeLen > XidTicksPerMin - 1)
							fadeLen = XidTicksPerMin - 1;

						XidFadeLength = fadeLen;
						break;
					}

					case Xid6_MutedVoices:
					{
						XidMutedChannels = spc.ReadByte();
						spc.ReadByte(); // Skip last byte in sub-chunk header.
						break;
					}

					case Xid6_NumberOfTimesToLoop:
					{
						int loopTimes = spc.ReadByte();

						if (loopTimes < 1)
							loopTimes = 1;
						else if (loopTimes > 9)
							loopTimes = 9;

						XidNumberOfTimesToLoop = loopTimes;

						spc.ReadByte(); // Skip last byte in sub-chunk header.
						break;
					}

					case Xid6_MixingPreAmpLevel:
					{
						int mixLevel = spc.ReadByte();

						if (mixLevel < 32768)
							mixLevel = 32768;
						else if (mixLevel > 524288)
							mixLevel = 524288;

						XidMixingLevel = mixLevel;

						spc.ReadByte(); // Skip last byte in sub-chunk header.
						break;
					}
				}

				xid6ChunkSize -= 4;
			}
		}

		#endregion
	}
}
