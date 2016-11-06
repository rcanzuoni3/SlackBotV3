using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SlackBotV3.CommandTypes
{
	class WeatherType : CommandType
    {
        public override List<string> CommandNames() { return new List<string>() { "weather" }; }
        public override string Help(string commandName) { return "Type weather followed by a city name to see the weather! The default city is conshohocken!"; }
        public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
        public override CommandScope GetCommandScope() { return CommandScope.Global; }

        public override Type GetCommandHandlerType()
        {
            return typeof(Weather);
        }

        class Weather : CommandHandler
        {
            public Weather(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                try
                {
                    string city = String.IsNullOrWhiteSpace(command.Text) ? "Conshohocken" : command.Text;
                    HttpWebRequest request =
                        (HttpWebRequest)
                            WebRequest.Create(
                                String.Format("http://api.openweathermap.org/data/2.5/weather?q=" + city +
                                              "&appid=bd82977b86bf27fb59a04b61b657fb6f"));

                    HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string json = reader.ReadToEnd();
                    response.Dispose();

                    JObject joResponse = JObject.Parse(json);
                    if (joResponse["main"] != null)
                    {
                        string kelvinTemp = joResponse["main"]["temp"].ToString();
                        string farentheitTemp = ((Double.Parse(kelvinTemp) - 273)*9/5 + 32).ToString();
                        city = joResponse["name"].ToString();
                        string emoji;
                        switch (((int)Double.Parse(farentheitTemp))/10)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                emoji = ":snowman:";
                                break;
                            case 4:
                            case 5:
                            case 6:
                                emoji = ":partly_sunny:";
                                break;
                            case 7:
                            case 8:
                            case 9:
                                emoji = ":sunny:";
                                break;
                            default:
                                emoji = ":skull:";
                                break;
                        }
                        SlackBot.Reply(command, city + " : " + farentheitTemp + "F  " + emoji);
                    }
                    else
                        SlackBot.Reply(command, "Are you some kind of stupid? " + city + " is not a city...");

                    return false;
                }
                catch (Exception)
                {
                    SlackBot.Reply(command, "Stop trying to break slackbot");
                    return false;
                }
            }
        }
    }
}