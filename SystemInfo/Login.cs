using RestSharp.Serializers;

namespace SystemInfo
{
    [SerializeAs(Name ="login")]
    public class Login
    {
        private string eml;
        private string pass;

        public Login (string email, string password)
        {
            eml = email;
            pass = password;
        }

        [SerializeAs(Name = "email")]
        public string email
        {
            get
            {
                return eml;
            }
            set
            {
                this.eml = value;
            }
        }

        [SerializeAs(Name = "password")]
        public string password
        {
            get
            {
                return pass;
            }
            set
            {
                this.pass = value;
            }
        }
    }
}