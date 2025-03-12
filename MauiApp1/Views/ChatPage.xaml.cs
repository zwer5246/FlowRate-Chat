using Microsoft.Maui.Controls;
using System.Diagnostics;
using WebSocketSharp;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.ObjectModel;
using MauiApp1.Classes;
using System.Data;

namespace MauiApp1.Views
{
    public partial class ChatPage : ContentPage
    {
        private ObservableCollection<FlowRateTextMessage> ChatMessages { get; set; } = new ObservableCollection<FlowRateTextMessage>();
        private ObservableCollection<ChatClass> ChatList { get; set; } = new ObservableCollection<ChatClass>();
        private int chatID;

        List<FlowRateTextMessage> messageByfer = new List<FlowRateTextMessage>();
        public bool historyRequested = false;
        public bool connectionApproved = false;
        WebSocket? ws;
        string userName;

        public ChatPage(string userName, string password)
        {
            InitializeComponent();
            ChatListView.ItemsSource = ChatMessages;
            ChatPicker.ItemsSource = ChatList;
        
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
                        await DisplayAlert("Сообщение", $"Ошибка подключения. \n Описание: {message.messageData}", "ОК");
                        await Navigation.PopAsync();
                    });
                    Close();
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.AuthenticationCompleted)
                {
                    Thread.Sleep(500);
                    connectionApproved = true;
                    this.userName = message.userName;
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.HistorySendendStart)
                {
                    historyRequested = true;
                    ChatMessages.Clear();
                    AddMessageToChat(message, false);
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.HistorySendend)
                {
                    AddMessageToChat(message, false);
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.HistorySendendEnd)
                {
                    AddMessageToChat(message, true);
                    historyRequested = false;
                    if (messageByfer.Count > 0)
                    {
                        for (int i = 0; i < messageByfer.Count; i++)
                        {
                            AddMessageToChat(message, true);
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
                        AddMessageToChat(message, true);
                    }
                }
                else if (message.messageType == FlowRateTextMessage.FlowRateMessageType.ChatListSended)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        for (int i = 0; i < message.chatList.Count; i++)
                        {
                            ChatList.Add(message.chatList[i]);
                        }
                        ChatPicker.SelectedIndex = 0;
                        chatID = ChatList[0].chatID;
                    });
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
            Connect(userName, password);
            //ws.Send(WebSocketSpecial.PrepareMessageToSend(FlowRateTextMessage.FlowRateMessageType.HistoryRequest, "52", "User123"));
        }

        public void Send(string data)
        {
            ws.Send(data);
        }

        public void Close()
        {
            ws.Close();
            connectionApproved = false;
            ws = null;
        }

        public async void Disconnect()
        {
            if (ws.IsAlive)
            {
                FlowRateTextMessage requestHistroyMessage = new FlowRateTextMessage(FlowRateTextMessage.FlowRateMessageType.Disconection, "-", this.userName);
                Send(PrepareMessageToSend(requestHistroyMessage));
                ws.Close();
                connectionApproved = false;
                ws = null;
            }
        }

        public void Connect(string userName, string password)
        {
            ws.Connect();
            if (ws.IsAlive)
            {
                FlowRateTextMessage message = new FlowRateTextMessage(FlowRateTextMessage.FlowRateMessageType.AuthenticationRequest, password, userName);
                ws.Send(PrepareMessageToSend(message));
            }
        }

        public async Task RequestMessageHistory(int chatID, int messageCount)
        {
            FlowRateTextMessage requestHistroyMessage = new FlowRateTextMessage(FlowRateTextMessage.FlowRateMessageType.HistoryRequest, $"Chat{chatID}", this.userName);
            requestHistroyMessage.mnessageCount = messageCount;
            Send(PrepareMessageToSend(requestHistroyMessage));
        }

        public static string PrepareMessageToSend(FlowRateTextMessage.FlowRateMessageType messageType, string messageData, string userName, int chatID)
        {
            FlowRateTextMessage message = new FlowRateTextMessage(messageType, messageData, userName);
            return JsonConvert.SerializeObject(message);
        }

        public static string PrepareMessageToSend(FlowRateTextMessage message)
        {
            return JsonConvert.SerializeObject(message);
        }

        private void OnDisappearing(object sender, EventArgs e)
        {

        }

        private void OnAppearing(object sender, EventArgs e)
        {

        }

        public void AddMessageToChat(FlowRateTextMessage message, bool online)
        {
            Dispatcher.Dispatch(() =>
            {
                ChatMessages.Add(message);
                if (online)
                {
                    ChatListView.ScrollTo(message, ScrollToPosition.MakeVisible, true);
                }
            });
        }

        private void SendMessage_Clicked(object sender, EventArgs e)
        {
            if (EntryMessage.Text == " " || EntryMessage.Text == null)
            {
                DisplayAlert("Ошибка", "Сообщение не может быть пустым", "OK");
            }
            else
            {
                if (chatID != 0)
                {
                    ws.Send(PrepareMessageToSend(FlowRateTextMessage.FlowRateMessageType.CommonUserMessage, EntryMessage.Text, userName, chatID));
                    EntryMessage.Text = "";
                }
            }
        }

        private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < ChatList.Count; i++)
            {
                if (ChatList[i].chatName == ChatPicker.SelectedItem.ToString())
                {
                    await RequestMessageHistory(ChatList[i].chatID, 999);
                    break;
                }
            }
        }

    }
}
