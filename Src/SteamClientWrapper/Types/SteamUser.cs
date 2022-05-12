using System.Diagnostics;

namespace SteamClientWrapper.Types
{
    /// <summary>
    /// Describes user
    /// </summary>
    [DebuggerDisplay("AccName={AcountName} NickName={PersonalName}]")]
    public class SteamUser
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        public long UserId { get; private set; }

        /// <summary>
        /// Name of the account
        /// </summary>
        public string AcountName { get; private set; }

        /// <summary>
        /// NickName of the account
        /// </summary>
        public string PersonalName { get; private set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="accName">AccountName</param>
        /// <param name="personalName">NickName</param>
        public SteamUser(long id, string accName, string personalName)
        {
            UserId = id;
            AcountName = accName;
            PersonalName = personalName;
        }
    }
}
