using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
namespace Adam.PetsPlugin.Transelations
{
    public class Transelation
    {

        private Color _color;
        private string _message;
        private string _transelation;
        private readonly RocketPlugin _plugin;
        public Transelation(RocketPlugin plugin, string transelation, params object[] args)
        {
            _transelation = transelation;
            this._plugin = plugin;
            refreshMessage(transelation, args);
        }


        public void refreshMessage(string trans, params object[] args)
        {
            var msg = plugin.Translate(trans, args);
            try
            {
                var cindex = msg.ToLower().LastIndexOf("color=");
                var color = msg.Substring(cindex + 6);
                var hexColor = color;
                if (color.StartsWith("#")) color.Remove(0, 1);
                if (!hexColor.StartsWith("#")) hexColor.Insert(0, "#");
                msg = msg.Remove(cindex);
                if (ColorUtility.TryParseHtmlString(hexColor, out _color))
                {
                }
                else if (_color.tryParseColorName(color))
                {

                }
            }
            catch { }
            _message = msg;
        }


        public void execute(SDG.Unturned.Player player)
        {
            UnturnedChat.Say(UnturnedPlayer.FromPlayer(player), _message, _color);
        }

        /*public void execute()
        {
            UnturnedChat.Say(_message, _color);
        }*/

        public void execute(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            UnturnedChat.Say(player, _message, _color);
        }

        public RocketPlugin plugin
        {
            get
            {
                return _plugin;
            }
        }

        public Color color
        {
            get
            {
                return _color;
            }
        }

        public string message
        {
            get
            {
                return _message;
            }
        }

        public string transelation
        {
            get
            {
                return _transelation;
            }
        }
    }
}
