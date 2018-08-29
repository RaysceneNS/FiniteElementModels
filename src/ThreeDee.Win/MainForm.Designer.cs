using System.ComponentModel;
using System.Windows.Forms;
using UI.Controls.Viewport;

namespace UI.Windows
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRotate = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPan = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomWin = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExtent = new System.Windows.Forms.ToolStripButton();
            this.panelViewport = new System.Windows.Forms.Panel();
            this.viewport = new Viewport();
            this.progressBar = new System.Windows.Forms.Label();
            this.toolBar.SuspendLayout();
            this.panelViewport.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(461, 290);
            // 
            // toolBar
            // 
            this.toolBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolBar.AutoSize = false;
            this.toolBar.Dock = System.Windows.Forms.DockStyle.None;
            this.toolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRotate,
            this.toolStripButtonPan,
            this.toolStripButtonZoomIn,
            this.toolStripButtonZoomWin,
            this.toolStripButtonExtent});
            this.toolBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolBar.Location = new System.Drawing.Point(769, 8);
            this.toolBar.Name = "toolBar";
            this.toolBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolBar.Size = new System.Drawing.Size(24, 244);
            this.toolBar.TabIndex = 72;
            // 
            // toolStripButtonRotate
            // 
            this.toolStripButtonRotate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRotate.Image = global::UI.Windows.Properties.Resources.Rotate;
            this.toolStripButtonRotate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRotate.Name = "toolStripButtonRotate";
            this.toolStripButtonRotate.Size = new System.Drawing.Size(22, 20);
            this.toolStripButtonRotate.Click += new System.EventHandler(this.ToolBarButtonRotate_Click);
            // 
            // toolStripButtonPan
            // 
            this.toolStripButtonPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPan.Image = global::UI.Windows.Properties.Resources.Pan;
            this.toolStripButtonPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPan.Name = "toolStripButtonPan";
            this.toolStripButtonPan.Size = new System.Drawing.Size(22, 20);
            this.toolStripButtonPan.Click += new System.EventHandler(this.ToolBarButtonPan_Click);
            // 
            // toolStripButtonZoomIn
            // 
            this.toolStripButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomIn.Image = global::UI.Windows.Properties.Resources.ZoomIn;
            this.toolStripButtonZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomIn.Name = "toolStripButtonZoomIn";
            this.toolStripButtonZoomIn.Size = new System.Drawing.Size(22, 20);
            this.toolStripButtonZoomIn.Click += new System.EventHandler(this.ToolBarButtonZoom_Click);
            // 
            // toolStripButtonZoomWin
            // 
            this.toolStripButtonZoomWin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomWin.Image = global::UI.Windows.Properties.Resources.ZoomWindow;
            this.toolStripButtonZoomWin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomWin.Name = "toolStripButtonZoomWin";
            this.toolStripButtonZoomWin.Size = new System.Drawing.Size(22, 20);
            this.toolStripButtonZoomWin.Click += new System.EventHandler(this.ToolBarButtonZoomWindow_Click);
            // 
            // toolStripButtonExtent
            // 
            this.toolStripButtonExtent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonExtent.Image = global::UI.Windows.Properties.Resources.FitToSize;
            this.toolStripButtonExtent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExtent.Name = "toolStripButtonExtent";
            this.toolStripButtonExtent.Size = new System.Drawing.Size(22, 20);
            this.toolStripButtonExtent.Click += new System.EventHandler(this.ToolBarButtonZoomExtents_Click);
            // 
            // panelViewport
            // 
            this.panelViewport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelViewport.Controls.Add(this.progressBar);
            this.panelViewport.Controls.Add(this.toolBar);
            this.panelViewport.Controls.Add(this.viewport);
            this.panelViewport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelViewport.Location = new System.Drawing.Point(0, 0);
            this.panelViewport.Name = "panelViewport";
            this.panelViewport.Size = new System.Drawing.Size(792, 573);
            this.panelViewport.TabIndex = 0;
            // 
            // viewport
            // 
            this.viewport.Action = ActionType.None;
            this.viewport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.viewport.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.viewport.DrawingMode = DrawingModes.Shaded;
            this.viewport.Location = new System.Drawing.Point(0, 8);
            this.viewport.Name = "viewport";
            this.viewport.OrientationIndicatorVisible = true;
            this.viewport.Size = new System.Drawing.Size(769, 564);
            this.viewport.TabIndex = 1;
            // 
            // progressBar
            // 
            this.progressBar.AutoSize = true;
            this.progressBar.Location = new System.Drawing.Point(246, 150);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(0, 13);
            this.progressBar.TabIndex = 73;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.panelViewport);
            this.IsMdiContainer = true;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.Text = "3D Window";
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.panelViewport.ResumeLayout(false);
            this.panelViewport.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Viewport viewport;
        private ToolStripPanel BottomToolStripPanel;
        private ToolStripPanel TopToolStripPanel;
        private ToolStripPanel RightToolStripPanel;
        private ToolStripPanel LeftToolStripPanel;
        private ToolStripContentPanel ContentPanel;
        private ToolStrip toolBar;
        private Panel panelViewport;
        private ToolStripButton toolStripButtonRotate;
        private ToolStripButton toolStripButtonPan;
        private ToolStripButton toolStripButtonZoomIn;
        private ToolStripButton toolStripButtonZoomWin;
        private ToolStripButton toolStripButtonExtent;
        private Label progressBar;
    }
}

