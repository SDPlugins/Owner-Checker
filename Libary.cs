using System;
using System.IO;
using System.Net;
using Rocket.Unturned.Player;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using SDG.Unturned;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Steamworks;
using Rocket.API;

namespace SDPlugins
{
    public class Library
    {
        public static void TellInfo(IRocketPlayer caller, UnturnedPlayer owner, CSteamID ownerid, CSteamID group)
        {
            string charname = Library.GetCharName(ownerid);

            if (Init.Instance.Configuration.Instance.SayPlayerID) UnturnedChat.Say(caller, "Owner ID: " + ownerid.ToString());
            if (owner != null)
            {
                if (Init.Instance.Configuration.Instance.SayPlayerCharacterName) UnturnedChat.Say(caller, "Character Name: " + owner.CharacterName);
            }
            else if (charname != null)
            {
                if (Init.Instance.Configuration.Instance.SayPlayerCharacterName) UnturnedChat.Say(caller, "Character Name: " + charname);
            }
            else
            {
                if (Init.Instance.Configuration.Instance.SayPlayerCharacterName) UnturnedChat.Say(caller, "Could not get character name, player is not online and PlayerInfoLib not installed.");
            }
            if (Init.Instance.Configuration.Instance.SayPlayerSteamName) UnturnedChat.Say(caller, "Steam Name: " + Library.SteamHTMLRequest(ownerid.ToString()));
            if (group != CSteamID.Nil)
            {
                string GroupName = Library.SteamHTMLGroupRequest(group.ToString());
                if (Init.Instance.Configuration.Instance.SayGroupID) UnturnedChat.Say(caller, "Group ID: " + group.ToString());
                if (Init.Instance.Configuration.Instance.SayGroupName) UnturnedChat.Say(caller, "Group Name: " + GroupName);
            }
        }
        public static string GetCharName(CSteamID id)
        {
            string dname = null;
            if (Init.Instance.Configuration.Instance.usePlayerInfoLib)
            {
                Init.ExecuteDependencyCode("PlayerInfoLib", (IRocketPlugin plugin) =>
                {
                    PlayerInfoLibrary.PlayerData data = PlayerInfoLibrary.PlayerInfoLib.Database.QueryById(id, true);
                    dname = data.CharacterName;
                });
            }
            return dname;
        }
        public static string HTTPWebClientRequest(string url)
        {
            WebClient client = new WebClient();

            string text = client.DownloadString(url);
            return text;
        }
        public static string SteamHTMLGroupRequest(string input)
        {
            string html = Library.HTTPWebClientRequest("http://steamcommunity.com/gid/" + input + "/memberslistxml?xml=1");
            string data = Library.getBetween(html, "<groupName>", "</groupName>").Replace(" ", "");
            data = data.Replace("<![CDATA[", "").Replace("]]>", "");
            return data;
        }
        public static void UnturnedHTMLRequest(UnturnedPlayer player, string url, string desc)
        {
            player.Player.channel.send("askBrowserRequest", player.CSteamID, SDG.Unturned.ESteamPacket.UPDATE_RELIABLE_BUFFER, desc, url);     
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        public static string SteamHTMLRequest(string input)
        {
            string html = Library.HTTPWebClientRequest("http://steamcommunity.com/profiles/" + input + "?xml=1");
            string data = Library.getBetween(html, "<steamID>", "</steamID>").Replace(" ", "");
            data = data.Replace("<![CDATA[", "").Replace("]]>", "");
            return data;
        }
    }
}
