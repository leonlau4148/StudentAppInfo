using Amazon.Auth.AccessControlPolicy;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using MongoDB.Bson;
using MongoDB.Driver;
using StudentAppInfo.crystalreports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Admin2 : UserControl
    {
        private IMongoCollection<BsonDocument> collection, collection2,collection3;
       
        public Admin2()
        {
            InitializeComponent();
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("department");
            collection = db.GetCollection<BsonDocument>("adduser");
            collection2 = db.GetCollection<BsonDocument>("users");
            collection3 = db.GetCollection<BsonDocument>("schoolprograms");
            // Query the database to get the list of students
         
        }

        private void PopulateComboBox(IMongoCollection<BsonDocument> collection, string fieldName, System.Windows.Forms.ComboBox comboBox)
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
            var distinctValues = collection3.Aggregate<BsonDocument>(pipeline)
                .ToList() // Execute the aggregation
                .Select(doc => doc["_id"].AsString) // Extract the distinct values
                .ToList();

            // Add the distinct values to the ComboBox
            foreach (var value in distinctValues)
            {
                comboBox.Items.Add(value);
            }
        }

    


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear the major ComboBox
            comboBox3.Items.Clear();
            comboBox3.Text = "";


            // Get the selected program
            string selectedProgram = comboBox2.Text;

            if (!string.IsNullOrEmpty(selectedProgram))
            {
                // Filter the collection by the selected program
                var filter = Builders<BsonDocument>.Filter.Eq("program", selectedProgram);
                var programDocument = collection3.Find(filter).FirstOrDefault();

                if (programDocument != null)
                {

                    // Populate the major ComboBox with majors for the selected program
                    var majors = programDocument["major"].AsBsonArray.Select(v => v.AsString).ToList();
                    foreach (var major in majors)
                    {
                        if (major == "")
                        {
                            comboBox3.Enabled = false;
                        }
                        else
                        {
                            comboBox3.Enabled = true;
                            comboBox3.Items.Add(major);
                        }

                    }
                }
            }
        }


     
        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            if(comboBox2.Text == "")
            {
                comboBox3.Enabled = false;
            }
        }


        private void Admin2_Load(object sender, EventArgs e)
        {

            // Populate the degree ComboBox
            PopulateComboBox(collection3, "degree", comboBox1);
            // Populate the program ComboBox
            PopulateComboBox(collection3, "program", comboBox2);

            // Populate the major ComboBox
            PopulateComboBox(collection3, "major", comboBox3);

            comboBox3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filters = new List<FilterDefinition<BsonDocument>>();

            // Add the role filter
            filters.Add(filterBuilder.Eq("role", "student"));

            // Check each combo box and add the corresponding filter if a value is selected
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                filters.Add(filterBuilder.Eq("degree", comboBox1.Text));
            }

            if (!string.IsNullOrEmpty(comboBox2.Text))
            {
                filters.Add(filterBuilder.Eq("program", comboBox2.Text));
            }

            if (!string.IsNullOrEmpty(comboBox3.Text))
            {
                filters.Add(filterBuilder.Eq("major", comboBox3.Text));
            }

            if (!string.IsNullOrEmpty(comboBox4.Text))
            {
                if (comboBox4.Text == "All Year Level")
                {
                    filters.Add(filterBuilder.In("year_level", new BsonArray { "1st Year", "2nd Year", "3rd Year", "4th Year", "5th Year" } ));
                }
                else
                {
                    filters.Add(filterBuilder.Eq("year_level", comboBox4.Text));
                }
               
            }

            if (!string.IsNullOrEmpty(comboBox5.Text))
            {
                if (comboBox5.Text == "New/Old")
                {
                    // Handle the "New/Old" case by adding a filter that checks for both "New" and "Old"
                    filters.Add(filterBuilder.In("student_status", new BsonArray { "New", "Old" }));
                }
                else
                {
                    filters.Add(filterBuilder.Eq("student_status", comboBox5.Text));
                }

            }
            if (!string.IsNullOrEmpty(comboBox6.Text))
            {
                if(comboBox6.Text == "Registered/Not Registered")
                {
                    // Handle the "Registered/Not Registered" case by adding a filter that checks for both "Registered" and "Not Registered"
                    filters.Add(filterBuilder.In("registration_status", new BsonArray { "Registered", "Not Registered" }));
                }
                else
                {
                    filters.Add(filterBuilder.Eq("registration_status", comboBox6.Text));
                }
               
            }
            if (!string.IsNullOrEmpty(comboBox7.Text))
            {
                if (comboBox7.Text == "Enrolled/Not Enrolled")
                {
                    // Handle the "Enrolled/Not Enrolled" case by adding a filter that checks for both "Enrolled" and "Not Enrolled"
                    filters.Add(filterBuilder.In("enrollment_status", new BsonArray { "Enrolled", "Not Enrolled" }));
                }
                else
                {
                    filters.Add(filterBuilder.Eq("enrollment_status", comboBox7.Text));
                }
                
            }

            // Combine all filters using the And operator
            FilterDefinition<BsonDocument> filterview;
            if (filters.Count == 1)
            {
                filterview = filters[0]; // If there's only one filter, use it directly
            }
            else
            {
                filterview = filterBuilder.And(filters); // Combine all filters with And
            }



            var students = collection.Find(filterview).ToList();

            // Create a DataTable to hold the data
            DataTable dt = new DataTable();

            dt.Columns.Add("firstname");
            dt.Columns.Add("middlename");
            dt.Columns.Add("lastname");
            dt.Columns.Add("year_level");
            dt.Columns.Add("degree");
            dt.Columns.Add("program");
            dt.Columns.Add("major");
            dt.Columns.Add("student_status");
            dt.Columns.Add("registration_status");
            dt.Columns.Add("enrollment_status");

            foreach (var student in students)
            {
                var row = dt.NewRow();
                row["firstname"] = student.GetValue("firstname").AsString;
                row["middlename"] = student.GetValue("middlename").AsString;
                row["lastname"] = student.GetValue("lastname").AsString;
                row["year_level"] = student.GetValue("year_level").AsString;
                row["degree"] = student.GetValue("degree").AsString;
                row["program"] = student.GetValue("program").AsString;
                row["major"] = student.GetValue("major").AsString;
                row["student_status"] = student.GetValue("student_status").AsString;
                row["registration_status"] = student.GetValue("registration_status").AsString;
                row["enrollment_status"] = student.GetValue("enrollment_status").AsString;
                dt.Rows.Add(row);
            }


            string filterExpression = "List of all student by ";
            List<string> filterss = new List<string>();

            // Check each combo box and add the corresponding filter if a value is selected
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                filterss.Add("Degree");
            }

            if (!string.IsNullOrEmpty(comboBox2.Text))
            {
                filterss.Add("Program");
            }

            if (!string.IsNullOrEmpty(comboBox3.Text))
            {
                filterss.Add("Major");
            }

            if (!string.IsNullOrEmpty(comboBox4.Text))
            {
                filterss.Add("Year Level");
            }

            if (!string.IsNullOrEmpty(comboBox5.Text))
            {
                if(comboBox5.Text == "Old")
                {
                    filterss.Add("Old Students");
                }
                else if (comboBox5.Text == "New")
                {
                    filterss.Add("New Students");
                }
                else
                {
                    filterss.Add("New/Old Students");
                }
            }

            if (!string.IsNullOrEmpty(comboBox6.Text))
            {
                if (comboBox6.Text == "Registered")
                {
                    filterss.Add("Registered");
                }
                else if (comboBox6.Text == "Not Registered")
                {
                    filterss.Add("Not Registered");
                }
                else
                {
                    filterss.Add("Registered/Not Registered");
                }
               
            }

            if (!string.IsNullOrEmpty(comboBox7.Text))
            {
               if(comboBox7.Text =="Enrolled")
                {
                    filterss.Add("Enrolled");
                }
               else if (comboBox7.Text == "Not Enrolled")
                {
                    filterss.Add("Not Enrolled");
                }
               else
                {
                      filterss.Add("Enrolled/Not Enrolled");
                 }
            }

            // Join the filters with commas and "and" for the last item
            if (filterss.Count > 0)
            {
                filterExpression += string.Join(", ", filterss.Take(filterss.Count - 1));
                if (filterss.Count > 1)
                {
                    filterExpression += " and " + filterss.Last();
                }
                else
                {
                    filterExpression += "" + filterss.Last();
                }
            }
            else
            {
                filterExpression = "List of all student"; // No filters selected
            }

            // Now you can use the filterExpression in your Crystal Reports record selection formula


            // Now you can use the filterExpression in your Crystal Reports record selection formula
            ReportDocument report = new ReportDocument();

           //parameter field
            ParameterFields paramFields = new ParameterFields();
            ParameterField paramField = new ParameterField();
            ParameterDiscreteValue paramDiscreteValue = new ParameterDiscreteValue();
            paramField.Name = "filter";
            paramDiscreteValue.Value = filterExpression;
            paramField.CurrentValues.Add(paramDiscreteValue);
            paramFields.Add(paramField);
            crystalReportViewer1.ParameterFieldInfo = paramFields;
            report.Load("C:\\Users\\Raven\\source\\repos\\StudentAppInfo\\StudentAppInfo\\crystalreports\\listofstudent.rpt");
            report.SetDataSource(dt);
            crystalReportViewer1.ReportSource = report;

        }
    }
}
