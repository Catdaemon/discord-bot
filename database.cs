using System.Data.SQLite;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.IO;

namespace discord_bot
{
    public static class Database
    {
        public class ChatMessage
        {
            [Key]
            public int MessageID {get; set;}
            public string ChannelID {get; set;}
            public string User {get; set;}
            public string Message {get; set;}
            public DateTime Timestamp {get; set;}
        }
        public class Channel
        {
            public string ChannelID {get; set;}
            public string Name {get; set;}
        }
        
        public static SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection("Data Source=db/bot.db");
            connection.Open();
            return connection;
        }

        public static void InitDB()
        {
            if (File.Exists("db/bot.db"))
                return;

            using (var connection = GetConnection())
            {
                connection.Execute(@"
                    CREATE TABLE ChatMessages (
                        MessageID INTEGER PRIMARY KEY AUTOINCREMENT,
                        ChannelID varchar(100),
                        User varchar(100),
                        Message varchar(512),
                        Timestamp datetime
                    )
                ");
                connection.Execute(@"
                    CREATE TABLE Channels (
                        ChannelID varchar(100) UNIQUE,
                        Name varchar(512)
                    )
                ");
            }
        }

        public static async Task<int> LogAsync(string channel, string user, string message)
        {
            using (var connection = GetConnection())
            {
                return await connection.InsertAsync<ChatMessage>(new ChatMessage() {
                    User = user,
                    ChannelID = channel,
                    Message = message,
                    Timestamp = DateTime.Now
                });
            }
        }

        public static async Task<int> LogChannelAsync(string channel, string name)
        {
            using (var connection = GetConnection())
            {
                if (await connection.QuerySingleOrDefaultAsync<Channel>("SELECT * FROM Channels WHERE ChannelID = @ChannelID", new { ChannelID = channel }) == null) {
                    return await connection.InsertAsync<Channel>(new Channel() {
                        ChannelID = channel,
                        Name = name
                    });
                }

                return 0;
            }
        }
    }
}