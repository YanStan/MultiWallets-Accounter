using System.Linq;
using Telegram.Bot.Args;
using System.Collections.Generic;
using Telegram.Bot.Types;
using MoneyCounter.Models;

namespace MoneyCounter
{
    class UserRepository
    {
        readonly MoneyCounterContext db = new MoneyCounterContext();

        public void SetBotChatMessages(List<Message> messagesList)
        {
            foreach (Message message in messagesList)
            {
                UserMessage userMessage = new UserMessage();
                userMessage.UserId = (int)message.Chat.Id;
                userMessage.MessageId = message.MessageId;
                if (message.Text != null && message.Text.StartsWith("Бухгалтер-бот приветствует тебя,"))
                    userMessage.IsMainMenu = true;
                else
                    userMessage.IsMainMenu = false;
                db.UserMessages.Add(userMessage);
                db.SaveChanges();
            }
        }

        public List<string> GetAdminUsernames() => db.Users.Where(x => x.IsAdmin == true).Select(x => x.UserName).ToList();
        public List<string> GetNonAdminUsernames() => db.Users.Where(x => x.IsAdmin == false).Select(x => x.UserName).ToList();

        public int? GetIdFromUserName(string username) => db.Users
            .FirstOrDefault(x => x.UserName == username && x.IsAdmin == true).UserId;
        public List<string> GetUsernamesFromWhiteList() => db.WhiteList.Select(x => x.UserName).ToList();


        public void DeleteExtraBotMessagesInDb(int userId)
        {
            var listOfUserMessages = GetBotMessagesWithUser(userId);
            int count = listOfUserMessages.Count;
            if (count > 3)
            {
                int headerId = listOfUserMessages.IndexOf(listOfUserMessages.LastOrDefault(x => x.IsMainMenu == true));
                listOfUserMessages.RemoveAt(headerId);
                listOfUserMessages.RemoveAt(listOfUserMessages.Count - 1);
                listOfUserMessages.RemoveAt(listOfUserMessages.Count - 1);
                db.UserMessages.RemoveRange(listOfUserMessages);
                db.SaveChanges();
            }
        }

        public List<UserMessage> GetBotMessagesWithUser(int userId) =>
            db.UserMessages.Where(x => x.UserId == userId).ToList();//IsUserExistsByName

        public bool IsUserExistsByName(string username) => GetUserFromUsername(username) != null;

        public bool IsUserAdminByName(string username)
        {
            var user = GetUserFromUsername(username);
            return user != null && user.IsAdmin;
        }

        public Models.User GetUserFromUsername(string username) => db.Users.FirstOrDefault(x => x.UserName == username);

        public Models.User GetUserFromUserId(int userId) => db.Users.FirstOrDefault(x => x.UserId == userId);
        public void UpgradeUserToAdmin(string username)
        {
            GetUserFromUsername(username).IsAdmin = true;
            db.SaveChanges();
        }
        public void DowngradeAdminToUser(string username)
        {
            GetUserFromUsername(username).IsAdmin = false;
            db.SaveChanges();
        }

        public void SetUserNameToWhiteList(string username)
        {
            WhiteUser whiteUser = new WhiteUser();
            whiteUser.UserName = username;
            db.WhiteList.Add(whiteUser);
            db.SaveChanges();
        }
        public bool IsUserInWhiteList(string username) => db.WhiteList.FirstOrDefault(x => x.UserName == username) != null;

        public void InitializeWhiteListIfEmpty(string username, string username2)
        {
            if(!db.WhiteList.Any())
            {
                WhiteUser whiteUser = new WhiteUser();
                WhiteUser whiteUser2 = new WhiteUser();
                whiteUser.UserName = username;
                whiteUser2.UserName = username2;
                db.WhiteList.AddRange(whiteUser, whiteUser2);
                db.SaveChanges();
            }
        }

        public void DeleteUserNameFromWhiteList(string username)
        {
            var whiteUser = db.WhiteList.FirstOrDefault(x => x.UserName == username);
            db.WhiteList.Remove(whiteUser);
            db.SaveChanges();
        }

        public void DeleteUser(string username)
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == username);
            db.Users.Remove(user);
            db.SaveChanges();
        }

        public void SetUserIfNotExist(MessageEventArgs e)
        {
            int userId = e.Message.From.Id;
            if (GetUserFromUserId(userId) == null)
            {
                Models.User user = new Models.User
                {
                    UserId = userId,
                    UserName = ("@" + e.Message.Chat.Username),
                    ChatStatus = "JUST_STARTED"
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public void SetAdminIfRootUser(int userId)
        {
            if(userId == 430611757 || userId == 359043468) //@Yan_stan and vladkheylo
            {
                db.Users.FirstOrDefault(x => x.UserId == userId).IsAdmin = true;
                db.SaveChanges();
            }

        }
        public bool IsUserAdmin(int userId) => db.Users.FirstOrDefault(x => x.UserId == userId).IsAdmin;

        public void SetUserChatStatus(int userId, string chatStatus)
        {
            Models.User user = db.Users.FirstOrDefault(x => x.UserId == userId);
            if(user != null)
                user.ChatStatus = chatStatus;
            db.SaveChanges();
        }

        public string GetUserChatStatus(int userId) => db.Users.FirstOrDefault(x => x.UserId == userId).ChatStatus;

        public void SetBotMessageId(int userId, int messageId)
        {
            var userMessage = new UserMessage();
            userMessage.UserId = userId;
            userMessage.MessageId = messageId;
            db.UserMessages.Add(userMessage);
            db.SaveChanges();
        }
    }
}
