using Smod2;
using Smod2.Attributes;
using Smod2.EventHandlers;
using Smod2.Events;

namespace BHK
{
	[PluginDetails(
		author = "Evan",
		name = "Better Health Kit",
		description = "Changes the way healthkits work",
		id = "rnen.better.healthkit",
		version = "1.0",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 2
		)]
	class BetterHealthKits : Plugin
	{
		public override void OnDisable() => this.Info(this.Details.name + " - Disabled");
		public override void OnEnable() => this.Info(this.Details.name + " - Enabled");

		public override void Register()
		{
			// Register multiple events
			this.AddEventHandlers(new DamageEvent(), Priority.Low);
			this.AddEventHandlers(new RoundEventHandler(this));
			// Register Command(s)

			// Register config setting(s)
			this.AddConfig(new Smod2.Config.ConfigSetting("BHK_HEAL_INTERVAL",	2f, Smod2.Config.SettingType.FLOAT, true, "The rate of healing (in seconds)"));
			this.AddConfig(new Smod2.Config.ConfigSetting("BHK_HEAL_AMOUNT",	3,	Smod2.Config.SettingType.NUMERIC, true, "The amount of health gained per interval"));
			this.AddConfig(new Smod2.Config.ConfigSetting("BHK_KIT_HEAL_AMOUNT",-1, Smod2.Config.SettingType.NUMERIC, true, "The total amount of health gained per kit"));
		}
	}
}
