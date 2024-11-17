using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ASM1DATAE_BH01350
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();

            
        }

        

        private void Form6_Load(object sender, EventArgs e)
        {
            // Replace this connection string with your actual DB connection string.
            string connectionString = "your_connection_string_here";
            string query = "SELECT COUNT(*) AS SessionCount FROM Sessions";

            

            // Load other statistics similarly
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // You can set up code here to refresh the stats or handle button events
        }
    

        private void chart1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            chart1.Series["Series1"].Points.AddXY("Nov 14", 2000);
            chart1.Series["Series1"].Points.AddXY("Nov 21", 3000);
            chart1.Series["Series1"].Points.AddXY("Nov 28", 4000);

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
