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
    public partial class Form3 : Form
    {
        private Prompt _prompt = new Prompt();
        private string userEmail;
        public Form3(string emaildbb)
        {
            InitializeComponent();
            userEmail = emaildbb;
        }

        bool sidebarexpand = true;
        private void button5_Click(object sender, EventArgs e)
        {
            if (sidebarexpand)
            {
                flowLayoutPanel1.Size = new Size(60, 600);
                sidebarexpand = false;
                panel5.Padding = new Padding(55, 0, 0, 0);
            }
            else
            {
                flowLayoutPanel1.Size = new Size(186, 600);
                sidebarexpand = true;
                panel5.Padding = new Padding(0, 0, 0, 0);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to exit?", "Exit");
            if (userSelectedYes)
            {
                // Do something if the user selected yes
                System.Environment.Exit(1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            User3 us1 = new User3
            {
                Dock = DockStyle.Fill
            };
            panel5.Controls.Clear();
            panel5.Controls.Add(us1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            User2 us2 = new User2(userEmail)
            {
                Dock = DockStyle.Fill
            };
            panel5.Controls.Clear();
            panel5.Controls.Add(us2);
        }

    

        private void Form3_Load(object sender, EventArgs e)
        {
            User3 us1 = new User3
            {
                Dock = DockStyle.Fill
            };
            panel5.Controls.Add(us1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to Log out?", "Log out");
            if (userSelectedYes)
            {
                // Do something if the user selected yes
                Form1 fr1 = new Form1();
                fr1.Show();
                this.Hide();
            }
        }
    }
}
