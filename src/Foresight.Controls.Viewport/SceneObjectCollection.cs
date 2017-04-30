using System;
using System.Collections;
using System.Collections.Generic;

namespace UI.Controls.Viewport
{
    public sealed class SceneObjectCollection : IEnumerable<SceneObject>
    {
        private readonly List<SceneObject> _entities;

        /// <summary>
        /// Constructor
        /// </summary>
        internal SceneObjectCollection()
        {
            _entities = new List<SceneObject>();
        }

        /// <summary>
        /// Returns the sb of entities in this collection
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _entities.Count; }
        }
        
        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<SceneObject> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        /// <summary>
        ///     return the enumerator for this collection
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        internal event EventHandler<EventArgs> ListChanged;

        /// <summary>
        /// Add the entity to this collection
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(SceneObject value)
        {
            value.Compile();

            _entities.Add(value);

            OnListChanged(new EventArgs());
        }

        /// <summary>
        ///     Clear the collection
        /// </summary>
        public void Clear()
        {
            foreach (var entity in this)
            {
                entity.Dispose();
            }
            _entities.Clear();

            OnListChanged(new EventArgs());
        }

        /// <summary>
        /// Compile the display lists for all the items in the collection
        /// </summary>
        public void Compile()
        {
            foreach (var current in this)
            {
                current.Compile();
            }
        }

        /// <summary>
        ///     Raises the ListChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnListChanged(EventArgs e)
        {
            ListChanged?.Invoke(this, e);
        }
    }
}