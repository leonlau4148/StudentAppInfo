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
    public partial class Form4 : Form
    {
        private Prompt _prompt = new Prompt();
        bool sidebarexpand = true;
        public Form4()
        {
            InitializeComponent();
        }
     
        public void Guide()
        {
            if (sidebarexpand)
            {
                panel5.Padding = new Padding(0, 0, 0, 0);
            }
            else
            {

                panel5.Padding = new Padding(55, 0, 0, 0);
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            
            if (sidebarexpand)
            {
                flowLayoutPanel1.Size = new Size(60, 600);
                if (button6clicked)
                {
                    panel5.Padding = new Padding(0, 0, 0, 0);
                }
                else if (button2clicked)
                {
               
                  panel5.Padding = new Padding(0, 0, 0, 0);
                 
                }
    
                else
                {
                    panel5.Padding = new Padding(55, 0, 0, 0);
                }
      
                sidebarexpand = false;
            }
            else
            {
                flowLayoutPanel1.Size = new Size(186, 600);
                sidebarexpand = true;
                panel5.Padding = new Padding(0, 0, 0, 0);
            }
        }
       

        private void Button4_Click(object sender, EventArgs e)
        {
            bool userSelectedYes = _prompt.ShowPrompt("Do you want to exit?", "Exit");
            if (userSelectedYes)
            {
                // Do something if the user selected yes
                System.Environment.Exit(1);
            }
        }

     

        private void Button3_Click(object sender, EventArgs e)
        {
            Admin1 us1 = new Admin1();
            panel5.Controls.Clear();
            us1.Dock = DockStyle.Fill;
            panel5.Controls.Add(us1);
            button6clicked = false;
            button2clicked = false;
            Guide();
        }
       bool button2clicked = false;
        private void Button2_Click(object sender, EventArgs e)
        {
            Crud2 us2 = new Crud2();
            panel5.Controls.Clear();
            us2.Dock = DockStyle.Fill;
            //padding us2
          
            panel5.Controls.Add(us2);
            panel5.Padding = new Padding(0, 0, 0, 0);
            button6clicked = false;
            button2clicked = true;
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            User1 us1 = new User1
            {
                Dock = DockStyle.Fill
            };
            panel5.Controls.Clear();
            panel5.Controls.Add(us1);
            button6clicked = false;
            button2clicked = false;
            //Guide();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            User1 us1 = new User1
            {
                Dock = DockStyle.Fill
            };
            panel5.Controls.Add(us1);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            Admin3 us1 = new Admin3
            {
                Dock = DockStyle.Fill
            };
            panel5.Controls.Clear();
            panel5.Controls.Add(us1);
            button6clicked = false;
            Guide();
        }
       bool button6clicked = false;
        private void button6_Click(object sender, EventArgs e)
        {
            Admin2 us1 = new Admin2
            {
                Dock = DockStyle.Fill
            };
            panel5.Controls.Clear();
            panel5.Controls.Add(us1);
            panel5.Padding = new Padding(0, 0, 0, 0);
            button6clicked = true;
        }


        private void button8_Click(object sender, EventArgs e)
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
