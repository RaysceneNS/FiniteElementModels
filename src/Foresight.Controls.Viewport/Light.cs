using System;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    internal class Light
    {
        private readonly float _ambientLevel;
        private readonly float _diffuseLevel;
        private readonly int _lightIndex;
        private readonly float _specularLevel;
        private Point3 _position;

        /// <summary>
        ///     Create a neutral colored light source i.e. (R=G=B)
        /// </summary>
        internal Light(int lightIndex, float diffuse, float specular, float ambient, Point3 position)
        {
            if (diffuse < 0.0f || diffuse > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(diffuse), diffuse, "Diffuse lighting level must be in the range of 0-1");
            if (specular < 0.0f || specular > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(specular), specular, "Specular lighting level must be in the range of 0-1");
            if (ambient < 0.0f || ambient > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(ambient), ambient, "Ambient lighting level must be in the range of 0-1");

            //set the light properties
            _diffuseLevel = diffuse;
            _ambientLevel = ambient;
            _specularLevel = specular;
            _lightIndex = lightIndex;
            _position = position;
        }

        public void SwitchOn()
        {
            Gl.glEnable(_lightIndex);
            Gl.glLightfv(_lightIndex, Gl.GL_POSITION, new[] {_position.X, _position.Z, -_position.Y, 1f});
            Gl.glLightfv(_lightIndex, Gl.GL_AMBIENT, new[] {_ambientLevel, _ambientLevel, _ambientLevel, 1f});
            Gl.glLightfv(_lightIndex, Gl.GL_DIFFUSE, new[] {_diffuseLevel, _diffuseLevel, _diffuseLevel, 1f});
            Gl.glLightfv(_lightIndex, Gl.GL_SPECULAR, new[] {_specularLevel, _specularLevel, _specularLevel, 1f});
        }

        public void SwitchOff()
        {
            Gl.glDisable(_lightIndex);
        }
    }
}