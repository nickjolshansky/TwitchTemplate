using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;
using System.IO;

public class TwitchChat : MonoBehaviour
{
    TcpClient Twitch;
    StreamReader reader;
    StreamWriter writer;

    const string URL = "irc.chat.twitch.tv";
    const int PORT = 6667;
    string channelName = "gameshowgroup"; //this needs to be spelled exactly as it appears in your twitch url. https://www.twitch.tv/gameshowgroup
    string oAuth = "I ain't sharin my confidentials"; //this can be found at https://twitchapps.com/tmi/ it will look something like this: oauth:rovcjeygsfg5kudfgjmd9z0f8ytf7jm

    public UnityEvent<string, string> OnChatMessage;

    float pingCounter;

    private void Awake()
    {
        Application.runInBackground = true;
        ConnectToTwitch();
    }

    private void Update()
    {
        //Ping twitch every minute to ensure connection holds
        pingCounter += Time.deltaTime;
        if(pingCounter > 60)
        {
            writer.WriteLine("PING " + URL);
            writer.Flush();
            pingCounter = 0;
        }


        if(Twitch.Available > 0) //new message in chat
        {
            string message = reader.ReadLine();

            if (message.Contains("PRIVMSG")) //PRIVMSG indicates that the message is from a user
            {
                int splitPoint = message.IndexOf("!");
                string user = message.Substring(1, splitPoint - 1);
                splitPoint = message.IndexOf(":", 1);
                string msg = message.Substring(splitPoint + 1);

                msg = FilterBannedWords(msg);

                OnChatMessage?.Invoke(user, msg);
            }
        }
    }

    void ConnectToTwitch()
    {
        Twitch = new TcpClient(URL, PORT);
        reader = new StreamReader(Twitch.GetStream());
        writer = new StreamWriter(Twitch.GetStream());

        writer.WriteLine("PASS " + oAuth);
        writer.WriteLine("NICK " + channelName.ToLower());
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();
    }


    private List<string> bannedWords = new List<string>{
        "anal",
        "anus",
        "arse",
        "ass",
        "ballsack",
        "balls",
        "bastard",
        "bitch",
        "bitches",
        "biatch",
        "bloody",
        "blowjob",
        "blow job",
        "bollock",
        "bollok",
        "boner",
        "boners",
        "boob",
        "bugger",
        "bum",
        "butt",
        "buttplug",
        "clitoris",
        "cock",
        "cocks",
        "cuck",
        "cucks",
        "coon",
        "coons",
        "crap",
        "cunt",
        "damn",
        "dick",
        "dicks",
        "dildo",
        "dyke",
        "dykes",
        "fag",
        "fags",
        "feck",
        "fellate",
        "fellatio",
        "felching",
        "fuck",
        "fucks",
        "f u c k",
        "fudgepacker",
        "fudge packer",
        "flange",
        "Goddamn",
        "God damn",
        "hell",
        "homo",
        "homos",
        "jerk",
        "jizz",
        "knobend",
        "knob end",
        "labia",
        "muff",
        "muffs",
        "nigger",
        "niggers",
        "nigga",
        "niggas",
        "penis",
        "piss",
        "poop",
        "prick",
        "pube",
        "pubes",
        "pussy",
        "queer",
        "queers",
        "scrotum",
        "sex",
        "shit",
        "shits",
        "s hit",
        "sh1t",
        "slut",
        "smegma",
        "spunk",
        "tit",
        "tits",
        "titties",
        "tosser",
        "turd",
        "twat",
        "vagina",
        "wank",
        "whore",
        "wtf"};
    public string FilterBannedWords(string input)
    {
        string[] words = input.Split(' ');

        for (int i = 0; i < words.Length; i++)
            if (bannedWords.Contains(words[i].ToLower()))
                words[i] = new string('*', words[i].Length);

        return string.Join(" ", words);
    }
}
