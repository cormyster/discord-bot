using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using System.Linq;
using DSharpPlus.Entities;
using DSharpPlus;

namespace Discord_Bot
{
    //Commands are methods that are recognised by the bot to be called by users in a server. Commands are marked with an attribute. The user calls prefix + attribute in order to execute the command
    //ComandContext contains context about where the command was posted, who posted it, which channel etc.
    class Commands : BaseCommandModule //inherit from the Dsharp command module
    {
        string jsonPath = @"C:\Users\Corey\OneDrive\Nerd shit\Discord Bot\Discord Bot\Facts.json";
        public dynamic GetJson() //Allows easy access to the json file
        {
            dynamic commands = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(jsonPath)); //Converts the text from JSON to a c# object
            return commands;
        }

        public void WriteJson(string Key, string Value)
        {
            var rawJson = System.IO.File.ReadAllText(jsonPath);
            JObject Json = JObject.Parse(rawJson);
            Json[Key] = Value;
            System.IO.File.WriteAllText(jsonPath, Json.ToString());
        }
        [Command("ask")]
        [Description("Overload for !ask command")]
        public async Task AskQuestion(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Please include the keyword you want to search. e.g. !ask worlds tallest mountain");
        }
        [Command("ask")]
        [Description("The user asks a bot a question")]
        public async Task AskQuestion(CommandContext ctx, params string[] questionArray) //params allows an infinite overload
        {
            string question = "";
            foreach (string s in questionArray) { question += s + "_"; }
            question = question.Remove(question.Length - 1); //remove the last space
            string rawAnswer = GetJson()[question];
            if (rawAnswer != null)
            {
                string answer = rawAnswer.Replace("_", " ");
                await ctx.Channel.SendMessageAsync(question + " is: \"" + answer + "\" according to my databases");
            }
            else
            {
                //search wikipedia 
                //if no results:
                await ctx.Channel.SendMessageAsync("Sorry! I don't know what that is, try adding the command with the !define command");
            }
        }

        [Command("define")]
        [Description("Overload for !define command")]
        public async Task DefineQuestion(CommandContext ctx) //overload incase someone uses the command wrong
        {
            await ctx.Channel.SendMessageAsync("Please include the keyword you want to define. e.g. !define worlds tallest mountain");
        }
        [Command("define")]
        [Description("The user can input a definition")]
        public async Task DefineQuestion(CommandContext ctx, params string[] statementArray)
        {
            bool overRide = true;
            string statement = "";
            foreach (string s in statementArray) { statement += s + "_"; }
            statement = statement.Remove(statement.Length - 1);
            var interactivity = ctx.Client.GetInteractivity();
            if (GetJson()[statement] != null)
            {
                overRide = false;
                
                string statementString = "";
                foreach (string s in statementArray) { statementString += s + " "; }
                await ctx.Channel.SendMessageAsync("A definition for " + statementString + "already exists! Would you like to override? (Y/N)");
                var overrideResponse = await interactivity.WaitForMessageAsync(c => c.Author.Id == ctx.User.Id);
                if (overrideResponse.Result.Content.ToLower() == "y")
                {
                    overRide = true;
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("Keeping the original definition");
                }
            }
            if (overRide)
            {
                await ctx.Channel.SendMessageAsync("What's the definition?");
                var response = await interactivity.WaitForMessageAsync(c => c.Author.Id == ctx.User.Id);
                if (response.Result != null)
                {
                    WriteJson(statement, response.Result.Content);
                    await ctx.Channel.SendMessageAsync("Added!");
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("Timeout");
                }
            }


        }

        [Command("delete")]
        [Description("Deletes a definitiongoogle")]
        public async Task DeleteQuestion(CommandContext ctx, params string[] factList)
        {
            string fact = "";
            foreach(string s in factList) { fact += s + "_"; }
            fact = fact.Remove(fact.Length - 1);
            var rawJson = System.IO.File.ReadAllText(jsonPath);
            JObject Json = JObject.Parse(rawJson);
            Json.Remove(fact);
            System.IO.File.WriteAllText(jsonPath, Json.ToString());
            
            await ctx.Channel.SendMessageAsync("Deleted!");
        }

        [Command("json")]
        [Description("Outputs the whole json file - admins only")]
        public async Task OutputJson(CommandContext ctx)
        {
            Console.WriteLine(ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.Administrator));
            if(ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.Administrator)) //permissions code for an admin
            {
                string json = System.IO.File.ReadAllText(jsonPath);
                await ctx.Channel.SendMessageAsync(json);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("Sorry, you need to be an admin to do that");
            }


        }

    }
}
