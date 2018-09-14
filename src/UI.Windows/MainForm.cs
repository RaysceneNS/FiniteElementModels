using System;
using System.Windows.Forms;

namespace UI.Windows
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await Modeling.DoWork(viewport);
        }
    }
}