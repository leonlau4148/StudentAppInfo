using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Windows.Forms;

namespace StudentAppInfo
{
    public partial class Admin3 : UserControl
    {
        private IMongoCollection<BsonDocument> collection;

        public Admin3()
        {
            InitializeComponent();
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("department");
            collection = database.GetCollection<BsonDocument>("users");
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
        }

        public class UserInfo
        {
            public string FullName { get; set; }
            public string Role { get; set; }
            public string Validate { get; set; }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // If checkBox1 is checked, uncheck checkBox2
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
            comboBox1.SelectedIndex = -1;
            //clear the combo box text
            comboBox1.Text = "";
            button1.Text = "Unverify";
            // Populate the combo box
            PopulateComboBox();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // If checkBox2 is checked, uncheck checkBox1
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
            comboBox1.SelectedIndex = -1;
            //clear the combo box text
            comboBox1.Text = "";
            button1.Text = "Verify";
            // Populate the combo box
            PopulateComboBox();
        }
      private void PopulateComboBox()
{
        // Define the aggregation pipeline stages
        var pipeline = new BsonDocument[0];

        // Check which checkbox is checked and adjust the pipeline accordingly
        if (checkBox1.Checked)
        {
            pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("verify_status", "Verified")),
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 0 },
                    { "fullName", new BsonDocument("$concat", new BsonArray
                    {
                        "$firstname",
                        new BsonDocument("$cond", new BsonArray
                        {
                            new BsonDocument("$ne", new BsonArray { "$middlename", "" }),
                            new BsonDocument("$concat", new BsonArray { " ", "$middlename" }),
                            ""
                        }),
                        " ",
                        "$lastname"
                    }) },
                    { "role", 1 },
                    { "verify_status", 1 }
                })
            };
        }
        else if (checkBox2.Checked)
        {
            pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("verify_status", "Not Verified")),
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 0 },
                    { "fullName", new BsonDocument("$concat", new BsonArray
                    {
                        "$firstname",
                        new BsonDocument("$cond", new BsonArray
                        {
                            new BsonDocument("$ne", new BsonArray { "$middlename", "" }),
                            new BsonDocument("$concat", new BsonArray { " ", "$middlename" }),
                            ""
                        }),
                        " ",
                        "$lastname"
                    }) },
                    { "role", 1 },
                    { "verify_status", 1 }
                })
            };
        }

        // Execute the aggregation and get the distinct values
        var distinctValues = collection.Aggregate<BsonDocument>(pipeline)
            .ToList()
            .Select(doc =>
            {
                if (doc.TryGetValue("fullName", out var fullName) &&
                    doc.TryGetValue("role", out var role) &&
                    doc.TryGetValue("verify_status", out var verifyStatus))
                {
                    return new UserInfo
                    {
                        FullName = fullName.AsString,
                        Role = role.AsString,
                        Validate = verifyStatus.AsString
                    };
                }
                return null; // or return a default UserInfo object with appropriate defaults
            })
            .Where(userInfo => userInfo != null) // Filter out null values
            .ToList();

            // Clear the ComboBox to start fresh
            comboBox1.Items.Clear();

            // Add the distinct values to the ComboBox
            foreach (var value in distinctValues)
            {
                comboBox1.Items.Add(value);
            }

            // Set the combo box to display the full name
            comboBox1.DisplayMember = "FullName";
        }
        private void Admin3_Load(object sender, EventArgs e)
        {
            PopulateComboBox();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = comboBox1.SelectedItem as UserInfo;
            if (selectedItem != null)
            {
                // Update the labels with the role and verification status
                label1.Text = selectedItem.Role;
            
            }
            else
            {
                // Clear the labels if no item is selected
                label1.Text = "";
              
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Verify or unverify the selected user
            var selectedItem = comboBox1.SelectedItem as UserInfo;
            if (selectedItem != null)
            {
                // Split the full name into first, middle, and last name parts
                string[] nameParts = selectedItem.FullName.Split(new[] { ' ' }, 3);
                if (nameParts.Length >= 2)
                {
                    string firstName = nameParts[0];
                    string lastName = nameParts[nameParts.Length - 1];
                    string middleName = nameParts.Length > 2 ? nameParts[1] : "";

                    // Create a filter to find the user by first, middle, and last name
                    var filter = Builders<BsonDocument>.Filter.Eq("firstname", firstName) &
                                 Builders<BsonDocument>.Filter.Eq("lastname", lastName);

                    // If there's a middle name, add it to the filter
                    if (!string.IsNullOrEmpty(middleName))
                    {
                        filter &= Builders<BsonDocument>.Filter.Eq("middlename", middleName);
                    }

                    // Find the current verify_status of the user
                    var currentUser = collection.Find(filter).FirstOrDefault();
                    if (currentUser != null && currentUser.TryGetValue("verify_status", out var verifyStatus))
                    {
                        string currentStatus = verifyStatus.AsString;
                        string newStatus = currentStatus == "Verified" ? "Not Verified" : "Verified";

                        // Create an update to set the verify_status based on the current status
                        var update = Builders<BsonDocument>.Update.Set("verify_status", newStatus);

                        // Update the user's verify_status in the collection
                        collection.UpdateOne(filter, update);

                        // Refresh the combo box to reflect the updated status
                        PopulateComboBox();

                        // Clear the selection after updating
                        comboBox1.SelectedIndex = -1;
                        comboBox1.Text = "";
                        // Prompt the user that the user has been verified or unverified
                        MessageBox.Show(newStatus == "Verified" ? "User has been verified." : "User has been unverified.");
                    }
                    else
                    {
                        // Handle the case where the user is not found or does not have a verify_status field
                        MessageBox.Show("User not found or does not have a verification status.");
                    }
                }
                else
                {
                    // Handle the case where the full name does not contain at least two parts
                    MessageBox.Show("The selected user's full name is not properly formatted.");
                }
            }
            else
            {

                MessageBox.Show("User Not Found. Please select a user to verify or unverify.");
            }
        }
    }
}