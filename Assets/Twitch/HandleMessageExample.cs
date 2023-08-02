using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMessageExample : MonoBehaviour
{
    TwitchChat twitchChat;

    private void Start()
    {
        twitchChat = FindObjectOfType<TwitchChat>();
        twitchChat.OnChatMessage.AddListener(HandleMessage);
    }

    void HandleMessage(string user, string msg)
    {
        print($"{user} said {msg}");
    }
}
