using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using MauiApp1.Classes;
using MauiApp1;

namespace MauiApp1.Views
{
    public class WebSocketSpecial : ChatPage
    {
        List<FlowRateTextMessage> messageByfer = new List<FlowRateTextMessage>();
        public bool historyRequested = true;
        bool connectionApproved = false;
        WebSocket? ws;
        string? userName;
        string? password;

        public WebSocketSpecial(string userName, string password)
        {
            ws = new WebSocket("ws://127.0.0.1:8082/TestServer");
            ws.OnOpen += (sender, e) =>
            {

            };
            ws.OnMessage += async (sender, e) =>
            {
                FlowRateTextMessage message = JsonConvert.DeserializeObject<FlowRateTextMessage>(e.Data);
                if (message.messageType == FlowRateTextMessage.FlowRateMessageType.AuthenticationRejected)
                {
                    Dispatcher.Dispatch(async () =>
                    {
                        await DisplayAlert("Сообщение", "Ошибка подключения. Возможно вы указали неверные данные для входа.", "ОК");
                        await Navigation.PopAsync();
                    });
                    Close();
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.AuthenticationCompleted)
                {
                    Thread.Sleep(500);
                    connectionApproved = true;
                    await RequestMessageHistory(chatIdintifier, 999);   
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.HistorySendendStart)
                {
                    historyRequested = true;
                    AddMessageToChat(message);
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.HistorySendend)
                {
                    AddMessageToChat(message);
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.HistorySendendEnd)
                {
                    AddMessageToChat(message);
                    historyRequested = false;
                    if (messageByfer.Count > 0)
                    {
                        for (int i = 0; i < messageByfer.Count; i++)
                        {
                            AddMessageToChat(messageByfer[i]);
                        }
                        messageByfer.Clear();
                    }
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.CommonUserMessage)
                {
                    if (historyRequested)
                    {
                        messageByfer.Add(message);
                    }
                    else
                    {
                        AddMessageToChat(message);
                    }
                }
            };
            ws.OnClose += (sender, e) =>
            {
                Dispatcher.Dispatch(async () =>
                {
                    await DisplayAlert("Сообщение", "Вы отключены.", "ОК");
                    await Navigation.PopAsync();
                });
            };
        }

        public async void Send(string data)
        {
            ws.Send(data);
        }

        public async void Close()
        {
            ws.Close();
            connectionApproved = false;
            ws = null;
        }

        public async Task<bool> Connect(string userName, string password)
        {
            ws.Connect();
            this.userName = userName;
            if (ws.IsAlive)
            {
                ws.Send(PrepareMessageToSend(FlowRateTextMessage.FlowRateMessageType.AuthenticationRequest, password, userName));
                for (int i = 0; i < 10; i++)
                {
                    if (connectionApproved)
                    {
                        this.userName = userName;
                        return true;
                    }
                    Thread.Sleep(200);
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public async Task RequestMessageHistory(string chatIdentifier, int messageCount)
        {
            FlowRateTextMessage requestHistroyMessage = new FlowRateTextMessage(FlowRateTextMessage.FlowRateMessageType.HistoryRequest, chatIdentifier, this.userName);
            requestHistroyMessage.mnessageCount = messageCount;
            Send(WebSocketSpecial.PrepareMessageToSend(requestHistroyMessage));
        }

        public static string PrepareMessageToSend(FlowRateTextMessage.FlowRateMessageType messageType, string messageData, string userName)
        {
            FlowRateTextMessage message = new FlowRateTextMessage(messageType, messageData, userName);
            return JsonConvert.SerializeObject(message);
        }

        public static string PrepareMessageToSend(FlowRateTextMessage message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}
