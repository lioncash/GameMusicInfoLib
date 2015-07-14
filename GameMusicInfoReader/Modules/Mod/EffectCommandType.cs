namespace GameMusicInfoReader.Modules.Mod
{
	/// <summary>
	/// Defines possible effect commands per <see cref="Note"/>.
	/// </summary>
	public enum EffectCommandType : byte
	{
		NormalPlayOrArpeggio         = 0x00,
		SlideUp                      = 0x01,
		SlideDown                    = 0x02,
		TonePortamento               = 0x03,
		Vibrato                      = 0x04,
		TonePortamentoAndVolumeSlide = 0x05,
		VibratorAndVolumeSlide       = 0x06,
		Tremolo                      = 0x07,
		Unused1                      = 0x08,
		SetSampleOffset              = 0x09,
		VolumeSlide                  = 0x0A,
		PositionJump                 = 0x0B,
		SetVolume                    = 0x0C,
		PatternBreak                 = 0x0D,
		ExtendedCommand              = 0x0E,
		SetSpeed                     = 0x0F,

		// Extended commands
		SetFilter           = 0xE0,
		FineSlideUp         = 0xE1,
		FineSlideDown       = 0xE2,
		GlissandoControl    = 0xE3,
		SetVibratoWaveform  = 0xE4,
		SetLoop             = 0xE5,
		JumpToLoop          = 0xE6,
		SetTremoloWaveform  = 0xE7,
		Unused2             = 0xE8,
		RetrigNote          = 0xE9,
		FineVolumeSlideUp   = 0xEA,
		FineVolumeSlideDown = 0xEB,
		NoteCut             = 0xEC,
		NoteDelay           = 0xED,
		PatternDelay        = 0xEE,
		InvertLoop          = 0xEF
	}
}
