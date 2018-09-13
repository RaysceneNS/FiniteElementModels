using System;

namespace UI.Controls.Viewport
{
    [Flags]
    public enum DrawingModes
    {
        WireFrame = 1,
        Shaded = 2,
        Vertices = 4
    }

    public enum ActionType
    {
        None,
        Zoom,
        Pan,
    }
}