using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BHK
{
	public static class ExtentionMethods
	{
		public static bool ContainsPlayer(this Dictionary<int, HealPlayer> healingPlayers, Player Player)
			=> healingPlayers.ContainsKey(Player.PlayerId);

		public static bool ContainsPlayer(this Dictionary<int, HealPlayer> healingPlayers, int PlayerID)
			=> healingPlayers.ContainsKey(PlayerID);
	}
}
