using System;
using System.Threading.Tasks;
using DSharpPlus; //c# wrapper for the api
using DSharpPlus.CommandsNext; //handling commands
using Newtonsoft.Json; //JSON file handling
using DSharpPlus.Interactivity; //alows interaction with a bot after a command has been called
using DSharpPlus.EventArgs; //handles events
using System.Reflection;

namespace Discord_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot(@"C:\Users\Corey\OneDrive\Nerd shit\Discord Bot\Discord Bot\Authentication.json"); //using objects in case i want multiple bots
            
            bot.MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
