using System;
using System.Collections;
using System.Collections.Generic;

namespace UI.Controls.Viewport
{
    public sealed class SceneObjectCollection : IEnumerable<SceneObject>
    {
        private readonly List<SceneObject> _entities;

        internal SceneObjectCollection()
        {
            _entities = new List<SceneObject>();
        }

        public int Count
        {
            get { return _entities.Count; }
        }
        
        public IEnumerator<SceneObject> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        internal event EventHandler<EventArgs> ListChanged;

        public void Add(SceneObject value)
        {
            value.Compile();

            _entities.Add(value);

            OnListChanged(new EventArgs());
        }

        public void Clear()
        {
            _entities.Clear();

            OnListChanged(new EventArgs());
        }

        public void Compile()
        {
            foreach (var current in this)
            {
                current.Compile();
            }
        }

        private void OnListChanged(EventArgs e)
        {
            ListChanged?.Invoke(this, e);
        }
    }
}