using Kinesis.Rendering;
using Kinesis.UI;
using Kinesis.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis;

public static class EntityExtension {
    extension(Entity entity) {

        /// <summary>
        /// Move the current <see cref="Entity"/> with <paramref name="x"/> and <paramref name="y"/> by the <paramref name="anchor"/>.
        /// </summary>
        /// <param name="x">Move adjustment on the X axis.</param>
        /// <param name="y">Move adjustment on the Y axis.</param>
        /// <param name="anchor">Pivot/Anchor of the movement.</param>
        public void Move(float x, float y, MoveAnchor anchor = MoveAnchor.ABSOLUTE) {
            Transform? transform = entity?.GetComponent<Transform>();
            if (transform == null) return;

            transform.Position = anchor switch {
                MoveAnchor.RELATIVE => new Vec2(x: transform.Position.X + x, y: transform.Position.Y + y),
                MoveAnchor.ABSOLUTE => new Vec2(x, y),
                _ => transform.Position
            };
        }

        public int CountComponent<T>(Func<T, bool>? comparand = null) where T: Component, IStaticType {
            int count = 0;
            comparand ??= static(_) => true;

            foreach (Component comp in entity)
                if (comp.TypeOf(T.Name) && comparand((T)comp))
                    ++count;

            return count;
        }

        public int LastChildrenPosition(Func<Entity, bool>? where = null) {
            int childCount = CountComponent<Hierarchy>(entity);
            if (where == null) return childCount;

            int pos = -1;
            for (int i = 1; i < childCount; ++i) {
                Entity? child = entity.GetComponent<Hierarchy>(i)!.Attached;

                if (child != null && where(child)) {
                    pos = i;
                }
            }

            return pos;
        }
    }
}

public enum MoveAnchor: byte {
    ABSOLUTE,
    RELATIVE
}
