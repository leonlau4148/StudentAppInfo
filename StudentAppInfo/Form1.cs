using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace StudentAppInfo
{
    public partial class Form1 : Form
    {
        private Prompt _prompt = new Prompt();
        private IMongoCollection<BsonDocument> collection;
        public Form1()
        {
            InitializeComponent();
            var dbClient = new MongoClient("mongodb://localhost:27017/");
            IMongoDatabase db = dbClient.GetDatabase("department");
            collection = db.GetCollection<BsonDocument>("users");
            textBox2.PasswordChar = '*';
        }

        private void login()
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            // Create a filter to check if the username and password exist in the database
            var filter = Builders<BsonDocument>.Filter.And(
                               Builders<BsonDocument>.Filter.Eq("username", username),
                               Builders<BsonDocument>.Filter.Eq("password", password)
                                                         );

            // Check if the username and password exist in the database
            var result = collection.Find(filter).FirstOrDefault();
            if (result != null)
            {
                string userEmail = result.GetValue("email").AsString;
                if (result.GetValue("role").AsString == "student")
                {
                    // check if the user is verified
                    if (result.GetValue("verify_status").AsString == "Verified")
                    {
                        Form3 fr3 = new Form3(userEmail);
                        fr3.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Your account is not verified yet. Email at registrar.ravenstateuni@edu.ph for verification.");
                    }
                }
                else
                {
                    if (result.GetValue("verify_status").AsString == "Verified")
                    {
                        Form4 fr4 = new Form4();
                        fr4.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Your account is not verified yet. Please contact the administrator.");
                    }

                }
            }
            else
            {
                MessageBox.Show("Invalid Username or Password");
            }
        }
        
        private void Button1_Click(object sender, EventArgs e)
        {
            login();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form2 fr2 = new Form2();
            fr2.Show();
            this.Hide();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.PasswordChar = '\0';
            }
            else
            {
                textBox2.PasswordChar = '*';
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to exit?", "Exit");
            if (userSelectedYes)
            {
                // Do something if the user selected yes
                System.Environment.Exit(1);
            }
        }
    }
}
