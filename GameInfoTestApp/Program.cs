using System;
using GameMusicInfoReader;

namespace GameInfoTestApp
{
	class Program
	{
		static void Main()
		{
			// TODO: Add tests

			const string file = "2.spc";
			SpcReader reader = new SpcReader(file);

			Console.WriteLine(reader.XidMixingLevel);
			Console.WriteLine(reader.XidLoopLength);
			Console.WriteLine((41 + 3 ) & ~3);

			Console.WriteLine("NAME: "  + reader.XidSong);
			Console.WriteLine("GAME: "  + reader.XidGame);
			Console.WriteLine("OST: "   + reader.XidOstTitle);
			Console.WriteLine("Artist:" + reader.XidArtist);
			Console.WriteLine("Disc Number: "    + reader.XidDiscNumber);
			Console.WriteLine("Track Number: "   + reader.XidTrackNumber);
			Console.WriteLine("Publisher Name: " + reader.XidPublisher);
			Console.WriteLine("Copyright: " + reader.XidCopyright);
			Console.WriteLine("Intro length (ticks): " + reader.XidIntroLength);
			
			// Keep the console window up
			Console.ReadLine();
		}
	}
}
