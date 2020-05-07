using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionBDDApp
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void importerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImporterMenu ImporterMenu = new ImporterMenu())
            {
                ImporterMenu.StartPosition = FormStartPosition.CenterParent;
                ImporterMenu.ShowDialog(this);
            }
        }
    }
}
