using System;
using System.Windows.Forms;
using UI.Controls.Viewport;

namespace UI.Windows
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            viewport.ActionChanged += Viewport_ActionChanged;
            viewport.MouseDown += Viewport_MouseDown;
            viewport.MouseMove += Viewport_MouseMove;
            viewport.DrawingMode = DrawingModes.Shaded;
            viewport.TopView();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await Modeling.DoWork(viewport, progressBar);
        }
        
        private void Viewport_MouseDown(object sender, MouseEventArgs e)
        {
            if (viewport.Action == ActionType.None)
                viewport.Invalidate();
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewport.Action == ActionType.None)
            {
                viewport.Invalidate();
            }
        }
        
        private void Viewport_ActionChanged(object sender, EventArgs e)
        {
            toolStripButtonPan.Checked = viewport.Action == ActionType.Pan;
            toolStripButtonZoomIn.Checked = viewport.Action == ActionType.Zoom;
        }
        
        private void ToolBarButtonZoomExtents_Click(object sender, EventArgs e)
        {
            viewport.ZoomExtents();
        }

        private void ToolBarButtonPan_Click(object sender, EventArgs e)
        {
            toolStripButtonPan.Checked = !toolStripButtonPan.Checked;
            viewport.Action = toolStripButtonPan.Checked ? ActionType.Pan : ActionType.None;
        }

        private void ToolBarButtonZoom_Click(object sender, EventArgs e)
        {
            toolStripButtonZoomIn.Checked = !toolStripButtonZoomIn.Checked;
            viewport.Action = toolStripButtonZoomIn.Checked ? ActionType.Zoom : ActionType.None;
        }
    }
}