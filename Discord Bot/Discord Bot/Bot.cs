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
    class Bot
    {
        
        public Bot(string path)
        {
            jsonPath = path;
        }
        string jsonPath;
        public DiscordClient client { get; private set; }
        public DiscordConfiguration discordConfig { get; private set; }
        public CommandsNextExtension commands { get; private set; }
        public InteractivityExtension interactivity { get; private set; }
        public dynamic GetJson() //Allows easy access to the json file
        {
            dynamic items = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(jsonPath)); //Converts the text from JSON to a c# object
            return items;
        }

        

        public async Task MainAsync(string[] args)
        {
            discordConfig = new DiscordConfiguration //configures the client connecting
            {
                Token = GetJson().token, //Authentication token taken from the json file
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true, //Logs everything in console
                LogLevel = LogLevel.Debug
            };
            client = new DiscordClient(discordConfig);
            client.Ready += OnClientReady; //subscribes to the client.ready
            client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(1) //After 1 minute the bot will stop looking for a response
            });//enables the interactivity module on the client

            commands = client.UseCommandsNext(new CommandsNextConfiguration //enables the use of commands for the client
            {
                StringPrefixes = new string[] { GetJson().prefix } //Commands are only recognised if it starts with !
            });

            client.MessageCreated += OnMsg;
            commands.RegisterCommands<Commands>();

            await client.ConnectAsync(); //Connects the bot to the discord API
            
            await Task.Delay(-1); //fixes a bug where the bot auto turns off
        }

        private Task OnMsg(MessageCreateEventArgs e) //For debugging
        {
            Console.WriteLine(e.Author.Username.ToString() + ": " + e.Message.Content.ToString());
            return Task.CompletedTask;
        }
        private Task OnClientReady(ReadyEventArgs e) //Triggers when the client is ready
        {
            Console.WriteLine(Environment.NewLine + "Bot online!" + Environment.NewLine);
            return Task.CompletedTask; //basically returning null for a task
        }
    }
}
