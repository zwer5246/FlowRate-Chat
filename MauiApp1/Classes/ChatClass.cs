using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Classes
{
    public class ChatClass
    {
        public string chatName { get; set; }
        public int chatID { get; set; }
        public FlowRateChatType chatType = FlowRateChatType.Unknown;

        public enum FlowRateChatType
        {
            TextChat,
            VideoAndTextChat,
            VideoChat,
            VoiceChat,
            Unknown
        }

        public ChatClass(string chatName, int chatID)
        {
            this.chatName = chatName;
            this.chatID = chatID;
        }
    }
}
