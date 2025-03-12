using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer_Test.Classes
{
    public class TestServer : WebSocketBehavior
    {
        Serilog.Core.Logger logger;
        DataBaseClass dataBaseInstance;
        private Dictionary<string, TestServer> clients = new Dictionary<string, TestServer>();
        private List<UserClass> users = new List<UserClass>();

        public TestServer(Serilog.Core.Logger logger, DataBaseClass dataBaseInstance)
        {
            this.logger = logger;
            this.dataBaseInstance = dataBaseInstance;
            GenerateUsersList();
        }

        protected override void OnOpen()
        {
            logger.Debug("Oppened WebSocket connection with " + Context.UserEndPoint.Address.ToString());
        }

        protected override void OnClose(CloseEventArgs e)
        {
            //...
            //UserClass disconnectedUser = users.Find(user => user.associatedIPaddres == Context.UserEndPoint);
            //logger.Information($"{disconnectedUser.userName} dissconected from server.");
            logger.Debug($"Connection on WebSocket with has been closed.");
        }

        protected override async void OnMessage(MessageEventArgs e)
        {
            FlowRateTextMessage message = JsonConvert.DeserializeObject<FlowRateTextMessage>(e.Data);
            if (message.messageType == FlowRateTextMessage.FlowRateMessageType.AuthenticationRequest)
            {
                bool userFoundedInDataBase = false;
                logger.Debug($"{message.userName} connecting with IP addres {this.Context.UserEndPoint}");
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].userName == message.userName)
                    {
                        if (users[i].password == message.messageData)
                        {
                            userFoundedInDataBase = true;
                            clients[message.userName] = this;
                            FlowRateTextMessage authAnswerMessage = new FlowRateTextMessage(FlowRateTextMessage.FlowRateMessageType.AuthenticationCompleted, null, users[i].userName);
                            this.Send(PrepareMessageToSend(authAnswerMessage));
                            FlowRateTextMessage chatListMessage = new FlowRateTextMessage(FlowRateTextMessage.FlowRateMessageType.ChatListSended, null, users[i].userName);
                            chatListMessage.chatList = await dataBaseInstance.RequestChats();
                            this.Send(PrepareMessageToSend(chatListMessage));
                            logger.Information($"{users[i].userName} with IP addres {this.Context.UserEndPoint} connected.");
                            break;
                        }
                    }
                }
                if (!userFoundedInDataBase)
                {
                    FlowRateTextMessage authAnswerMessage = new FlowRateTextMessage(FlowRateTextMessage.FlowRateMessageType.AuthenticationRejected, "Unknow user", message.userName);
                    //this.Send(PrepareMessageToSend(authAnswerMessage));
                    //this.Sessions.CloseSession(ID);
                    logger.Debug($"{message.userName} with IP addres kicked. Reason: {authAnswerMessage.messageData}.");
                }
            }
            else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.RegistrationRequested)
            {
                
            }
            else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.HistoryRequest)
            {
                logger.Debug(message.userName + " request chat history. Sending...");
                List<FlowRateTextMessage> messagesHistory = await dataBaseInstance.RequestChatHistory(message.messageData, message.mnessageCount);
                for (int i = 0; i < messagesHistory.Count; i++)
                {
                    clients[message.userName].Send(JsonConvert.SerializeObject(messagesHistory[i]));
                }
            }
            else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.Disconection)
            {
                clients.Remove(message.userName);
                logger.Debug($"{message.userName} with IP addres kicked. Reason: Disconnected by user.");
            }
            else
            {
                logger.Debug(Context.UserEndPoint.Address.ToString() + " says: " + message.messageData);
                if (await dataBaseInstance.SaveMessageToDataBase(message.messageChatID, message.messageData, message.userName))
                {
                    Sessions.Broadcast(e.Data);
                }
            }
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            logger.Error("Error on WebSocket connection with " + Context.UserEndPoint.Address.ToString() + ". Message: " + e.ToString());
        }

        private async void GenerateUsersList()
        {
            this.users = await dataBaseInstance.RequestAllUsers();
        }

        private string PrepareMessageToSend(FlowRateTextMessage message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}
