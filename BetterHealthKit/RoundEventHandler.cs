using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BHK
{
	public class HealPlayer
	{
		public int PID;
		public int HealTotalAmount = 0;
		public bool finished = false;

		public HealPlayer(int PlayerID) => PID = PlayerID;
	}

	class DamageEvent : IEventHandlerPlayerHurt
	{
		Dictionary<int, HealPlayer> HealingPlayers => RoundEventHandler.healingPlayers;

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if(HealingPlayers.ContainsPlayer(ev.Player))
			{
				HealingPlayers[ev.Player.PlayerId].finished = true;
			}
		}
	}

	class RoundEventHandler : IEventHandlerMedkitUse, IEventHandlerUpdate
	{
		private readonly BetterHealthKits plugin;
		public static Dictionary<int,HealPlayer> healingPlayers = new Dictionary<int, HealPlayer>();

		static float HealInterval => BetterHealthKits.healInterval;
		static int HealAmount => BetterHealthKits.healAmount;
		static int TotalKitAmount => BetterHealthKits.totalKitAmount;

		public RoundEventHandler(BetterHealthKits plugin) => this.plugin = plugin;

		public void OnMedkitUse(PlayerMedkitUseEvent ev)
		{
			int amount = (TotalKitAmount > 0) ? TotalKitAmount : ev.RecoverHealth;
			if (healingPlayers.ContainsPlayer(ev.Player))
			{
				HealPlayer hp = healingPlayers[ev.Player.PlayerId];
				hp.finished = false;
				hp.HealTotalAmount += amount;
			}
			else
			{
				HealPlayer hpnew = new HealPlayer(ev.Player.PlayerId)
				{
					HealTotalAmount = amount
				};
				healingPlayers.Add(ev.Player.PlayerId, hpnew);
			}
			ev.RecoverHealth = 0;
		}


		DateTime secTimer = DateTime.Now;
		public void OnUpdate(UpdateEvent ev)
		{
			if (plugin.Server.Round.Duration > 0)
				if (secTimer < DateTime.Now)
				{
					if (healingPlayers.Count > 0)
					{
						List<int> l1 = new List<int>();
						foreach (KeyValuePair<int, HealPlayer> kp in healingPlayers)
						{
							if (kp.Value.finished)
								l1.Add(kp.Value.PID);
						}
						if (l1.Count > 0)
							foreach (int i in l1)
								healingPlayers.Remove(i);
					}
					if (PluginManager.Manager.Server.NumPlayers > 0 && healingPlayers.Count > 0)
						foreach (Player p in PluginManager.Manager.Server.GetPlayers())
						{
							if (healingPlayers.ContainsPlayer(p.PlayerId))
							{
								HealPlayer healP = healingPlayers[p.PlayerId];
								if (healP.finished) healP.HealTotalAmount = 0;
								if (p.GetHealth() >= p.TeamRole.MaxHP || healP.HealTotalAmount < 1)
								{
									healP.HealTotalAmount = 0;
									healP.finished = true;
								}
								else if (healP.HealTotalAmount > 0)
								{
									if (healP.HealTotalAmount > HealAmount)
									{
										p.AddHealth(HealAmount);
										healP.HealTotalAmount -= HealAmount;
									}
									else
									{
										p.AddHealth(healP.HealTotalAmount);
										healP.HealTotalAmount = 0;
									}
								}
								if (p.GetHealth() > p.TeamRole.MaxHP)
									p.SetHealth(p.TeamRole.MaxHP);
							}
						}
					secTimer = DateTime.Now.AddSeconds(HealInterval);
				}
		}
	}

	class EarlyWaitinForPlayers : IEventHandlerWaitingForPlayers
	{
		private readonly BetterHealthKits plugin;
		public EarlyWaitinForPlayers(BetterHealthKits plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!plugin.GetConfigBool("BHK_ENABLED"))
				PluginManager.Manager.DisablePlugin(plugin);
			BetterHealthKits.healInterval = plugin.GetConfigFloat("BHK_HEAL_INTERVAL");
			BetterHealthKits.healAmount = plugin.GetConfigInt("BHK_HEAL_AMOUNT");
			BetterHealthKits.totalKitAmount = plugin.GetConfigInt("BHK_KIT_HEAL_AMOUNT");
		}
	}
}
