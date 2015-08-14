
namespace Build.Extensions.Security
{
    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return null;
            }
            else
            {
                return UserName;
            }
        }
    }
}
