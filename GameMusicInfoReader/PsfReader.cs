﻿using System.IO;

namespace GameMusicInfoReader
{
	/// <summary>
	/// A class for reading files that are based off of the Playstation PSF format.
	/// </summary>
	public sealed class PsfReader
	{
		// A filestream that represents an xSF file.
		private readonly string file;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to the PSF file</param>
		public PsfReader(string path)
		{
			this.file = File.ReadAllText(path);

			Artist     = GetInfo("artist=");
			Game       = GetInfo("game=");
			SongTitle  = GetInfo("title=");
			Genre      = GetInfo("genre=");
			Copyright  = GetInfo("copyright=");
			Year       = GetInfo("year=");
			Comment    = GetInfo("comment=");
			XSFRipper  = GetXSFRipper(path);
			Volume     = GetInfo("volume=");
			Length     = GetInfo("length=");
			FadeLength = GetInfo("fade=");
		}

		/// <summary>
		/// The artist of the song within the xSF file
		/// </summary>
		public string Artist
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the game the xSF file is from
		/// </summary>
		public string Game
		{
			get;
			private set;
		}

		/// <summary>
		/// The title of the xSF file
		/// </summary>
		public string SongTitle
		{
			get;
			private set;
		}

		/// <summary>
		/// Genre of the game
		/// </summary>
		public string Genre
		{
			get;
			private set;
		}

		/// <summary>
		/// The copyright of the song within the xSF file
		/// </summary>
		public string Copyright
		{
			get;
			private set;
		}

		/// <summary>
		/// The year the track within the xSF file was released
		/// </summary>
		public string Year
		{
			get;
			private set;
		}

		/// <summary>
		/// Any comments left in the xSF file.
		/// Can be non-existent.
		/// </summary>
		public string Comment
		{
			get;
			private set;
		}

		/// <summary>
		/// The name/info of the person who ripped
		/// the xSF file
		/// </summary>
		public string XSFRipper
		{
			get;
			private set;
		}
		private  string GetXSFRipper(string path)
		{
			// Ignore case and don't care about the culture
			if (path.EndsWith("psf", true, null) 
			|| path.EndsWith("psf2", true, null) 
			|| path.EndsWith("minipsf", true, null) 
			|| path.EndsWith("minipsf2", true, null))
				return GetInfo("psfby=");
				
			if (path.EndsWith("gsf", true, null)
			|| path.EndsWith("minigsf", true, null))
				return GetInfo("gsfby=");
				
			if (path.EndsWith("usf", true, null) 
			|| path.EndsWith("miniusf", true, null))
				return GetInfo("usfby=");
				
			if (path.EndsWith("2sf", true, null)
			|| path.EndsWith("mini2sf", true, null))
				return GetInfo("2sfby=");

			// This should never be returned, but is
			// here just in case.
			return "N/A";
		}

		/// <summary>
		/// The relative volume of the xSF
		/// This is an integer and can be parsed as such
		/// </summary>
		public string Volume
		{
			get;
			private set;
		}

		/// <summary>
		/// The length of the song
		/// <remarks>
		/// It can be in one of the following formats:
		/// <para>seconds.decimal</para>
		/// <para>minutes:seconds.decimal</para>
		/// <para>hours:minutes:seconds.decimal</para>
		/// </remarks>
		/// </summary>
		public string Length
		{
			get;
			private set;
		}

		/// <summary>
		/// The length of the fadeout for the track.
		/// <remarks>
		/// 
		/// It can be in one of the following formats:
		/// <para/>
		/// seconds.decimal
		/// <para/>
		/// minutes:seconds.decimal
		/// <para/>
		/// hours:minutes:seconds.decimal
		/// 
		/// </remarks>
		/// </summary>
		public string FadeLength
		{
			get;
			private set;
		}

		/// <summary>
		/// Retrieves information specified by the indexOf string.
		/// <para/>
		/// xSF Files store their metadata as a raw text string, thus, this makes
		/// reading any kind of tag information from them incredibly easy.
		/// It is easy to know what kind of information you're retrieving as every tag
		/// label has it's own identification string.
		/// <para/>
		/// ie. game=, artist=, copyright=, etc.
		/// 
		/// Thus, we can use the indexOf parameter to easily find the index of where
		/// that actual string is within the file.
		/// <para/>
		/// 
		/// This method also removes the indexOf parameter string from the overall
		/// output string so that instead of printing out:
		/// <para/>
		/// "artist=[artist name]" it will simply write it out as if were only:
		/// <para/>
		/// "[Artist name]" (without the [] of course)
		/// </summary>
		/// <param name="indexOf">Identification tag to get within the xSF file</param>
		/// <returns>The desired information string (assuming it's present within the file.</returns>
		private string GetInfo(string indexOf)
		{
			// Get the index of the indexOf tag so we know where to start
			// within the file for getting the desired metadata.
			int index = file.IndexOf(indexOf);

			// Error handling in case a tag isn't present in a file
			if (index == -1)
				return "N/A";

			// Get a new string (substring) of the entire original string.
			// this shortens up things for us.
			string news = file.Substring(index);

			// Get the first occurrence of the 0xA character.
			// This signifies the end of a tag
			int nullIndex = news.IndexOf((char)0xA);

			// Perform another substring. This cuts off all text after the 0xA character.
			// It also removes the part of the string that is the 'indexOf' parameter
			return news.Substring(0, nullIndex).Remove(0, indexOf.Length);
		}
	}
}
