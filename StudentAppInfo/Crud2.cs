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
    public partial class Crud2 : UserControl
    {
        private Prompt _prompt = new Prompt();
        private IMongoCollection<BsonDocument> collection, collection2,collection3;
        private DataTable dataTable = null;
      
        public Crud2()
        {
            InitializeComponent();
            // Connect to MongoDB
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("department");
            collection = database.GetCollection<BsonDocument>("schoolprograms");
            collection2 = database.GetCollection<BsonDocument>("adduser");
            collection3 = database.GetCollection<BsonDocument>("users");
            LoadData();
        }

        private void LoadData()
        {
            // Query the MongoDB collections
            var documents = collection2.Find(new BsonDocument()).ToList();
            var documents3 = collection3.Find(new BsonDocument()).ToList();

            // Convert the documents to a DataTable
            dataTable = new DataTable();

            // Define the expected columns for the DataTable
            dataTable.Columns.Add("_id");
            dataTable.Columns.Add("Firstname");
            dataTable.Columns.Add("Middlename");
            dataTable.Columns.Add("Lastname");
            dataTable.Columns.Add("Address");
            dataTable.Columns.Add("Birthdate");
            dataTable.Columns.Add("Age");
            dataTable.Columns.Add("Email");
            dataTable.Columns.Add("Contact");
            dataTable.Columns.Add("Year_Level");
            dataTable.Columns.Add("Degree");
            dataTable.Columns.Add("Program");
            dataTable.Columns.Add("Major");
            dataTable.Columns.Add("Enrollment_Status");
            dataTable.Columns.Add("Registration_Status");
            dataTable.Columns.Add("Username");
            dataTable.Columns.Add("Password");
            dataTable.Columns.Add("student_status");

            foreach (var document in documents)
            {
                var row = dataTable.NewRow();
                foreach (var key in document.Elements)
                {
                    // Check if the column exists in the DataTable before setting the value
                    if (dataTable.Columns.Contains(key.Name))
                    {
                        row[key.Name] = key.Value.ToString();
                    }
                }

                // Find the corresponding document in collection3 and add the username and password
                var document3 = documents3.Find(d => d["email"] == document["email"]);
                if (document3 != null)
                {
                    row["Username"] = document3["username"].AsString;
                    row["Password"] = document3["password"].AsString;
                }

                dataTable.Rows.Add(row);
            }

            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = dataTable;
            dataGridView1.Columns["_id"].Visible = false;
            dataGridView1.Columns["password"].Visible = false; // Hide the password column
            

            // Change the column names if needed
            dataTable.Columns["Registration_Status"].ColumnName = "Registration Status";
            dataTable.Columns["Enrollment_Status"].ColumnName = "Enrollment Status";
            dataTable.Columns["Year_Level"].ColumnName = "Year Level";
            dataTable.Columns["student_status"].ColumnName = "Student Status";
           
       
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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

        public void cleardata()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox7.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox9.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            if (dataGridView1.Rows.Count > 0)
            {
                // Select the first row
                dataGridView1.Rows[0].Selected = true;
                // Scroll to the first row if it's not visible
                dataGridView1.FirstDisplayedScrollingRowIndex = 0;
            }
        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Text = "";
            comboBox4.Text = "";
        }

        private void Crud2_Load(object sender, EventArgs e)
        {
            // Populate the degree ComboBox
            PopulateComboBox(collection, "degree", comboBox2);

            // Populate the program ComboBox
            PopulateComboBox(collection, "program", comboBox3);

            // Populate the major ComboBox
            PopulateComboBox(collection, "major", comboBox4);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cleardata();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to save the changes?", "Save Profile");
            if (userSelectedYes)
            {

                // Check if any of the textboxes are empty
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) ||
                   string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox6.Text) ||
                    string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox9.Text) ||
                    string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(comboBox3.Text) ||
                  !radioButton1.Checked && !radioButton2.Checked)
                {
                    MessageBox.Show("Please enter all fields.");
                    return;
                }
                // Get the selected values
                string birthdate = dateTimePicker1.Text;
                string selectedYearLevel = comboBox1.Text;
                string selectedDegree = comboBox2.Text;
                string selectedProgram = comboBox3.Text;
                string selectedMajor = comboBox4.Text;
                string username = textBox1.Text;
                string password = textBox2.Text;
                string firstname = textBox3.Text;
                string middlename = textBox4.Text;
                string lastname = textBox5.Text;
                string age = textBox6.Text;
                string email = textBox7.Text;
                string contact = textBox8.Text;
                string address = textBox9.Text;
                var document = new BsonDocument
                    {
                             { "email",email },
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
                             { "registration_status" ,"Registered"},
                             { "enrollment_status", "Enrolled" },
                             { "role", "student" },  
                             { "datecreated", DateTime.Now }

                    };
                    

                var document2 = new BsonDocument
                {
                        { "username", username },
                        { "password", password },
                        { "email", email },
                        { "firstname", firstname },
                        { "middlename", middlename },
                        { "lastname", lastname },
                        { "status", "Active" },
                        { "verify_status", "Verified" },
                        { "registration_status" ,"Registered"},
                        { "enrollment_status", "Enrolled" },
                        { "role", "student" },
                        { "datecreated", DateTime.Now }


                };
                var filter = Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("username", username),
                    Builders<BsonDocument>.Filter.Eq("email", email),
                    Builders<BsonDocument>.Filter.Eq("firstname", firstname),
                    Builders<BsonDocument>.Filter.Eq("middlename", middlename),
                    Builders<BsonDocument>.Filter.Eq("lastname", lastname)


                );

                var existingDocument = collection3.Find(filter).FirstOrDefault();

                if (existingDocument != null)
                {
                     if (existingDocument["firstname"].AsString == firstname && existingDocument["middlename"].AsString == middlename && existingDocument["lastname"].AsString == lastname)
                         {
                            if (existingDocument["verify_status"].AsString == "verified")
                            {
                                MessageBox.Show("User already exist.");
                            }
                            else
                            {
                                 //delete the existing document and save the new one
                                 collection3.DeleteOne(filter);
                                 collection2.InsertOne(document);
                                 collection3.InsertOne(document2);
                                 MessageBox.Show("Saved to MongoDB.");
                            }
                        }
                     else
                     {
                        if (existingDocument["verify_status"].AsString == "verified")
                        {
                            MessageBox.Show("User already exist.");
                        }
                        else
                        {
                            collection2.InsertOne(document);
                            collection3.InsertOne(document2);
                            MessageBox.Show("Saved to MongoDB.");
                        }
                       
                    }
                 
                }
                else
                {
                    collection2.InsertOne(document);
                    collection3.InsertOne(document2);
                    MessageBox.Show("Saved to MongoDB.");
                }
                // Reload the data after saving changes
                LoadData();
              }

               
            }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the row index is valid
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                // Select the entire row
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the row index is valid
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                // Select the entire row
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Check if the row index is valid
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                // Select the entire row
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
             
            // Check if the row index is valid
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                // Select the entire row
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Populate TextBox with data from the clicked row
                
                textBox1.Text = row.Cells["username"].Value.ToString();
                textBox2.Text = row.Cells["password"].Value.ToString();
                textBox3.Text = row.Cells["firstname"].Value.ToString();
                textBox4.Text = row.Cells["middlename"].Value.ToString();
                textBox5.Text = row.Cells["lastname"].Value.ToString();
                textBox6.Text = row.Cells["age"].Value.ToString();
                textBox7.Text = row.Cells["email"].Value.ToString();
                textBox8.Text = row.Cells["contact"].Value.ToString();
                textBox9.Text = row.Cells["address"].Value.ToString();
                comboBox1.Text = row.Cells["Year Level"].Value.ToString();
                comboBox2.Text = row.Cells["Degree"].Value.ToString();
                comboBox3.Text = row.Cells["Program"].Value.ToString();
                comboBox4.Text = row.Cells["Major"].Value.ToString();
                radioButton1.Checked = row.Cells["Student Status"].Value.ToString() == "New" ? true : false;
                radioButton2.Checked = row.Cells["Student Status"].Value.ToString() == "Old" ? true : false;
                dateTimePicker1.Text = row.Cells["birthdate"].Value.ToString();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to delete the changes?", "Delete Profile");
            if (userSelectedYes)
            {
                // Check if any row is selected
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    // Get the _id of the selected document
                    var documentId = selectedRow.Cells["_id"].Value.ToString(); // Assuming "_id" is the name of the column containing the document ID

                    // Define the filter to delete the document from MongoDB
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(documentId));

                    // Delete the document from MongoDB collection
                    collection2.DeleteOne(filter);

                    //remove collection 3 data 
                    var filter2 = Builders<BsonDocument>.Filter.Eq("username", selectedRow.Cells["username"].Value.ToString());
                    collection3.DeleteOne(filter2);
                  


                    // Remove the row from the DataTable
                    dataTable.Rows.RemoveAt(selectedRow.Index);
                    cleardata();
                    // Reload the data after deleting the document
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //update textbox data to the database
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to update the changes?", "Update Profile");

            if (userSelectedYes)
            {
                // Check if a row is selected
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                    // Get the selected values
                    string birthdate = dateTimePicker1.Text;
                    string selectedYearLevel = comboBox1.Text;
                    string selectedDegree = comboBox2.Text;
                    string selectedProgram = comboBox3.Text;
                    string selectedMajor = comboBox4.Text;
                    string username = textBox1.Text;
                    string password = textBox2.Text;
                    string firstname = textBox3.Text;
                    string middlename = textBox4.Text;
                    string lastname = textBox5.Text;
                    string age = textBox6.Text;
                    string email = textBox7.Text;
                    string contact = textBox8.Text;
                    string address = textBox9.Text;
                    var document = new BsonDocument
                    {
                        { "username", username },
                        { "password", password },
                        { "email", email },
                        { "firstname", firstname },
                        { "middlename", middlename },
                        { "lastname", lastname },
                        { "address", address },
                        { "birthdate", birthdate },
                        { "age", age },
                        { "contact", contact },
                        { "year_level", selectedYearLevel },
                        { "degree", selectedDegree },
                        { "program", selectedProgram },
                        { "major", selectedMajor },
                        { "student_status", radioButton1.Checked ? radioButton1.Text : radioButton2.Text },
                        {"role","student" },
                        { "registration_status", "Registered" },
                        { "enrollment_status", "Enrolled" },
                        { "datecreated", DateTime.Now }
                    };

                    var document2 = new BsonDocument
                    {
                        { "username", username },
                        { "password", password },
                        { "email",email },
                        { "firstname", firstname },
                        { "middlename", middlename },
                        { "lastname", lastname },
                        { "status", "Active" },
                        { "verify_status", "Verified" },
                        { "registration_status" ,"Registered"},
                        { "enrollment_status", "Enrolled" },
                        { "role", "student" },
                        { "datecreated", DateTime.Now }

                    };

                    // Get the _id of the selected document
                    string documentId = selectedRow.Cells["_id"].Value?.ToString();
                    if (string.IsNullOrEmpty(documentId))
                    {
                        MessageBox.Show("The selected row does not contain a valid '_id'.");
                        return;
                    }

                    // Check if the documentId is a valid ObjectId
                    if (ObjectId.TryParse(documentId, out ObjectId objectId))
                    {
                        // update the document in the MongoDB collection
                        var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                        collection2.UpdateOne(filter, new BsonDocument("$set", document));
                        collection3.UpdateOne(filter, new BsonDocument("$set", document2));
                        MessageBox.Show("Updated to MongoDB.");
                        LoadData();
                        cleardata();
                    }
                    else
                    {
                        MessageBox.Show("Invalid ObjectId format.");
                    }
                }
                else
                {
                    MessageBox.Show("No row is selected.");
                }
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Check if the row index is valid
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                // Select the entire row
                dataGridView1.Rows[e.RowIndex].Selected = true;
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

            //populate the textbox with the row
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                   
                    textBox1.Text = row.Cells["username"].Value.ToString();
                    textBox2.Text = row.Cells["password"].Value.ToString();
                    textBox3.Text = row.Cells["firstname"].Value.ToString();
                    textBox4.Text = row.Cells["middlename"].Value.ToString();
                    textBox5.Text = row.Cells["lastname"].Value.ToString();
                    textBox6.Text = row.Cells["age"].Value.ToString();
                    textBox7.Text = row.Cells["email"].Value.ToString();
                    textBox8.Text = row.Cells["contact"].Value.ToString();
                    textBox9.Text = row.Cells["address"].Value.ToString();
                    comboBox1.Text = row.Cells["Year Level"].Value.ToString();
                    comboBox2.Text = row.Cells["Degree"].Value.ToString();
                    comboBox3.Text = row.Cells["Program"].Value.ToString();
                    comboBox4.Text = row.Cells["Major"].Value.ToString();
                    radioButton1.Checked = row.Cells["Student Status"].Value.ToString() == "New" ? true : false;
                    radioButton2.Checked = row.Cells["Student Status"].Value.ToString() == "Old" ? true : false;
                    dateTimePicker1.Text = row.Cells["birthdate"].Value.ToString();



                }
            }
        }

    }
    }

