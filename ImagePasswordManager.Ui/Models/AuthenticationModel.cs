namespace SimplePasswordManager.Models
{
    public class AuthenticationModel
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Password}";
        }
    }
}