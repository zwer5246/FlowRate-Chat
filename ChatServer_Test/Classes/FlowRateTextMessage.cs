using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer_Test.Classes
{
    public class FlowRateTextMessage
    {
        public FlowRateMessageType messageType { get; set; }
        public string? messageData { get; set; }
        public int mnessageCount { get; set; }
        public string userName { get; set; }
        public int messageID { get; set; }
        public DateTime messageDate { get; set; }
        public List<FlowRateMessageSpecialTags> messageSpecialTags { get; set; }
        public int messageChatID { get; set; }
        public List<ChatClass> chatList { get; set; }

        public enum FlowRateMessageType
        {
            CommonUserMessage,
            HistoryRequest,
            HistorySendendStart,
            HistorySendend,
            HistorySendendEnd,
            AuthenticationRequest,
            AuthenticationCompleted,
            AuthenticationRejected,
            RegistrationRequested,
            RegistrationCompleted,
            RegistrationRejected,
            ChatListSended,
            Disconection
        }

        public enum FlowRateMessageSpecialTags
        {

        }

        public FlowRateTextMessage(FlowRateMessageType messageType, string? message, string userName)
        {
            this.messageType = messageType;
            messageData = message;
            this.userName = userName;
            messageDate = DateTime.Now;
        }

        //public FlowRateTextMessage(FlowRateMessageType messageType, string message, string userName, List<FlowRateMessageSpecialTags> messageSpecialTags)
        //{
        //    this.messageType = messageType;
        //    messageData = message;
        //    this.userName = userName;
        //    messageDate = DateTime.Now;
        //    if (messageSpecialTags.Count != 0)
        //    {
        //        this.messageSpecialTags = messageSpecialTags;
        //    }
        //}
    }
}
