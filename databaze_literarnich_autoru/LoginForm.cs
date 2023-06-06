using System.Security.Cryptography;

namespace FinalProject
{
    public partial class LoginForm : Form
    {
        Dictionary<string, UserLoginInfo> users = new Dictionary<string, UserLoginInfo>
        {
            {"user", new UserLoginInfo("gLhePJ7nRHklkPk7WfBRIZSwxTHfkxwNHNV8Zkr3npptLvLq9jMVKx4eYIOpQp4b", new UserRoles.Normal()) },
            {"admin", new UserLoginInfo("ME2/pnvopTtVaFW0UuiflsEUJg4oSgu8aXpYfRnBQNqTZ6/7L4bSDHVPyfUj0VfY", new UserRoles.Editor()) },
        };
        public UserRole? SelectedUserRole { get; private set; } = null;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private byte[] CreatePasswordHash(string password)
        {
            byte[] saltValue = RandomNumberGenerator.GetBytes(16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltValue, 120000, HashAlgorithmName.SHA512);
            byte[] outHash = pbkdf2.GetBytes(32);
            return outHash.Concat(saltValue).ToArray();
        }

        private byte[] CreatePasswordHash(string password, byte[] saltValue)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltValue, 120000, HashAlgorithmName.SHA512);
            byte[] outHash = pbkdf2.GetBytes(32);
            return outHash.Concat(saltValue).ToArray();
        }

        private bool VerifyPassword(string password, byte[] savedPasswordHash)
        {
            var salt = savedPasswordHash.TakeLast(16).ToArray();
            var generatedHash = CreatePasswordHash(password, salt);
            return generatedHash.SequenceEqual(savedPasswordHash);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text))
            {
                return;
            }
            if(!users.ContainsKey(textBox1.Text) || !VerifyPassword(textBox2.Text, Convert.FromBase64String(users[textBox1.Text].PasswordHash)))
            {
                MessageBox.Show("Neplatny login.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Uspesne prihlaseno.", "Uspech", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            SelectedUserRole = users[textBox1.Text].Role;
            this.Close();
        }
    }
}