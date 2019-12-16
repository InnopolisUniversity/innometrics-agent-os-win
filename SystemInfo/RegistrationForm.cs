using RestSharp;
using System;
using System.Windows.Forms;

namespace SystemInfo
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
        }

        /**
         * Register method to handle the Register Button
         * @param object sender
         * @param RoutedEventArgs e
         */
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtName.Text;
            string password = txtPassword.Text;
            string email = txtEmail.Text;
            string lastname = txtSurname.Text;

            var client = new RestClient("https://innometric.guru:8120");
            var login = new RestRequest("https://innometric.guru:8120/V1/Admin/User", Method.POST);
            login.RequestFormat = DataFormat.Json;
            login.AddHeader("content-type", "application/json");
            //login.AddHeader("Authorization", "Basic");
            login.AddBody(new { email = email, name = username, surname = lastname, password = password });
            var response = client.Execute(login);
           
            if (response.StatusCode.ToString().Equals("OK"))
            {
                string message = response.StatusCode.ToString() + "\n" + "Registration successful";
                MessageBox.Show(message);
            } else
            {
                MessageBox.Show(response.StatusCode.ToString());
            }

            this.Close();
        }
    }
}
