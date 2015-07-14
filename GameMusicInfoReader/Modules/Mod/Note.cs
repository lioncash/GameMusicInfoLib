using System;

namespace GameMusicInfoReader.Modules.Mod
{
	/// <summary>
	/// Represents a single note within a <see cref="Pattern"/>.
	/// </summary>
	public sealed class Note
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Note data.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="data"/> is null.</exception>
		/// <exception cref="ArgumentException">If <paramref name="data"/> does not have a length of 4.</exception>
		public Note(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			if (data.Length != 4)
				throw new ArgumentException("Note data must be 4 bytes in size", "data");

			SampleNumber = (data[0] & 0xF0) | (data[2] >> 4);
			NotePeriod   = ((data[0] & 0x0F) << 8) | data[1];

			EffectCommand = InitializeEffectCommand((byte)(data[2] & 0xF), data[3]);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Sample number indicating which
		/// sample to use for this note.
		/// </summary>
		public int SampleNumber
		{
			get;
			private set;
		}

		/// <summary>
		/// The period of this note.
		/// </summary>
		public int NotePeriod
		{
			get;
			private set;
		}

		/// <summary>
		/// The <see cref="EffectCommand"/> for this note.
		/// </summary>
		public EffectCommand EffectCommand
		{
			get;
			private set;
		}

		#endregion

		#region Helper Functions

		private static EffectCommand InitializeEffectCommand(byte effectCommand, byte effectData)
		{
			// Check if the command is an extended command.
			if (effectCommand == 0xE)
			{
				effectCommand >>= 4;
				effectData &= 0xF;
			}

			if (!Enum.IsDefined(typeof (EffectCommandType), effectCommand))
				throw new ArgumentException("Invalid effect command.", "effectCommand");

			return new EffectCommand((EffectCommandType)effectCommand, effectData);
		}

		#endregion
	}
}
