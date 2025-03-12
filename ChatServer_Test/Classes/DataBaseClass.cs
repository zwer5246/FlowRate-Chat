using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer_Test.Classes
{
    public class DataBaseClass
    {
        static string connectionString = "Server=192.168.1.210;Database=chat;User Id=sa;Password=123PassworD;TrustServerCertificate=True;MultipleActiveResultSets=True;";
        SqlConnection dataBaseConnection = new SqlConnection(connectionString);
        Serilog.Core.Logger logger;

        public DataBaseClass(Serilog.Core.Logger logger)
        {
            this.logger = logger;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private async Task CheckDataBaseConnectionAsync()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.OpenAsync();
                await Task.Delay(4000);
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    logger.Fatal("Connection with SQL Server instance lost.");
                    Environment.Exit(0);
                }
            }
        }

        public async Task<bool> ConnectToDataBase()
        {
            try
            {
                await dataBaseConnection.OpenAsync();
                logger.Information("Connection to SQL Server successful estabilished.");
                //Task.Run(async () =>
                //{
                //    while (true)
                //    {
                //        await CheckDataBaseConnectionAsync();
                //    }
                //});
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Cant connect to SQL Server. " + ex.Message);
            }
            finally
            {
                if (dataBaseConnection.State != System.Data.ConnectionState.Open)
                {
                    logger.Fatal("Server cant start without SQL Server instance.");
                }
            }
            return false;
        }

        public async Task<bool> SaveMessageToDataBase(int ChatID, string messageData, string userName)
        {
            string sqlExpression = string.Format("insert into {0} (messageData, userName) values ('{1}', '{2}')", $"Chat{ChatID}", messageData, userName);
            SqlCommand cmd = new SqlCommand(sqlExpression, dataBaseConnection);
            int number = await cmd.ExecuteNonQueryAsync();

            if (number <= 0)
            {
                logger.Warning("Failed to save message in DataBase.");
                return false;
            }
            return true;
        }

        public async Task<List<FlowRateTextMessage>> RequestChatHistory(string ChatID, int messagesCount)
        {
            string sqlExpression = string.Format("select count(*) from {0}", $"Chat{ChatID}");
            SqlCommand cmd = new SqlCommand(sqlExpression, dataBaseConnection);
            int returnedRows = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            if (returnedRows < messagesCount)
            {
                messagesCount = returnedRows;
            }

            sqlExpression = string.Format("select * from {0}", $"Chat{ChatID}");
            cmd = new SqlCommand(sqlExpression, dataBaseConnection);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            List<FlowRateTextMessage> messagesHistory = new List<FlowRateTextMessage>();

            int count = 0;

            if (reader.HasRows)
            {
                while (await reader.ReadAsync() || count != messagesCount)
                {
                    FlowRateTextMessage message = new FlowRateTextMessage
                    (
                        FlowRateTextMessage.FlowRateMessageType.HistorySendend,
                        reader.GetValue(1).ToString(),
                        reader.GetValue(2).ToString()
                    );
                    messagesHistory.Add(message);
                    count++;
                }
            }
            await reader.CloseAsync();

            messagesHistory.Reverse();
            messagesHistory[0].messageType = FlowRateTextMessage.FlowRateMessageType.HistorySendendStart;
            messagesHistory[messagesHistory.Count - 1].messageType = FlowRateTextMessage.FlowRateMessageType.HistorySendendEnd;
            return messagesHistory;
        }

        public async Task<List<UserClass>> RequestAllUsers()
        {
            string sqlExpression = "select * from users";
            SqlCommand cmd = new SqlCommand(sqlExpression, dataBaseConnection);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            List<UserClass> registratedUsers = new List<UserClass> ();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    UserClass user = new UserClass
                    (
                        Convert.ToString(reader.GetValue(1)),
                        Convert.ToString(reader.GetValue(5)),
                        Convert.ToDateTime(reader.GetValue(2)),
                        Convert.ToString(reader.GetValue(3))
                    );

                    registratedUsers.Add(user);
                }
            }
            await reader.CloseAsync();

            return registratedUsers;
        }

        public async Task<UserClass> RequestUser(string userName, string password)
        {
            string sqlExpression = "select * from users";
            SqlCommand cmd = new SqlCommand(sqlExpression, dataBaseConnection);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    if (userName == Convert.ToString(reader.GetValue(1)) & password == Convert.ToString(reader.GetValue(5)))
                    {
                        UserClass user = new UserClass
                        (
                            Convert.ToString(reader.GetValue(1)),
                            Convert.ToString(reader.GetValue(5)),
                            Convert.ToDateTime(reader.GetValue(2)),
                            Convert.ToString(reader.GetValue(3))
                        );
                        await reader.CloseAsync();
                        return user;
                    }
                }
            }
            return new UserClass(); // Returns null user
        }

        public async Task<List<ChatClass>> RequestChats()
        {
            string sqlExpression = "select * from chats";
            SqlCommand cmd = new SqlCommand(sqlExpression, dataBaseConnection);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            List<ChatClass> chatList = new List<ChatClass>();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    ChatClass chatClass = new ChatClass
                    (
                        Convert.ToString(reader.GetValue(1)),
                        Convert.ToInt32(reader.GetValue(0))
                    );
                    chatList.Add(chatClass);
                }
            }
            return chatList;
        }

        public async Task UpdateUser()
        {

        }

        public async void CloseConnection()
        {
            await dataBaseConnection.CloseAsync();
        }

        private string AssemblyConnectionString()
        {
            string connectionString = "";

            return connectionString;
        }
    }
}
