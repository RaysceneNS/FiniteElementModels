using System;

namespace UI.Controls.Viewport
{
    [Flags]
    public enum Axes
    {
        X = 1,
        Y = 2,
        Z = 4,
        Xyz = X | Y | Z
    }

    [Flags]
    public enum DrawingModes
    {
        Wireframe = 1,
        Shaded = 2,
        Vertices = 4
    }

    public enum ActionType
    {
        None,
        Zoom,
        Pan,
        Rotate,
        ZoomWindow
    }

    public enum PlotMode
    {
        PerElement,
        PerNode
    }
}