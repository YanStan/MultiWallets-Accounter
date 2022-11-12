
namespace MoneyCounter.OrdinaryInputedTextCatchers
{
    class Add2WalletsToTransactionExecutor
    {
        public void AddFirstWalletToUserStatusData(string[] userStatusArray, string firstWallet, UserRepository userRepos, int userId)
        {
            string upperEntity = userStatusArray[2];
            string categoryName = userStatusArray[4];
            string subcategoryName = userStatusArray[6];
            userRepos.SetUserChatStatus(userId, $"WAIT/ADDWALLETSTOENTITY/{upperEntity}/CATEGORY/{categoryName}/SUBCATEGORY/{subcategoryName}/{firstWallet}>");
        }
        public void AddSecondWalletToUserStatusData(string[] userStatusArray, string secondWallet, UserRepository userRepos, int userId)
        {
            string upperEntity = userStatusArray[2];
            string categoryName = userStatusArray[4];
            string subcategoryName = userStatusArray[6];
            string firstWallet = userStatusArray[7].Split(">")[0];
            userRepos.SetUserChatStatus(userId, $"WAIT/ADDMONEYTOENTITY/{upperEntity}/CATEGORY/{categoryName}/SUBCATEGORY/{subcategoryName}/{firstWallet}>{secondWallet}");
        }
    }
}
