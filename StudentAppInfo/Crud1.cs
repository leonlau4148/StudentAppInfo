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
    public partial class Crud1 : UserControl
    {
        private IMongoCollection<BsonDocument> collection, collection2;
        private DataTable dataTable = null;
        private Prompt _prompt = new Prompt();
        public Crud1()
        {
            InitializeComponent();
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("department");
            collection = database.GetCollection<BsonDocument>("schoolprograms");
            collection2 = database.GetCollection<BsonDocument>("adduser");

            // Load data into the DataGridView
            LoadData();
        }
        private void LoadData()
        {
            // Query the MongoDB collection
            var documents = collection2.Find(new BsonDocument()).ToList();

            // Convert the documents to a DataTable
            dataTable = new DataTable();

            // Define the expected columns for the DataTable
            //    dataTable.Columns.Add("_id"); // Assuming the _id field is used to identify the document
            dataTable.Columns.Add("_id");
            dataTable.Columns.Add("Firstname");
            dataTable.Columns.Add("Middlename");
            dataTable.Columns.Add("Lastname");
            dataTable.Columns.Add("Address");
            dataTable.Columns.Add("Birthdate");
            dataTable.Columns.Add("Age");
            dataTable.Columns.Add("Email"); // Add the email column
            dataTable.Columns.Add("Contact");
            dataTable.Columns.Add("Year_Level");
            dataTable.Columns.Add("Degree");
            dataTable.Columns.Add("Program");
            dataTable.Columns.Add("Major");
            dataTable.Columns.Add("Enrollment_Status");
            dataTable.Columns.Add("Registration_Status");


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
                dataTable.Rows.Add(row);
            }

            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = dataTable;
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Save the changes to MongoDB when a cell's value is edited
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var documentId = dataTable.Rows[e.RowIndex]["_id"].ToString(); // Assuming the _id field is used to identify the document
                var columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                var newValue = dataTable.Rows[e.RowIndex][columnName].ToString();

                // Create an update document with the new value
                var update = Builders<BsonDocument>.Update.Set(columnName, newValue);

                // Update the document in the MongoDB collection
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(documentId));
                collection.UpdateOne(filter, update);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to save the changes?", "Save Profile");
            if (userSelectedYes)
            {
                // Iterate through the rows in the DataTable
                foreach (DataRow row in dataTable.Rows)
                {
                    var documentId = row["_id"].ToString(); // Assuming the _id field is used to identify the document

                    // Create an update definition to update all fields
                    var updateDefinition = Builders<BsonDocument>.Update
                        .Set("firstname", row["Firstname"].ToString())
                        .Set("middlename", row["Middlename"].ToString())
                        .Set("lastname", row["Lastname"].ToString())
                        .Set("address", row["Address"].ToString())
                        .Set("birthdate", row["Birthdate"].ToString())
                        .Set("age", row["Age"].ToString())
                        .Set("email", row["Email"].ToString())
                        .Set("contact", row["Contact"].ToString())
                        .Set("year_level", row["Year_Level"].ToString())
                        .Set("degree", row["Degree"].ToString())
                        .Set("program", row["Program"].ToString())
                        .Set("major", row["Major"].ToString())
                        .Set("registration_status", row["Registration_Status"].ToString())
                        .Set("enrollment_status", row["Enrollment_Status"].ToString());

                    // Update the document in the MongoDB collection
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(documentId));
                    collection2.UpdateOne(filter, updateDefinition);
                }

                // Reload the data after saving changes
                LoadData();
            }
        }

        private void Crud1_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
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

                    // Remove the row from the DataTable
                    dataTable.Rows.RemoveAt(selectedRow.Index);
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }


            
        }
    }
}
