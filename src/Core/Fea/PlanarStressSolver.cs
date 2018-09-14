using System;
using Core.Algorithm;

namespace Core.Fea
{
    /// <summary>
    /// A basic 2D planar stress solver class
    /// </summary>
    public class PlanarStressSolver
    {
        private const int DEGREES_OF_FREEDOM = 2;
        private readonly float _youngsModulus;
        private readonly float _thickness;
        private readonly float _poissonsRatio;
        private readonly float _minimumLoad;
        private int _numElements;
        private readonly Model _model;

        public PlanarStressSolver(Model model, float thickness, float youngsModulus, float poissonsRatio)
        {
            this._model = model;
            this._minimumLoad = 0.001f;

            this._thickness = thickness;
            this._youngsModulus = youngsModulus;
            this._poissonsRatio = poissonsRatio;
        }

        public void SolvePlaneStress(IProgress<TaskProgress> progressReport)
        {
            // set an i at each node
            var counter = 0;
            foreach (var node in this._model.Nodes)
            {
                node.SetIndex(counter++);
            }
            this._numElements = counter * DEGREES_OF_FREEDOM;

            // create storage for solving variables
            var isDisplaced = new bool[this._numElements];
            var displacements = new float[this._numElements];
            var loads = new float[this._numElements];

            //copy loadings and constraints into triangles arrays
            foreach (var node in this._model.Nodes)
            {
                if (node.Loaded)
                {
                    loads[node.Index * DEGREES_OF_FREEDOM] = node.LoadX;
                    loads[node.Index * DEGREES_OF_FREEDOM + 1] = node.LoadY;
                }

                if (node.Constrained)
                {
                    // x constraint?
                    if (node.ConstraintX)
                    {
                        isDisplaced[node.Index * DEGREES_OF_FREEDOM] = true;
                        displacements[node.Index * DEGREES_OF_FREEDOM] = node.DisplacementX;
                    }

                    // y constraint?
                    if (node.ConstraintY)
                    {
                        isDisplaced[node.Index * DEGREES_OF_FREEDOM + 1] = true;
                        displacements[node.Index * DEGREES_OF_FREEDOM + 1] = node.DisplacementY;
                    }
                }
            }

            var stiffnessIndexes = new int[this._numElements][];
            var stiffnessMatrix = new float[this._numElements][];
            int lastProgressPercent = 0;

            for (var i = 0; i < this._numElements; i++ )
            {
                var localStiffness = new float[this._numElements];

                this.CalculateElementStiffness(i, ref localStiffness);

                this.CalculateLoads(i, ref localStiffness, isDisplaced, displacements, ref loads[i]);

                AddToStiffnessMatrix(localStiffness, out stiffnessIndexes[i], out stiffnessMatrix[i]);

                var progressPercent = (int)(100f * i / this._numElements);
                if (progressPercent > lastProgressPercent)
                {
                    lastProgressPercent = progressPercent;

                    progressReport.Report(new TaskProgress{Text = "Solving / Populating Stiffness ..." , ProgressPercentage = progressPercent});
                }
            }

            // solve for forces
            var forces = new float[this._numElements];
            this.SolveEquations(forces, loads, stiffnessMatrix, stiffnessIndexes);

            //modify the stiffness matrix to enforce the constraints
            counter = 0;
            foreach (var node in this._model.Nodes)
            {
                node.SetFreedom(forces[counter], forces[counter + 1]);
                counter += 2;
            }

            // solve the system of linear equations for the unknown displacements
            lastProgressPercent = 0;
            for (var i = 0; i < this._model.Elements.Count; i++)
            {
                var element = this._model.Elements[i];
                var localUnknowns = new float[DEGREES_OF_FREEDOM * 3];

                for (var j = 0; j < 3; j ++ )
                {
                    localUnknowns[j * DEGREES_OF_FREEDOM + 0] = forces[element.Connection[j] * DEGREES_OF_FREEDOM + 0];
                    localUnknowns[j * DEGREES_OF_FREEDOM + 1] = forces[element.Connection[j] * DEGREES_OF_FREEDOM + 1];
                }
                element.CalculateStress(localUnknowns);

                var progressPercent = (int)(100f * i / this._model.Elements.Count);
                if (progressPercent > lastProgressPercent)
                {
                    lastProgressPercent = progressPercent;

                    progressReport.Report(new TaskProgress { Text = "Solving / Stress recovery ...", ProgressPercentage = progressPercent });
                }
            }
        }

