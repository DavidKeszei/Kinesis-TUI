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
    }
}

public enum MoveAnchor: byte {
    ABSOLUTE,
    RELATIVE
}
