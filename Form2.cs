using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bayambang_ExtractData
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DAL dal = new DAL();
            if (dal.IsConnectionOK(textBox1.Text))
            {
                MessageBox.Show("Connection is success!");
            }
            else
            {
                MessageBox.Show("Connection failed!");
            }
            dal.Dispose();
            dal = null;
        }
    }
}
