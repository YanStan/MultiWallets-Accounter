using Telegram.Bot.Args;

namespace MoneyCounter
{
    public class UserData
    {
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string UserText { get; private set; }
        public string UserStatus { get; private set; }
        public string[] UserStatusArray { get; private set; }
        public int UserStatusArrLen { get; private set; }
        public void Set(MessageEventArgs e)
        {
            UserId = e.Message.From.Id;
            Username = e.Message.From.Username;
            UserText = e.Message.Text;
            FirstName = e.Message.From.FirstName;
        }
        public void SetWithStatus(MessageEventArgs e)
        {
            Set(e);
            UserRepository repos = new UserRepository();
            UserStatus = repos.GetUserChatStatus(UserId);
        }
        public void SetWithStatusArray(MessageEventArgs e)
        {
            SetWithStatus(e);
            UserStatusArray = UserStatus.Split("/", System.StringSplitOptions.RemoveEmptyEntries);
            UserStatusArrLen = UserStatusArray.Length;
        }

        public void SetNewStatusData(string userChatStatus)
        {
            UserStatus = userChatStatus;
            UserStatusArray = userChatStatus.Split("/", System.StringSplitOptions.RemoveEmptyEntries);
            UserStatusArrLen = UserStatusArray.Length;
        }
    }
}