        private void SolveEquations(float[] forces, float[] loads, float[][] stiffnessValues, int[][] stiffnessIndexes)
        {
            var loads1 = new float[this._numElements];
            var loads2 = new float[this._numElements];
            var stresses = new float[this._numElements];
            var minLoad = this._minimumLoad;

            loads.CopyTo(loads1, 0);
            loads1.CopyTo(loads2, 0);

            var totalLoadsSquared = MultiplyArrays(loads1, loads1);
            var totalLoadsSquaredOriginal = totalLoadsSquared;

            var lastProgress = 0;
            for (var i = 0; i < this._numElements; i++ )
            {
                if (totalLoadsSquared <= minLoad * minLoad * totalLoadsSquaredOriginal)
                {
                    return;
                }

                this.CalculateStresses(loads2, ref stresses, stiffnessValues, stiffnessIndexes);
                var avgLoad = totalLoadsSquared / MultiplyArrays(loads2, stresses);
                var totalOriginalLoad = totalLoadsSquared;

                for (var j = 0; j < this._numElements; j++)
                {
                    forces[j] += avgLoad * loads2[j];
                    loads1[j] -= avgLoad * stresses[j];
                }

                totalLoadsSquared = MultiplyArrays(loads1, loads1);
                var newLoads = totalLoadsSquared / totalOriginalLoad;

                for (var j = 0; j < this._numElements; j++)
                {
                    loads2[j] = loads1[j] + newLoads * loads2[j];
                }

                var progress = (int)(100f * (totalLoadsSquaredOriginal - totalLoadsSquared) / (totalLoadsSquaredOriginal - minLoad * minLoad * totalLoadsSquaredOriginal));
                if (progress > lastProgress)
                {
                    lastProgress = progress;
                }
            }
        }

        private void CalculateElementStiffness(int index, ref float[] stiffness)
        {
            var num = index / DEGREES_OF_FREEDOM;
            const int NODE_COUNT = 3;
            for (var i = 0; i < this._model.Elements.Count; i++ )
            {
                var element = this._model.Elements[i];

                var flag = false;
                var connections = new int[NODE_COUNT];
                connections[0] = element.Connection[0];
                if (connections[0] == num)
                    flag = true;
                connections[1] = element.Connection[1];
                if (connections[1] == num)
                    flag = true;
                connections[2] = element.Connection[2];
                if (connections[2] == num)
                    flag = true;

                if (flag)
                {
                    // get the nodes at the corners of the element
                    var node1 = _model.Nodes[element.Connection[0]];
                    var node2 = _model.Nodes[element.Connection[1]];
                    var node3 = _model.Nodes[element.Connection[2]];
                    var x1 = node1.X;
                    var y1 = node1.Y;
                    var x2 = node2.X;
                    var y2 = node2.Y;
                    var x3 = node3.X;
                    var y3 = node3.Y;

                    element.CalcElemK(this._thickness, this._youngsModulus, this._poissonsRatio, x1, y1, x2, y2, x3,
                        y3);
                    for (var j = 0; j < NODE_COUNT; j++)
                    {
                        for (var k = 0; k < NODE_COUNT; k++)
                        {
                            for (var l = 0; l < DEGREES_OF_FREEDOM; l++)
                            {
                                for (var m = 0; m < DEGREES_OF_FREEDOM; m++)
                                {
                                    var a = connections[j] * DEGREES_OF_FREEDOM + l;
                                    var b = connections[k] * DEGREES_OF_FREEDOM + m;

                                    if (a == index)
                                        stiffness[b] += element.Stiffness[j * DEGREES_OF_FREEDOM + l,
                                            k * DEGREES_OF_FREEDOM + m];
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CalculateStresses(float[] loads, ref float[] stresses, float[][] stiffnessValues, int[][] stiffnessIndexes)
        {
            for (var i = 0; i < this._numElements; i++)
            {
                float stiffness = 0f;
                for (var j = 0; j < stiffnessValues[i].Length; j++)
                {
                    stiffness += stiffnessValues[i][j] * loads[stiffnessIndexes[i][j]];
                }
                stresses[i] = stiffness;
            }
        }

        private void CalculateLoads(int index, ref float[] stiffness, bool[] isDisplaced, float[] displacements, ref float loads)
        {
            var ind = index;

            if (!isDisplaced[ind])
            {
                for (var i = 0; i < this._numElements; i++)
                {
                    if (isDisplaced[i])
                    {
                        loads -= stiffness[i] * displacements[i];
                        stiffness[i] = 0f;
                    }
                }
            }
            else 
            {
                loads = stiffness[ind] * displacements[ind];

                for (var i = 0; i < this._numElements; i++ )
                {
                    if (i != ind)
                    {
                        stiffness[i] = 0f;
                    }
                }
            }
        }

        private static void AddToStiffnessMatrix(float[] data, out int[] indexes, out float[] values)
        {
            float TOLERANCE = 0.00001f;
            //triangleCount the items needed
            var count = 0;
            foreach (float t in data)
            {
                if (Math.Abs(t) > TOLERANCE)
                    count++;
            }

            // create the storage
            values = new float[count];
            indexes = new int[count];
            
            // set the values in the array
            count = 0;
            for (var i = 0; i < data.Length; i++ )
            {
                if (Math.Abs(data[i]) > TOLERANCE)
                {
                    values[count] = data[i];
                    indexes[count] = i;
                    count++;
                }
            }
        }

        private static float MultiplyArrays(float[] arr1, float[] arr2)
        {
            float scalar = 0f;
            for (var i = 0; i < arr1.Length; i++)
            {
                scalar += arr1[i] * arr2[i];
            }
            return scalar;
        }
    }
}
