using System;
using System.Windows.Forms;
using UI.Controls.Viewport;

namespace UI.Windows
{
    /// <summary>
    ///     The main window for the application
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The constructor for our UI
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            viewport.ActionChanged += Viewport_ActionChanged;
            viewport.MouseDown += Viewport_MouseDown;
            viewport.MouseMove += Viewport_MouseMove;
            viewport.OrientationIndicatorVisible = true;
            viewport.DrawingMode = DrawingModes.Shaded;
            viewport.Legend.Visible = true;
            viewport.TopView();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await Modeling.DoWork(viewport, progressBar);
        }
        
        /// <summary>
        /// Handles the MouseDown event of the viewport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs" /> instance containing the event data.</param>
        private void Viewport_MouseDown(object sender, MouseEventArgs e)
        {
            if (viewport.Action == ActionType.None)
                viewport.Invalidate();
        }

        /// <summary>
        /// Handles the MouseMove event of the viewport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs" /> instance containing the event data.</param>
        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewport.Action == ActionType.None)
            {
                viewport.Invalidate();
            }
        }
        
        /// <summary>
        /// Handles the ActionChanged event of the viewport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void Viewport_ActionChanged(object sender, EventArgs e)
        {
            toolStripButtonPan.Checked = viewport.Action == ActionType.Pan;
            toolStripButtonZoomIn.Checked = viewport.Action == ActionType.Zoom;
            toolStripButtonZoomWin.Checked = viewport.Action == ActionType.ZoomWindow;
            toolStripButtonRotate.Checked = viewport.Action == ActionType.Rotate;
        }
        
        /// <summary>
        /// Zoom fit
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void ToolBarButtonZoomExtents_Click(object sender, EventArgs e)
        {
            viewport.ZoomExtents();
        }

        /// <summary>
        ///     Handles the Click event of the toolStripButtonActionRotate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void ToolBarButtonRotate_Click(object sender, EventArgs e)
        {
            toolStripButtonRotate.Checked = !toolStripButtonRotate.Checked;
            viewport.Action = toolStripButtonRotate.Checked ? ActionType.Rotate : ActionType.None;
        }

        /// <summary>
        ///     Handles the Click event of the toolStripButtonActionPan control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void ToolBarButtonPan_Click(object sender, EventArgs e)
        {
            toolStripButtonPan.Checked = !toolStripButtonPan.Checked;
            viewport.Action = toolStripButtonPan.Checked ? ActionType.Pan : ActionType.None;
        }

        /// <summary>
        ///     Handles the Click event of the toolStripButtonActionZoom control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void ToolBarButtonZoom_Click(object sender, EventArgs e)
        {
            toolStripButtonZoomIn.Checked = !toolStripButtonZoomIn.Checked;
            viewport.Action = toolStripButtonZoomIn.Checked ? ActionType.Zoom : ActionType.None;
        }

        /// <summary>
        ///     Zoom window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void ToolBarButtonZoomWindow_Click(object sender, EventArgs e)
        {
            toolStripButtonZoomWin.Checked = !toolStripButtonZoomWin.Checked;
            viewport.Action = toolStripButtonZoomWin.Checked ? ActionType.ZoomWindow : ActionType.None;
        }
    }
}