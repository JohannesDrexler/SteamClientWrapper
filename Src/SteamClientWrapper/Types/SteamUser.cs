using System.Diagnostics;

namespace SteamWrapper.Types
{
    [DebuggerDisplay("AccName={AcountName} NickName={PersonalName}]")]
    public class SteamUser
    {
        public long UserId { get; private set; }

        public string AcountName { get; private set; }

        public string PersonalName { get; private set; }

        public SteamUser(long id, string accName, string personalName)
        {
            UserId = id;
            AcountName = accName;
            PersonalName = personalName;
        }
    }
}
