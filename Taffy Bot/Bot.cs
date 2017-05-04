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
            "Streamer"
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
                    "!roles remove  -   Removes a role from you." +
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
            m_CmdService.CreateCommand("db return_users").Do(async (e) =>
            {
                //SqlService.GetProfile(1265);
                await e.Channel.SendMessage("This is not implemented.");
            });
            m_CmdService.CreateCommand("dbr").Do(async (e) =>
            {
                await e.Channel.SendMessage("No");
                Console.WriteLine("Used");
                SqlService.UploadUser(e.User);
                Console.WriteLine("After");
            });
            m_CmdService.CreateCommand("coinflip").Do(async (e) =>
            {
                Random r = new Random();
                int numb = r.Next(0, 100);
                await e.Channel.SendMessage("The coin flipped and got " + (numb < 50 && numb >= 0 ? "**heads**" : "**tails**") + ".");
            });

            m_Client.ExecuteAndWait(async () =>
            {
                Console.WriteLine("Line");
                await m_Client.Connect("MzA4OTQ1MTIxMDc5MTk3Njk3.C-oPkg.o9s_EOLxPxeT_AWFhpVZNLN6w3Y", TokenType.Bot);
                m_Client.UserJoined += M_Client_UserJoined;
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
