using System.IO;
using System.Text;

namespace GameMusicInfoReader.Modules
{
	/// <summary>
	/// A reader for getting info from 669 modules.
	/// </summary>
	public sealed class SixSixNineReader
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to the 669 module.</param>
		public SixSixNineReader(string path)
		{
			using (var ssn = new BinaryReader(File.OpenRead(path)))
			{
				HeaderID = Encoding.UTF8.GetString(ssn.ReadBytes(2));
				Comment  = Encoding.UTF8.GetString(ssn.ReadBytes(108));

				// Totals
				TotalSamples = ssn.ReadByte();
				TotalPatterns = ssn.ReadByte();

				// Loop order
				LoopOrder = ssn.ReadByte();
			}
		}

		/// <summary>
		/// The header magic of the 669 file.
		/// <para/>
		/// Should get the string 'if' if it's a normal 669 module.
		/// <para/>
		/// You should get 'JN' if it's an extended 669 module
		/// </summary>
		public string HeaderID
		{
			get;
			private set;
		}

		/// <summary>
		/// The embedded comment in the 669 file (if any)
		/// </summary>
		public string Comment
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number or samples saved.
		/// <para/>
		/// <remarks>Is between or equal to 0-64 sample(s)</remarks>
		/// </summary>
		public int TotalSamples
		{
			get;
			private set;
		}

		/// <summary>
		/// The total number of patterns saved.
		/// <para/>
		/// <remarks>Is between or equal to 0-128 pattern(s)</remarks>
		/// </summary>
		public int TotalPatterns
		{
			get;
			private set;
		}

		/// <summary>
		/// The loop order number within the 669 module.
		/// </summary>
		public int LoopOrder
		{
			get;
			private set;
		}
	}
}
