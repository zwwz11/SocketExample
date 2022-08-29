using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoketExample
{
    public partial class SelectForm : Form
    {
        private string connectionType = string.Empty;
        public string ConnectionType => connectionType;

        public SelectForm()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            this.connectionType = btn.Name.Contains("Server") ? "Server" : "Client";
            this.Close();
        }
    }
}
