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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace StudentAppInfo
{
    public partial class Admin1 : UserControl
    {
        private Prompt _prompt = new Prompt();
        private IMongoCollection<BsonDocument> collection;
        private DataTable dataTable = null;
        public Admin1()
        {
            InitializeComponent();
            var dbClient = new MongoClient("mongodb://localhost:27017/");
            IMongoDatabase db = dbClient.GetDatabase("department");
            collection = db.GetCollection<BsonDocument>("schoolprograms");
            LoadData();
           
        }
        private void LoadData()
        {
            // Query the MongoDB collection
            var documents = collection.Find(new BsonDocument()).ToList();

            // Convert the documents to a DataTable
            dataTable = new DataTable();

            // Define the expected columns for the DataTable
            dataTable.Columns.Add("_id");
            dataTable.Columns.Add("degree");
            dataTable.Columns.Add("program");
            dataTable.Columns.Add("major");

            // Modify the column names to capitalize the first letter
            foreach (DataColumn column in dataTable.Columns)
            {
                column.ColumnName = Char.ToUpper(column.ColumnName[0]) + column.ColumnName.Substring(1);
            }

            foreach (var document in documents)
            {
                var id = document.GetValue("_id", "").ToString();
                var degree = document.Contains("degree") ? document["degree"].ToString() : "";
                var program = document.Contains("program") ? document["program"].ToString() : "";

                // Check if the document has a Major array
                if (document.TryGetValue("major", out var majorArrayBson) && majorArrayBson.IsBsonArray)
                {
                    var majorArray = majorArrayBson.AsBsonArray;

                    foreach (var majorBson in majorArray)
                    {
                        var major = majorBson.AsString;

                        // Create a new row for each major within the same document
                        var row = dataTable.NewRow();
                        row["_id"] = id;
                        row["degree"] = degree;
                        row["program"] = program;
                        row["major"] = major;
                        dataTable.Rows.Add(row);
                    }
                }
                else
                {
                    // If there is no Major array, add the document as a single row
                    var row = dataTable.NewRow();
                    row["_id"] = id;
                    row["degree"] = degree;
                    row["program"] = program;
                    // Add an empty string for Majors column since there are no majors in this case
                    row["major"] = string.Empty;
                    dataTable.Rows.Add(row);
                }
            }

            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = dataTable;

            dataGridView1.Columns["_id"].Visible = false;

            // Auto size the columns
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            
        }

        private void delete()
        {
            // Check if any row is selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Get the _id of the selected document
                var documentId = selectedRow.Cells["_id"].Value.ToString();

                // Get the major value to be deleted
                string majorToDelete = selectedRow.Cells["major"].Value.ToString();

                // Find the document by its _id
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(documentId));
                var document = collection.Find(filter).FirstOrDefault();

                if (document != null)
                {
                    // Check if the document has a Major array
                    if (document.TryGetValue("major", out var majorArrayBson) && majorArrayBson.IsBsonArray)
                    {
                        var majorArray = majorArrayBson.AsBsonArray;

                        // If the major to delete is in the array, remove it
                        if (majorArray.Remove(new BsonString(majorToDelete)))
                        {
                            // If there are no majors left, delete the entire document
                            if (majorArray.Count == 0)
                            {
                                collection.DeleteOne(filter);
                            }
                            else
                            {
                                // Update the document with the new majors array
                                var updateDefinition = Builders<BsonDocument>.Update.Set("major", majorArray);
                                collection.UpdateOne(filter, updateDefinition);
                            }

                            // Reload the data after saving changes
                            LoadData();
                            // Display a success message
                            MessageBox.Show("Deleted successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Major value not found in the majors array.");
                        }
                    }
                    else
                    {
                        // If there is no Major array, delete the entire document
                        collection.DeleteOne(filter);
                        // Reload the data after saving changes
                        LoadData();
                        // Display a success message
                        MessageBox.Show("Deleted successfully.");
                    }
                }
                else
                {
                    MessageBox.Show("Document not found.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete a major or the program and degree.");
            }
        }

        private void update()
        {
            // Check if any row is selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Get the _id of the selected document
                var documentId = selectedRow.Cells["_id"].Value.ToString();

                // Get the original major value from the selected row
                string originalMajorValue = selectedRow.Cells["major"].Value.ToString();

                // Get the new major value from the text box
                string degree = textBox1.Text;
                string program = textBox2.Text;
                string newMajorValue = textBox3.Text.Trim();

                // Find the document by its _id
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(documentId));
                var document = collection.Find(filter).FirstOrDefault();

                if (document != null)
                {


                    // Extract the majors array from the document
                    if (document.TryGetValue("major", out var majorsArrayBson) && majorsArrayBson.IsBsonArray)
                    {
                        var majorsArray = majorsArrayBson.AsBsonArray;

                        // Find the index of the original major value in the array
                        int majorIndex = majorsArray.IndexOf(new BsonString(originalMajorValue));

                        // Check if the majorIndex is within the bounds of the array
                        if (majorIndex >= 0)
                        {
                            // Update the major at the specified index
                            majorsArray[majorIndex] = new BsonString(newMajorValue);

                            // Define the update definition to set the new majors array
                            var updateDefinition = Builders<BsonDocument>.Update.Set("major", majorsArray);
                            //update also the degree and program
                            updateDefinition = updateDefinition.Set("degree", degree);
                            updateDefinition = updateDefinition.Set("program", program);
                            // Update the document in the MongoDB collection
                            collection.UpdateOne(filter, updateDefinition);

                            // Reload the data after saving changes
                            LoadData();
                            // Display a success message
                            MessageBox.Show("Updated successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Original major value not found in the majors array.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Document does not have a majors array.");
                    }
                }
                else
                {
                    MessageBox.Show("Document not found.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private void add()
        {
          
                // Get the selected values from the ComboBoxes
                string degree = textBox1.Text;
                string program = textBox2.Text;
                string major = textBox3.Text;

                // Create a new BsonDocument with the selected values


                var newData = new BsonDocument
            {

                { "degree", degree },
                { "program", program },
                { "major", new BsonArray(major.Split(',')) } // Assuming majors are comma-separated
            };

                // Check if a document with the same program exists
                var filter = Builders<BsonDocument>.Filter.Eq("program", program);
                var existingDocument = collection.Find(filter).FirstOrDefault();

                if (existingDocument != null)
                {
                    // If the degree is the same, add the major to the majors array if it's not already present
                    if (existingDocument["degree"].AsString == degree)
                    {
                        var majorsArray = existingDocument["major"].AsBsonArray;
                        var newMajor = new BsonString(major);

                        if (!majorsArray.Contains(newMajor))
                        {
                            majorsArray.Add(newMajor);

                            // Update the document with the new majors array
                            var update = Builders<BsonDocument>.Update.Set("major", majorsArray);
                            collection.UpdateOne(filter, update);
                            MessageBox.Show("Data Updated");
                        }
                    }
                    else
                    {
                        // If the degree is different, save the new document to the database
                        collection.InsertOne(newData);
                        MessageBox.Show("New Data Added");
                    }
                }
                else
                {
                    // If no document with the same program exists, save the new document to the database
                    collection.InsertOne(newData);
                    MessageBox.Show("New Data Added");
                }
            

            // Reload the data after saving changes
            LoadData();
        }

        public void Cleartextbox()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            if (dataGridView1.Rows.Count > 0)
            {
                // Select the first row
                dataGridView1.Rows[0].Selected = true;
                // Scroll to the first row if it's not visible
                dataGridView1.FirstDisplayedScrollingRowIndex = 0;
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
                textBox1.Text = row.Cells["degree"].Value.ToString();
                textBox2.Text = row.Cells["program"].Value.ToString();
                textBox3.Text = row.Cells["major"].Value.ToString();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to add the changes?", "add Confirmation");
            if (userSelectedYes)
            {
                
                add();
                Cleartextbox();
            }
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to update the changes?", "Update Confirmation");
            if (userSelectedYes)
            {
                update();
                Cleartextbox();
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Cleartextbox();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to delete the changes?", "Delete Confirmation");
            if (userSelectedYes)
            {
                delete();
                Cleartextbox();
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
                    textBox1.Text = row.Cells["degree"].Value.ToString();
                    textBox2.Text = row.Cells["program"].Value.ToString();
                    textBox3.Text = row.Cells["major"].Value.ToString();
                }
            }
        }
    }
}
