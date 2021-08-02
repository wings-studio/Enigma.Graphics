using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Enigma.Graphics
{
    public abstract class Renderer : IDisposable, IDictionary<string, Scene>
    {
        public virtual bool IsRunning { get; set; } = false;

        protected readonly Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();

        public GraphicsAPI GraphicsAPI { get => GraphicsDevice.GraphicsAPI; set => GraphicsDevice.GraphicsAPI = value; }
        public Vortice.Mathematics.Color4 ClearColor 
        { get => GraphicsDevice.ColorForClear; set => GraphicsDevice.ColorForClear = value; }

        public IGraphicsDevice GraphicsDevice { get; protected set; }

        public virtual void Dispose() => GraphicsDevice.Dispose();

        public virtual void Render()
        {
            IsRunning = true;
            CreateResources();
            while (IsRunning)
            {
                BeginFrame();
                RenderFrame();
                EndFrame();
            }
        }

        public virtual void CreateResources()
        {
            foreach (Scene scene in scenes.Values)
                scene.CreateResources(GraphicsDevice);
        }

        protected virtual void BeginFrame() => GraphicsDevice.Begin();
        protected virtual void RenderFrame()
        {
            foreach (Scene scene in scenes.Values)
                scene.Render(GraphicsDevice);
        }
        protected virtual void EndFrame() => GraphicsDevice.End();

        #region IDictionary methods
        public Scene this[string key] { get => ((IDictionary<string, Scene>)scenes)[key]; set => ((IDictionary<string, Scene>)scenes)[key] = value; }

        public ICollection<string> Keys => ((IDictionary<string, Scene>)scenes).Keys;

        public ICollection<Scene> Values => ((IDictionary<string, Scene>)scenes).Values;

        public int Count => ((ICollection<KeyValuePair<string, Scene>>)scenes).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, Scene>>)scenes).IsReadOnly;

        public void Add(string key, Scene value)
        {
            ((IDictionary<string, Scene>)scenes).Add(key, value);
        }

        public void Add(KeyValuePair<string, Scene> item)
        {
            ((ICollection<KeyValuePair<string, Scene>>)scenes).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<string, Scene>>)scenes).Clear();
        }

        public bool Contains(KeyValuePair<string, Scene> item)
        {
            return ((ICollection<KeyValuePair<string, Scene>>)scenes).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, Scene>)scenes).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, Scene>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, Scene>>)scenes).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, Scene>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Scene>>)scenes).GetEnumerator();
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, Scene>)scenes).Remove(key);
        }

        public bool Remove(KeyValuePair<string, Scene> item)
        {
            return ((ICollection<KeyValuePair<string, Scene>>)scenes).Remove(item);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out Scene value)
        {
            return ((IDictionary<string, Scene>)scenes).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)scenes).GetEnumerator();
        }
        #endregion
    }
}
