using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SDPlugins
{
    public class Find : IRocketCommand
    {
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
        public List<string> Permissions
        {
            get
            {
                return new List<string>
                {
                  "SDPlugins.find"
                };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            foreach (BarricadeRegion br in BarricadeManager.regions)
            {
                foreach (BarricadeData bd in br.barricades)
                {
                    if (Physics.Raycast(bd.point, Vector3.up, RayMasks.GROUND | RayMasks.GROUND2))
                    {
                        ((UnturnedPlayer)caller).Teleport(bd.point, 0);
                        return;
                    }
                }
            }
        }
    }
}
