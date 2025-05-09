using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentAppInfo
{
    public partial class User2 : UserControl
    {
        private string userEmail;
        private Prompt _prompt = new Prompt();
        private IMongoCollection<BsonDocument> collection, collection2, collection3;

        public User2(string emaildb)
        {
            InitializeComponent();
            // Connect to MongoDB
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("department");
            collection = database.GetCollection<BsonDocument>("schoolprograms");
            collection2 = database.GetCollection<BsonDocument>("adduser");
            collection3 = database.GetCollection<BsonDocument>("users");
            userEmail = emaildb;
          //  label1.Text = "Welcome, " + userEmail;
        }

        private void PopulateComboBox(IMongoCollection<BsonDocument> collection, string fieldName, ComboBox comboBox)
        {
            // Clear the ComboBox to start fresh
            comboBox.Items.Clear();

            // Define the aggregation pipeline stages
            var pipeline = new BsonDocument[]
            {
            new BsonDocument("$unwind", "$" + fieldName), // Unwind the array
            new BsonDocument("$group", new BsonDocument // Group by the field to get distinct values
            {
                { "_id", "$" + fieldName }
            }),
            new BsonDocument("$sort", new BsonDocument("_id", 1)) // Sort the values
            };

            // Execute the aggregation and get the distinct values
            var distinctValues = collection.Aggregate<BsonDocument>(pipeline)
                .ToList() // Execute the aggregation
                .Select(doc => doc["_id"].AsString) // Extract the distinct values
                .ToList();

            // Add the distinct values to the ComboBox
            foreach (var value in distinctValues)
            {
                comboBox.Items.Add(value);
            }
        }

        private void User2_Load(object sender, EventArgs e)
        {
            // Populate the degree ComboBox
            PopulateComboBox(collection, "degree", comboBox2);

            // Populate the program ComboBox
            PopulateComboBox(collection, "program", comboBox3);

            // Populate the major ComboBox
            PopulateComboBox(collection, "major", comboBox4);

            var emailToFind = userEmail; // Replace with the email you're looking for
            var filter = Builders<BsonDocument>.Filter.Eq("email", emailToFind);

            // Find the document
            var document = collection2.Find(filter).FirstOrDefault();

            if (document != null)
            {
                var retrievedEmail = document["email"].AsString;
                var retrievedYearLevel = document["year_level"].AsString;
                var retrievedProgram = document["program"].AsString;
                var retrievedMajor = document["major"].AsString;
                var retrievedStudentStatus = document["student_status"].AsString;
                var retrievedDegree = document["degree"].AsString;
                var retrievedAge = document["age"].AsString;
                var retrievedContact = document["contact"].AsString;
                var retrievedAddress = document["address"].AsString;
                var retrievedBirthdate = document["birthdate"].AsString;

                comboBox1.Text = retrievedYearLevel;
                comboBox2.Text = retrievedDegree;
                comboBox3.Text = retrievedProgram;
                textBox8.Text = retrievedAge;
                textBox9.Text = retrievedContact;
                textBox7.Text = retrievedAddress;
                dateTimePicker1.Text = retrievedBirthdate;


                if (retrievedMajor == "")
                {
                    comboBox4.Enabled = false;
                }
                else
                {
                    comboBox4.Text = retrievedMajor;
                }


                if (retrievedStudentStatus == "New")
                {
                    radioButton1.Checked = true;
                }
                else if (retrievedStudentStatus == "Old")
                {
                    radioButton2.Checked = true;
                }
               


            }


            // Check if the username and password exist in the database
            var result = collection3.Find(filter).FirstOrDefault();
            if (result != null)
            {
                string GetUserEmail = result.GetValue("email").AsString;
                if (GetUserEmail == userEmail)
                {
                    string GetUsername = result.GetValue("username").AsString;
                    string GetPassword = result.GetValue("password").AsString;
                    string GetFirstname = result.GetValue("firstname").AsString;
                    string GetMiddlename = result.GetValue("middlename").AsString;
                    string GetLastname = result.GetValue("lastname").AsString;
                    textBox1.Text = GetUsername;
                    textBox2.Text = GetPassword;
                    textBox3.Text = GetUserEmail;
                    textBox4.Text = GetFirstname;
                    textBox5.Text = GetMiddlename;
                    textBox6.Text = GetLastname;
                    
                }
            
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Text = "";
            comboBox4.Text = "";
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

           
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear the major ComboBox
            comboBox4.Items.Clear();
            comboBox4.Text = "";


            // Get the selected program
            string selectedProgram = comboBox3.Text;

            if (!string.IsNullOrEmpty(selectedProgram))
            {
                // Filter the collection by the selected program
                var filter = Builders<BsonDocument>.Filter.Eq("program", selectedProgram);
                var programDocument = collection.Find(filter).FirstOrDefault();

                if (programDocument != null)
                {

                    // Populate the major ComboBox with majors for the selected program
                    var majors = programDocument["major"].AsBsonArray.Select(v => v.AsString).ToList();
                    foreach (var major in majors)
                    {
                        if (major == "")
                        {
                            comboBox4.Enabled = false;
                        }
                        else
                        {
                            comboBox4.Enabled = true;
                            comboBox4.Items.Add(major);
                        }

                    }
                }
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to save the changes?", "Save Profile");
            if (userSelectedYes)
            {
                // Get the selected values
                string selectedYearLevel = comboBox1.Text;
                string selectedDegree = comboBox2.Text;
                string selectedProgram = comboBox3.Text;
                string selectedMajor = comboBox4.Text;
                string username = textBox1.Text;
                string password = textBox2.Text;
                string email = textBox3.Text;
                string firstname = textBox4.Text;
                string middlename = textBox5.Text;
                string lastname = textBox6.Text;
                string address = textBox7.Text;
                string birthdate = dateTimePicker1.Text;
                string age = textBox8.Text;
                string contact = textBox9.Text;


                // if userEmail is equal to mongodb email then already exist
                var filter = Builders<BsonDocument>.Filter.Eq("email", userEmail);
                var existingDocument = collection2.Find(filter).FirstOrDefault();



                if (existingDocument != null)
                {
                    // Update the existing document
                    var update = Builders<BsonDocument>.Update
                        .Set("email", userEmail)
                        .Set("firstname", firstname)
                        .Set("middlename", middlename)
                        .Set("lastname", lastname)
                        .Set("address", address)
                        .Set("birthdate", birthdate)
                        .Set("age", age)
                        .Set("contact", contact)
                        .Set("student_status", radioButton1.Checked ? radioButton1.Text : radioButton2.Text)
                        .Set("year_level", selectedYearLevel)
                        .Set("degree", selectedDegree)
                        .Set("program", selectedProgram)
                        .Set("major", selectedMajor)
                        .Set("datecreated", DateTime.Now);
                    var update2 = Builders<BsonDocument>.Update
                        .Set("username", username)
                        .Set("password", password)
                        .Set("email", email)
                        .Set("firstname", firstname)
                        .Set("middlename", middlename)
                        .Set("lastname", lastname);
                       
                    collection2.UpdateOne(filter, update);
                    collection3.UpdateOne(filter, update2);
                    MessageBox.Show("Data updated in MongoDB.");
                    return;
                }
                else
                {
                    if (string.IsNullOrEmpty(selectedYearLevel) || string.IsNullOrEmpty(selectedDegree) || string.IsNullOrEmpty(selectedProgram))
                    {
                        MessageBox.Show("Please fill up all fields.");
                        return;
                    }
                    else
                    {

                        var document = new BsonDocument
                                {
                                    { "email",userEmail },
                                    { "firstname",firstname },
                                    { "middlename",middlename },
                                    { "lastname",lastname },
                                    { "address",address },
                                    { "birthdate",birthdate },
                                    { "age",age },
                                    { "contact",contact},
                                    { "year_level", selectedYearLevel },
                                    { "degree",selectedDegree },
                                    { "program", selectedProgram },
                                    { "major", selectedMajor },
                                    { "student_status", radioButton1.Checked ? radioButton1.Text : radioButton2.Text},
                                    { "datecreated", DateTime.Now }

                                };
                        collection2.InsertOne(document);
                   

                        MessageBox.Show("Saved to MongoDB.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Data not saved.");
            }
        }
    }
}
