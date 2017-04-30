using System;

namespace UI.Controls.Viewport
{
    /// <summary>
    ///     The component axes of 3D
    /// </summary>
    [Flags]
    public enum Axes
    {
        /// <summary>
        ///     The x axe
        /// </summary>
        X = 1,

        /// <summary>
        ///     The y axe
        /// </summary>
        Y = 2,

        /// <summary>
        ///     The z axe
        /// </summary>
        Z = 4,
        
        /// <summary>
        ///     xyz combination
        /// </summary>
        Xyz = X | Y | Z
    }

    /// <summary>
    ///     Represents the different drawing modes supported in this viewport
    /// </summary>
    [Flags]
    public enum DrawingModes
    {
        /// <summary>
        ///     Wireframe, displays linear connections between vertices
        /// </summary>
        Wireframe = 1,

        /// <summary>
        ///     Shaded, Paints solid planes between vertices
        /// </summary>
        Shaded = 2,

        /// <summary>
        ///     Vertices, just the vertices
        /// </summary>
        Vertices = 4
    }

    /// <summary>
    ///     The action type to use when clicking in the viewport
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        ///     Do not perform an action
        /// </summary>
        None,

        /// <summary>
        ///     Zoom in or out when the mouse is dragged correspondingly up or down
        /// </summary>
        Zoom,

        /// <summary>
        ///     Pan the model when the mouse is dragged
        /// </summary>
        Pan,

        /// <summary>
        ///     Rotate the model when the mouse is dragged
        /// </summary>
        Rotate,

        /// <summary>
        ///     Zoom into the scene using a selection box
        /// </summary>
        ZoomWindow,
        
    }
}