﻿using System.Collections.Generic;
using ECS.Runtime.Access;
using ECS.Runtime.Access.Collector;
using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.RemoveListeners
{
    public class SelfRemovedEventSystem<T> : ReactiveSystem
    {
        private readonly List<ComponentRemoved<T>> _listenerBuffer = new List<ComponentRemoved<T>>();

        public SelfRemovedEventSystem(World world) : base(world)
        {
        }

        protected override Collector GetCollector(World world)
        {
            return world.Collector(Mask.With<T>().Removed());
        }

        protected override bool Filter(Entity entity)
        {
            return !entity.Has<T>() && entity.Has<RemovedListeners<T>>();
        }

        protected override void Execute(List<Entity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(entity.Get<RemovedListeners<T>>().Value);
                var value = entity.Get<T>();

                for (var k = 0; k < _listenerBuffer.Count; k++)
                {
                    _listenerBuffer[k](entity);
                }
            }
        }
    }
}