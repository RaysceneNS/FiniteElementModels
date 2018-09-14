namespace UI.Controls.Viewport
{
    public abstract class SceneObject 
    {
        internal abstract void ModelExtents(out float minX, out float maxX, out float minY, out float maxY);
        internal abstract void WindowExtents(out float minX, out float maxX, out float minY, out float maxY);
        internal abstract void Draw();
    }
}