using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentAppInfo
{
    public partial class Form2 : Form
    {
        private Prompt _prompt = new Prompt();
        private IMongoCollection<BsonDocument> collection, collection2;
        BsonDocument newData = null;
        BsonDocument newData2 = null;
        public Form2()
        {
            InitializeComponent();
            var dbClient = new MongoClient("mongodb://localhost:27017/");
            IMongoDatabase db = dbClient.GetDatabase("department");
            collection = db.GetCollection<BsonDocument>("users");
            collection2 = db.GetCollection<BsonDocument>("adduser");
            textBox2.PasswordChar = '*';
        }
        private void register()
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string email = textBox3.Text.Trim();
            string firstname = textBox4.Text.Trim();
            string middlename = textBox5.Text.Trim();
            string lastname = textBox6.Text.Trim();

            //radiobutton for role
            string role = radioButton1.Checked ? "student" : "admin";

      

            //if role is student
            if (role == "student")
            {
                newData = new BsonDocument
            {
                { "username", username },
                { "password", password },
                { "email", email },
                { "firstname", firstname },
                { "middlename", middlename },
                { "lastname", lastname },
                { "status", "Active" },
                { "verify_status", "Not Verified" },
                { "registration_status" ,"Not Registered"},
                { "enrollment_status", "Not Enrolled" },
                { "role", role },
                { "datecreated", DateTime.Now }
            };

                newData2 = new BsonDocument
            {
                { "username", username },
                { "password", password },
                { "email", email },
                { "firstname", firstname },
                { "middlename", middlename },
                { "lastname", lastname },
                { "degree","" },
                { "program","" },
                { "major","" },
                { "year_level","" },
                { "student_status", "" },
                { "registration_status" ,"Not Registered"},
                { "enrollment_status", "Not Enrolled" },
                { "role", role },
                { "datecreated", DateTime.Now }
            };
            }
            else
            {
                newData = new BsonDocument
            {
                { "username", username },
                { "password", password },
                { "email", email },
                { "firstname", firstname },
                { "middlename", middlename },
                { "lastname", lastname },
                { "status", "Active" },
                { "verify_status", "Not Verified" },
                { "role", role },
                { "datecreated", DateTime.Now }
            };
            }

            var filter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq("username", username),
                Builders<BsonDocument>.Filter.Eq("email", email)
            );

            var existingDocument = collection.Find(filter).FirstOrDefault();

            if (existingDocument != null)
            {
                if (existingDocument["username"].AsString == username)
                {
                    MessageBox.Show("Username already taken");

                }
                else if (existingDocument["email"].AsString == email)
                {
                    MessageBox.Show("Email already taken");
                }


            }
            else
            {
                if (role == "student")
                {
                    collection.InsertOne(newData);
                    collection2.InsertOne(newData2);

                }
                else
                {
                    collection.InsertOne(newData);
                }
                MessageBox.Show("Account created successfully");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to exit?", "Exit");
            if (userSelectedYes)
            {
                // Do something if the user selected yes
                System.Environment.Exit(1);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            register();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            newData = null;
            newData2 = null;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 fr1 = new Form1();
            fr1.Show();
            this.Hide();
        }
    }
}
