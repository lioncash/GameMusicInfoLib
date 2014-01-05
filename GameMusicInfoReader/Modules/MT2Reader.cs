using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from MadTracker 2 modules.
	/// </summary>
	public sealed class MT2Reader
	{
		// TODO: Patterns, Instrument chunk, Automation chunk, Drums

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to an MT2 module.</param>
		public MT2Reader(string path)
		{
			using(BinaryReader mtt = new BinaryReader(File.OpenRead(path)))
			{
				// Header
				byte[] header = new byte[4];
				mtt.Read(header, 0, 4);
				HeaderID = Encoding.UTF8.GetString(header);

				// Skip 4 bytes (safe to ignore)
				mtt.BaseStream.Position += 4;
				
				// Internal version number.
				Version = mtt.ReadInt16();

				// Tracker name.
				byte[] trackerName = new byte[32];
				mtt.Read(trackerName, 0, 32);
				TrackerName = Encoding.UTF8.GetString(trackerName);

				// Track title
				byte[] title = new byte[64];
				mtt.Read(title, 0, 64);
				Title = Encoding.UTF8.GetString(title);

				// Totals
				TotalPositions = mtt.ReadInt16();
				RestartPosition = mtt.ReadInt16();
				TotalPatterns = mtt.ReadInt16();
				TotalTracks = mtt.ReadInt16();

				// Samples/tick
				SamplesPerTick = mtt.ReadInt16();
				TicksPerLine = mtt.ReadByte();
				LinesPerBeat = mtt.ReadByte();

				// Flag conditions
				int flags = mtt.ReadInt32();
				if ((flags & 1)  != 0) HasPackedPatterns = true;
				if ((flags & 2)  != 0) HasAutomation = true;
				if ((flags & 8)  != 0) HasDrumAutomation = true;
				if ((flags & 16) != 0) HasMasterAutomation = true;

				// More totals
				TotalInstruments = mtt.ReadInt16();
				TotalSamples = mtt.ReadInt16();

				// Now get all of the chunks.
				TrackInfo = new List<TrackData>();
				Comments = new List<Message>();
				EnumerateChunks(mtt);
			}
		}

		/// <summary>
		/// The header identifier of this MT2 module.
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// Internal version number of this MT2 module.
		/// </summary>
		public short Version
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the tracker that the module was created in.
		/// </summary>
		public string TrackerName
		{
			get;
			private set;
		}

		/// <summary>
		/// The title of the MT2 module file
		/// </summary>
		public string Title
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of position in the MT2 file
		/// </summary>
		public int TotalPositions
		{
			get;
			private set;
		}

		/// <summary>
		/// Restart position after finishing playback.
		/// </summary>
		public int RestartPosition
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of patterns in the MT2 file
		/// </summary>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of tracks in the MT2 file
		/// </summary>
		public int TotalTracks
		{
			get;
			private set;
		}

		/// <summary>
		/// A number representing how many samples are to be processed per tick
		/// </summary>
		public int SamplesPerTick
		{
			get;
			private set;
		}

		/// <summary>
		/// A number representing how many ticks occur per line
		/// </summary>
		public int TicksPerLine
		{
			get;
			private set;
		}

		/// <summary>
		/// A number representing how many lines per beat there are
		/// </summary>
		public int LinesPerBeat
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns true if the module has packed patterns
		/// </summary>
		public bool HasPackedPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns true if the module has automation
		/// </summary>
		public bool HasAutomation
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns true if the module has drum automation
		/// </summary>
		public bool HasDrumAutomation
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns true if the module has master automation
		/// </summary>
		public bool HasMasterAutomation
		{
			get;
			private set;
		}

		/// <summary>
		/// The total amount of instruments present in the MT2 file
		/// </summary>
		public int TotalInstruments
		{
			get;
			private set;
		}

		/// <summary>
		/// The number of samples present within the MT2 file
		/// </summary>
		public int TotalSamples
		{
			get;
			private set;
		}

		//-- Chunk Parsing --//

		//- TRKS chunk -/

		/// <summary>
		/// Track information for each track in this MT2 module.
		/// </summary>
		public List<TrackData> TrackInfo
		{
			get;
			private set;
		}

		/// <summary>
		/// Comments stored within this MT2 module.
		/// </summary>
		/// <remarks>
		/// TODO: Is it possible for an MT2 module to have
		///       more than one comment in it? Documentation doesn't say.
		/// </remarks>
		public List<Message> Comments
		{
			get;
			private set;
		}

		/// <summary>
		/// Summary chunk data
		/// </summary>
		public Summary SummaryData
		{
			get;
			private set;
		}

		// Enumerates the chunk blocks in the module
		private void EnumerateChunks(BinaryReader br)
		{
			// Grab the length of the drum data and skip over it.
			br.BaseStream.Seek(382, SeekOrigin.Begin);
			int drumDataLen = br.ReadInt16();
			br.BaseStream.Position += drumDataLen;

			// Get length of the chunk data.
			int additionalDataLen = br.ReadInt32();

			// Loop and enumerate chunks
			while (additionalDataLen > 0)
			{
				// Read chunk ID and size
				string id = new string(br.ReadChars(4));
				int size = br.ReadInt32();

				switch (id)
				{
					case "TRKS":
					{
						TrackData data = new TrackData();
						data.MasterVolume      = br.ReadInt16();
						data.TrackVolume       = br.ReadInt16();
						data.UsingEffectBuffer = br.ReadBoolean();
						data.OutputTrack       = br.ReadBoolean();
						data.EffectID          = br.ReadInt16();
						// NOTE: After effect ID are eight 16-bit ints. These are track params.
						// We don't read these because they aren't needed.

						// Add to the list of track data
						TrackInfo.Add(data);

						// Skip track params
						br.BaseStream.Position += 16;
						// Skip rest of track data
						br.BaseStream.Position += (size - 24); // -24 = taking into account the above read vars.
						break;
					}

					case "MSG\0":
					{
						Message msg = new Message();
						msg.ShowComment = br.ReadBoolean();
						msg.Comment = new string(br.ReadChars(size-1)); // -1 because ShowComment is part of the overall data

						// Add to overall comments
						Comments.Add(msg);
						break;
					}

					case "SUM\0":
					{
						Summary summary = new Summary();
						br.BaseStream.Position += 6; // Skip the hash, doubt it's needed.
						summary.Content = new string(br.ReadChars(size-6)); // -6 since the hash is part of the data.

						// Add to property
						SummaryData = summary;
						break;
					}

					default: // Unknown chunk ID
					{
						br.BaseStream.Position += size;
						break;
					}
				}

				// Decrement overall size, as we've processed a chunk.
				additionalDataLen -= (size + 8); // +8 = taking into account the chunk ID and size identifiers.
			}
		}

		#region Chunk Structures

		/// <summary>
		/// Track data chunk (TRKS)
		/// </summary>
		public struct TrackData
		{
			/// <summary>
			/// Master volume for the track.
			/// </summary>
			public int MasterVolume { get; internal set; }

			/// <summary>
			/// Track volume
			/// </summary>
			public int TrackVolume { get; internal set; }

			/// <summary>
			/// Whether or not an effect buffer is being used for the track.
			/// </summary>
			public bool UsingEffectBuffer { get; internal set; }

			/// <summary>
			/// Whether or not this track is outputted.
			/// <para>false = Self</para>
			/// <para>true = Output track</para>
			/// </summary>
			public bool OutputTrack { get; internal set; }

			/// <summary>
			/// The effect ID for this track.
			/// </summary>
			public int EffectID { get; internal set; }

			// NOTE: Technically after the ID is 8 words of individual track params.
			//       ie. Each param = 2 bytes.
		}

		/// <summary>
		/// Message chunk (MSG) for storing a comment.
		/// </summary>
		public struct Message
		{
			/// <summary>
			/// Whether or not to display the message (can be ignored, doesn't matter).
			/// </summary>
			public bool ShowComment { get; internal set; }

			/// <summary>
			/// The actual comment.
			/// </summary>
			public string Comment { get; internal set; }
		}

		/// <summary>
		/// Summary (SUM) chunk
		/// </summary>
		public struct Summary
		{
			/// <summary>
			/// Build summary mask
			/// </summary>
			public long SummaryMask { get; internal set; }

			/// <summary>
			/// Build summary content.
			/// </summary>
			public string Content { get; internal set; }
		}

		#endregion
	}
}

