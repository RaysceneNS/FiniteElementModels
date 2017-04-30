using System;
using System.Drawing;
using System.Threading.Tasks;
using Core.Algorythm;
using Core.Fem;
using UI.Controls.Viewport;
using UI.Controls.Viewport.Overlay;
using Label = System.Windows.Forms.Label;

namespace UI.Windows
{
	public static class Modeling
    {
        private const float TOLERANCE = 0.00001f;

        private static Model BracketModel(IProgress<TaskProgress> progressReport)
        {
            var lb = new LoopBuilder();
            lb.AddLineSegment(0, 0, 100, 0);
            lb.AddLineSegment(100, 0, 100, 10);
            lb.AddLineSegment(100, 10, 0, 10);
            lb.AddLineSegment(0, 10, 0, 0);

            var lb2 = new LoopBuilder();
            lb2.AddArc(48, 5, 2, 0, 360);

            var mesher = new BasicMesher();
            mesher.AddLoop(lb.Build(true, 1));
            mesher.AddLoop(lb2.Build(true, 1));

            // Perform a triangulation within the boundaries of the shape
            var model = mesher.TriangulateIteratively(progressReport);

            // apply Displacements and loads to the nodes of the model
            foreach (var node in model.Nodes)
            {
                if (Math.Abs(node.Y) < TOLERANCE && node.X < 65)
                {
                    node.FixAll();
                }

                if (node.X > 83 && node.X < 97 && Math.Abs(node.Y - 10) < TOLERANCE)
                    node.ApplyLoad(0, -50);
            }

            return model;
        }

        public static async Task DoWork(Viewport viewport, Label progressBar)
        {
            progressBar.Visible = true;
            viewport.Legend.Visible = false;

            var progressReport = new Progress<TaskProgress>(progress =>
            {
                progressBar.Text = progress.Text + " (" + progress.ProgressPercentage + "%)";
                System.Diagnostics.Trace.WriteLine(progress.Text);
            });

            var model = BracketModel(progressReport);

            //build a scene to display this model
            viewport.SceneObjects.Add(new FemScene(model, 0.1f)
            {
                PlotMode = PlotMode.PerNode
            });
            viewport.Invalidate();
            viewport.ZoomExtents();
            viewport.DrawingMode = DrawingModes.Wireframe;

            // Solve the unknowns in the Finite Element model
            await Task.Run(() => new PlanarStressSolver(model, 10, 30000, 0.25f).SolvePlaneStress(progressReport));

            // if the solver was not interrupted
            if (model.IsSolved)
            {
                await Task.Run(() => model.ComputeEdges());
                await Task.Run(() => model.PlotAverageVonMises());
            }

            viewport.Legend.Title = "Avg. Von Mises";
            viewport.Legend.MinValue = model.MinNodeValue;
            viewport.Legend.MaxValue = model.MaxNodeValue;
            viewport.Legend.Visible = true;

            AddLabels(viewport, model);
            
            viewport.SceneObjects.Compile();

            progressBar.Visible = false;
            viewport.DrawingMode = DrawingModes.Shaded;
        }

        private static void AddLabels(Viewport viewport, Model fm)
        {
            viewport.Labels.Clear();

            //label max displacement
            viewport.Labels.Add(new FlagLabel(
                fm.MaxNode.X + fm.MaxNode.UX,
                fm.MaxNode.Y + fm.MaxNode.UY,
                0,
                "max",
                SystemFonts.DefaultFont,
                Color.Beige,
                Color.Black));
        }
    }
}
