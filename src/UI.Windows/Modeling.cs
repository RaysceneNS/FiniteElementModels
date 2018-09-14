using System;
using System.Drawing;
using System.Threading.Tasks;
using Core.Algorithm;
using Core.Fea;
using UI.Controls.Viewport;

namespace UI.Windows
{
    public static class Modeling
    {
        private const float TOLERANCE = 0.00001f;

        private static Model BracketModel(IProgress<TaskProgress> progressReport)
        {
            var meshBuilder = new MeshBuilder()
                .AddLoop(new LoopBuilder().AddRectangle(25, 5, 25, 5).Build())
                .AddLoop(new LoopBuilder().AddCircle(26, 5, 4).Build())
                .AddLoop(new LoopBuilder().AddCircle(9, 5, 4).Build())
                .AddLoop(new LoopBuilder().AddCircle(41, 5, 4).Build());

            // Perform a triangulation within the boundaries of the shape
            var model = meshBuilder.TriangulateIteratively(progressReport);

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

        public static async Task DoWork(Viewport viewport)
        {
            viewport.Labels.Clear();

            var progressReport = new Progress<TaskProgress>(progress =>
            {
                System.Diagnostics.Trace.WriteLine(progress.Text + " (" + progress.ProgressPercentage + "%)");
            });

            var model = BracketModel(progressReport);

            //build a scene to display this model
            var scene = new FeaScene(model);
            scene.Compile();
            viewport.SceneObjects.Add(scene);
            viewport.LookAt(scene);
            viewport.ZoomExtents();
            
            // Solve the unknowns in the Finite Element model
            await Task.Run(() => model.Solve(progressReport));

            // if the solver was not interrupted
            if (model.IsSolved)
            {
                await Task.Run(() => model.ComputeEdges());
                await Task.Run(() => model.PlotAverageVonMises());
            }

            scene.Compile();
            viewport.Legend.Show("Avg. Von Mises", model.MaxNodeValue, model.MinNodeValue);

            viewport.Labels.Add(new LabelFlag(
                model.MaxNode.X + model.MaxNode.FreedomX,
                model.MaxNode.Y + model.MaxNode.FreedomY,
                0,
                "max",
                SystemFonts.DefaultFont,
                Color.Beige,
                Color.Black));
            
            viewport.Invalidate();
        }
    }
}
