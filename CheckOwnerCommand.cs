using SDPlugins;
using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SDPlugins
{
    public class CheckOwner : IRocketCommand
    {
        RaycastHit hit;
        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Name
        {
            get { return "checkowner"; }
        }

        public string Help
        {
            get { return "Check the owner of a certain object"; }
        }

        public string Syntax
        {
            get { return "/checkowner"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out hit, 10, (RayMasks.VEHICLE | RayMasks.BARRICADE | RayMasks.STRUCTURE)))
            {
                byte x;
                byte y;

                ushort plant;
                ushort index;

                BarricadeRegion r;
                StructureRegion s;

                InteractableVehicle vehicle = hit.transform.gameObject.GetComponent<InteractableVehicle>();

                if (BarricadeManager.tryGetInfo(hit.transform, out x, out y, out plant, out index, out r))
                {

                    var bdata = r.barricades[index];

                    UnturnedPlayer owner = UnturnedPlayer.FromCSteamID((CSteamID)bdata.owner);
                    Library.TellInfo(caller, owner, (CSteamID)bdata.owner, (CSteamID)bdata.group);
                }

                else if (StructureManager.tryGetInfo(hit.transform, out x, out y, out index, out s))
                {
                    var sdata = s.structures[index];

                    UnturnedPlayer owner = UnturnedPlayer.FromCSteamID((CSteamID)sdata.owner);
                    Library.TellInfo(caller, owner, (CSteamID)sdata.owner, (CSteamID)sdata.group);
                }

                else if (vehicle != null)
                {
                    if (vehicle.lockedOwner != CSteamID.Nil)
                    {
                        UnturnedPlayer owner = UnturnedPlayer.FromCSteamID(vehicle.lockedOwner);

                        Library.TellInfo(caller, owner, vehicle.lockedOwner, vehicle.lockedGroup);
                        return;
                    }
                    UnturnedChat.Say("Vehicle does not have an owner.");
                }
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>
                {
                  "SDPlugins.checkowner"
                };
            }
        }
    }
}
