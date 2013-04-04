using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// Reader for getting info from MadTracker 2 modules.
	/// </summary>
	public class MT2Reader
	{
		// TODO: Patterns, Instrument chunk, Automation chunk, Drums

		// Filestream representing an MT2 module.
		private readonly FileStream mtt;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">The path to an MT2 module.</param>
		public MT2Reader(string path)
		{
			mtt = File.OpenRead(path);
		}

		/// <summary>
		/// The name of the tracker that the module was created in.
		/// </summary>
		public string TrackerName
		{
			get
			{
				byte[] trackerName = new byte[32];

				// Seek 10 (0xA) bytes in
				mtt.Seek(0xA, SeekOrigin.Begin);

				// Read 32 bytes
				mtt.Read(trackerName, 0, 32);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(trackerName);
			}
		}

		/// <summary>
		/// The title of the MT2 module file
		/// </summary>
		public string Title
		{
			get
			{
				byte[] trackerName = new byte[64];

				// Seek 42 (0x2A) bytes in
				mtt.Seek(0x2A, SeekOrigin.Begin);

				// Read 64 bytes
				mtt.Read(trackerName, 0, 64);

				// Convert retrieved bytes into a string
				UTF8Encoding encoding = new UTF8Encoding();
				return encoding.GetString(trackerName);
			}
		}

		/// <summary>
		/// The total number of position in the MT2 file
		/// </summary>
		public int TotalPositions
		{
			get
			{
				// Seek 106 (0x6A) bytes in
				mtt.Seek(0x6A, SeekOrigin.Begin);

				return mtt.ReadByte();
			}
		}

		/// <summary>
		/// The total number of patterns in the MT2 file
		/// </summary>
		public int TotalPatterns
		{
			get
			{
				// Seek 110 (0x6E) bytes in
				mtt.Seek(0x6E, SeekOrigin.Begin);

				return mtt.ReadByte();
			}
		}

		/// <summary>
		/// The total number of tracks in the MT2 file
		/// </summary>
		public int TotalTracks
		{
			get
			{
				// Seek 112 (0x70) bytes in
				mtt.Seek(0x70, SeekOrigin.Begin);

				return mtt.ReadByte();
			}
		}

		/// <summary>
		/// A number representing how many samples are to be processed per tick
		/// </summary>
		public int SamplesPerTick
		{
			get
			{
				// Seek 114 (0x72) bytes in
				mtt.Seek(0x72, SeekOrigin.Begin);

				return mtt.ReadByte();
			}
		}

		/// <summary>
		/// A number representing how many ticks occur per line
		/// </summary>
		public int TicksPerLine
		{
			get
			{
				// Seek 116 (0x74) bytes in
				mtt.Seek(0x74, SeekOrigin.Begin);

				return mtt.ReadByte();
			}
		}

		/// <summary>
		/// A number representing how many lines per beat there are
		/// </summary>
		public int LinesPerBeat
		{
			get
			{
				// Seek 117 (0x75) bytes in
				mtt.Seek(0x75, SeekOrigin.Begin);

				return mtt.ReadByte();
			}
		}

		/// <summary>
		/// Returns true if the module has packed patterns
		/// </summary>
		public bool HasPackedPatterns
		{
			get
			{   
				// Seek 118 bytes in
				mtt.Seek(0x76, SeekOrigin.Begin);
				// Read the flag byte
				byte flags = (byte) mtt.ReadByte();

				// If bit 0 is set then we have packed patterns
				if ((flags & 1) != 0)
					return true;
					
				// Bit not set, no packed patterns
				return false;
			}
		}

		/// <summary>
		/// Returns true if the module has automation
		/// </summary>
		public bool HasAutomation
		{
			get
			{
				// Seek 118 bytes in
				mtt.Seek(0x76, SeekOrigin.Begin);
				// Read the flag byte
				byte flags = (byte)mtt.ReadByte();

				// If bit 1 is set then we have packed patterns
				if ((flags & 2) != 0)
					return true;

				// Bit not set, no automation
				return false;
			}
		}

		/// <summary>
		/// Returns true if the module has drum automation
		/// </summary>
		public bool HasDrumAutomation
		{
			get
			{
				// Seek 118 bytes in
				mtt.Seek(0x76, SeekOrigin.Begin);
				// Read the flag byte
				byte flags = (byte)mtt.ReadByte();

				// If bit 3 is set then we have drum automation
				if ((flags & 8) != 0)
					return true;

				// Bit not set, no drum automation
				return false;
			}
		}

		/// <summary>
		/// Returns true if the module has master automation
		/// </summary>
		public bool HasMasterAutomation
		{
			get
			{
				// Seek 118 bytes in
				mtt.Seek(0x76, SeekOrigin.Begin);
				// Read the flag byte
				byte flags = (byte)mtt.ReadByte();

				// If bit 4 is set then we have drum automation
				if ((flags & 16) != 0)
					return true;

				// Bit not set, no drum automation
				return false;
			}
		}

		/// <summary>
		/// The total amount of instruments present in the MT2 file
		/// </summary>
		public int TotalInstruments
		{
			get
			{
				// Seek 122 (0x7A) bytes in
				mtt.Seek(0x7A, SeekOrigin.Begin);

				return mtt.ReadByte();
			}
		}

		/// <summary>
		/// The number of samples present within the MT2 file
		/// </summary>
		public int TotalSamples
		{
			get
			{
				// Seek 124 (0x7C) bytes in
				mtt.Seek(0x7C, SeekOrigin.Begin);

				return mtt.ReadByte();
			}
		}

		//-- Chunk Parsing --//

		//- TRKS chunk -/

		/// <summary>
		/// The master volume of the track 
		/// </summary>
		public string MasterVolume
		{
			get
			{
				// Get a streamreader so we can read the file into a string
				StreamReader reader = new StreamReader(mtt.Name);

				// Read the entire file to the end
				string entireFile = reader.ReadToEnd();

				// Get the index of the string so we know where to start
				// within the file for getting the desired data.
				int index = entireFile.IndexOf("TRKS") + 4;

				// Error handling
				if (index == -1)
					return "TRKS chunk does not exist. Cannot get value";

				mtt.Seek(index, SeekOrigin.Begin);

				return mtt.ReadByte().ToString();
			}
		}

		
		// -- MSG chunk parsing -- //

		/// <summary>
		/// The comment embedded in a MT2 file
		/// </summary>
		public string Comment
		{   // NOTE: Would seeking to the start index be faster ?
			get
			{
				// Get a streamreader so we can read the file into a string
				StreamReader reader = new StreamReader(mtt.Name);

				// Read the file into a string
				string entireFile = reader.ReadToEnd();

				// Get the start index (when "MSG" is first encountered).
				// The + 9 is representing extra chars after "MSG" that aren't a part of the main string,
				// so skip them
				int startIndex = entireFile.IndexOf("MSG") + 9;

				// Error handling
				if (startIndex == -1)
					return "No comment exists within this module";

				// Get the end index (When the next chunk, "SUM", is encountered).
				// The -2 represents the two empty bytes (0x00) before the "SUM" chunk.
				// There's no point in printing these, so skip back over them.
				int endIndex = entireFile.LastIndexOf("SUM") - 2;

				// Overall length of the string
				int length = (endIndex - startIndex);

				// Return substring containing the comment
				return entireFile.Substring(startIndex, length);
			}
		}

		/// <summary>
		/// The summary of the comment
		/// </summary>
		public string Summary
		{
			get
			{
				// Get a streamreader so we can read the file into a string
				StreamReader reader = new StreamReader(mtt.Name);

				// Read the file into a string
				string entireFile = reader.ReadToEnd();

				// Get the start index (when "SUM" is first encountered).
				// The +14 is representing extra chars after the "S" in "SUM" 
				// that aren't a part of the main string, so skip them
				int startIndex = entireFile.LastIndexOf("SUM") + 14;

				// Error handling
				if (startIndex == -1)
					return "No summary exists in this module";

				// Get the end index (When the next chunk, "TMAP", is encountered).
				// The -8 represents the eight empty bytes (0x00) before the "TMAP" chunk.
				// There's no point in printing these, so skip back over them.
				int endIndex = entireFile.LastIndexOf("TMAP") - 8;

				// Overall length of the string
				int length = (endIndex - startIndex);

				// Return substring containing the comment
				return entireFile.Substring(startIndex, length);
			}
		}
	}
}

