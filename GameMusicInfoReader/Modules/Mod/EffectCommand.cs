namespace GameMusicInfoReader.Modules.Mod
{
	/// <summary>
	/// Represents an effect command for a ProTracker module.
	/// </summary>
	public sealed class EffectCommand
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">Type of this effect command.</param>
		/// <param name="value">Data related to the type of this effect command.</param>
		public EffectCommand(EffectCommandType type, int value)
		{
			Type = type;
			Value = value;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Type of this effect command.
		/// </summary>
		public EffectCommandType Type
		{
			get;
			private set;
		}

		/// <summary>
		/// Data for this effect command
		/// </summary>
		public int Value
		{
			get;
			private set;
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("EffectCommand: Type {0} | Data: 0x{1:X2}", Type, Value);
		}

		#endregion
	}
}
