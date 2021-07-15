using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Graphics
{
    public abstract class Scene
    {
        protected readonly List<IRenderTask> renderTasks = new List<IRenderTask>();
        protected readonly List<IRenderTask> disabledtasks = new List<IRenderTask>();

        public virtual void AddRenderTask<T>(T renderTask) where T : IRenderTask => renderTasks.Add(renderTask);
        public void AddRenderTask<T>() where T : IRenderTask, new() => renderTasks.Add(new T());
        public virtual void DisableRenderTasks<T>() where T : IRenderTask 
            => disabledtasks.AddRange(renderTasks.Where((task) => task is T));

        public virtual void CreateResources(IGraphicsDevice graphicsDevice)
        {
            foreach (IRenderTask task in renderTasks)
                task.CreateResources(graphicsDevice, this);
        }

        public virtual void Render(IGraphicsDevice gd)
        {
            foreach (IRenderTask task in renderTasks)
            {
                if (!disabledtasks.Contains(task))
                    task.Render(gd, this);
            }
        }
    }
}
