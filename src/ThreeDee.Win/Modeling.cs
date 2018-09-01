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
            var mesher = new BasicMesher()
                .AddLoop(new LoopBuilder().AddRectangle(25, 5, 25, 5).Build())
                .AddLoop(new LoopBuilder().AddCircle(26, 5, 4).Build())
                .AddLoop(new LoopBuilder().AddCircle(9, 5, 4).Build())
                .AddLoop(new LoopBuilder().AddCircle(41, 5, 4).Build());

            // Perform a triangulation within the boundaries of the shape
            var model = mesher.TriangulateIteratively(progressReport);

            // apply Displacements and loads to the nodes of the model
            foreach (var node in model.Nodes)
            {
                if (Math.Abs(node.Y) < TOLERANCE && node.X < 5)
                {
                    node.FixAll();
                }
                if (Math.Abs(node.Y) < TOLERANCE && node.X > 45)
                {
                    node.FixAll();
                }

                if (node.X > 20 && node.X < 26 && Math.Abs(node.Y - 10) < TOLERANCE)
                    node.ApplyLoad(0, (node.X - 20) * -250);
                if (node.X >= 26 && node.X < 31 && Math.Abs(node.Y - 10) < TOLERANCE)
                    node.ApplyLoad(0, (31 - node.X) * -250);
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
            
            viewport.Labels.Add(new FlagLabel(
                fm.MaxNode.X + fm.MaxNode.FreedomX,
                fm.MaxNode.Y + fm.MaxNode.FreedomY,
                0,
                "max",
                SystemFonts.DefaultFont,
                Color.Beige,
                Color.Black));
        }
    }
}
