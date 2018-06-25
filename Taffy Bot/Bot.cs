using Discord;
using Discord.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taffy_Bot
{
    public class Bot
    {
        const string VersionNotes = 
            "```" +
            "Taffy-Bot (Version 1.7) \n" +
            "Created by Nomnom \n" +
            "----------- \n" +
            "* Added a bot channel to check when the bot goes online. \n" +
            "* Added a welcome message. \n" +
            "```";

        Server m_Server;
        DiscordClient m_Client;
        CommandService m_CmdService;

        List<Message> m_MessagesToDelete = new List<Message>();

        bool m_MessageDeleteWait;

        string[] m_BlacklistedRanks = new string[]
        {
            "Chammy Pro ✓",
            "Retro - Mod",
            "@everyone",
            "Admin",
            "Bot",
            "Fille Movie Maker ✓",
            "Youtuber",
            "Streamer",
            "Enforcer"
        };

        public Bot()
        {
            m_Client = new DiscordClient(input =>
            {
                input.LogLevel = LogSeverity.Info;
                input.LogHandler = Log;
            });

            m_Client.UsingCommands(input =>
            {
                input.PrefixChar = '!';
                input.AllowMentionPrefix = true;
            });

            m_CmdService = m_Client.GetService<CommandService>();

            m_CmdService.CreateCommand("help").Do(async (e) =>
            {
                await e.Channel.SendMessage(
                    "```" +
                    "!help          -   Displays all available commands.\n" +
                    "!roles         -   Displays all available roles.\n" +
                    "!roles add     -   Adds a role to you.\n" +
                    "!roles remove  -   Removes a role from you.\n" +
                    "!coinflip      -   Flips a coin.\n" +
                    "!slap          -   Slaps a user with seaweed!\n" +
                    "!bot           -   Look at the current bot version info.\n" +
                    "```");
            });
            m_CmdService.CreateCommand("roles").Do(async (e) =>
            {
                List<Role> roles = e.Server.Roles.ToList();
                roles.Sort(delegate (Role n, Role n2) { return n.Name.CompareTo(n2.Name); });

                string final = "```";
                for (int i = 0; i < roles.Count; i++)
                {
                    string role = roles[i].Name;
                    if (m_BlacklistedRanks.Contains(role)) continue;
                    final += role;
                    final += '\n';
                }
                final += "```";
                await e.Channel.SendMessage(final);
            });
            m_CmdService.CreateCommand("roles add").Parameter("role", ParameterType.Multiple).Do(async (e) =>
            {
                List<Role> roles = e.Server.Roles.ToList();
                string arg = "";
                for(int i = 0; i < e.Args.Length; i++)
                {
                    arg += e.Args[i] + (i < e.Args.Length - 1 ? " " : "");
                }
                if (e.User.Roles.FirstOrDefault(role=>role.Name == arg) != null)
                {
                    await e.Channel.SendMessage("You already have the role of ***" + arg + "***.");
                    return;
                }
                if (m_BlacklistedRanks.Contains(arg) || roles.FirstOrDefault(role=>role.Name == arg) == null)
                {
                    await e.Channel.SendMessage("The role of ***" + arg + "*** does not exist.");
                    return;
                }

                User user = e.User;
                await user.AddRoles(roles.FirstOrDefault(role => role.Name == arg));
                await e.Channel.SendMessage("You now have the role of " + "***" + arg + "***.");
            });
            m_CmdService.CreateCommand("roles remove").Parameter("role", ParameterType.Multiple).Do(async (e) =>
            {
            List<Role> roles = e.User.Roles.ToList();
            string arg = "";
            for (int i = 0; i < e.Args.Length; i++)
            {
                arg += e.Args[i] + (i < e.Args.Length - 1 ? " " : "");
            }
            if (roles.FirstOrDefault(role => role.Name == arg) == null)
            {
                    await e.Channel.SendMessage("The role of ***" + arg + "** does not exist.");
                    return;
                }

                await e.User.RemoveRoles(roles.FirstOrDefault(role => role.Name == arg));
                await e.Channel.SendMessage("Removed the role of ***" + arg + "***.");
            });
            /*m_CmdService.CreateCommand("db return_users").Do(async (e) =>
            {
                await e.Channel.SendMessage(SqlService.GetProfile(1265));
            });
            m_CmdService.CreateCommand("dbr").Do(async (e) =>
            {
                await e.Channel.SendMessage("No");
                Console.WriteLine("Used");
                SqlService.UploadUser(e.User);
                Console.WriteLine("After");
            });*/
            m_CmdService.CreateCommand("slap").Parameter("user2", ParameterType.Required).Do(async (e) =>
            {
                await e.Channel.SendMessage(e.User.NicknameMention + " slapped " + e.GetArg("user2") + " with a piece of seaweed!");
            });
            m_CmdService.CreateCommand("coinflip").Do(async (e) =>
            {
                Random r = new Random();
                int numb = r.Next(0, 100);
                await e.Channel.SendMessage("The coin flipped and got " + (numb < 50 && numb >= 0 ? "**heads**" : "**tails**") + ".");
            });
            m_CmdService.CreateCommand("bot").Do(async (e) =>
            {
                await e.Channel.SendMessage(VersionNotes);
            });

            m_Client.ExecuteAndWait(async () =>
            {
                Console.WriteLine("Line");
                await m_Client.Connect("token", TokenType.Bot);
                m_Client.UserJoined += M_Client_UserJoined;
                Console.WriteLine("Connected");
            });
        }

        private void M_Client_UserJoined(object sender, UserEventArgs e)
        {
            Channel general = e.Server.GetChannel(307700523195170827);
            general.SendMessage(string.Format("Welcome **{0}** to the server!", e.User.Name));
        }

        public async Task WaitToRemoveFirstMessage(Message _m)
        {
            Console.WriteLine("Waiting");
            m_MessageDeleteWait = true;
            System.Threading.Thread.Sleep(10 * 1000);

            Console.WriteLine("After Wait");

            Channel c = _m.Channel;
            await c.DeleteMessages(new Message[] { _m });

            Console.WriteLine("Deleted");
        }

        private void Log(object _sender, LogMessageEventArgs _e)
        {

        }
    }
}
