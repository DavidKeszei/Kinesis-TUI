using Kinesis.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

public class ListOf: Entity {

    public List<Entity> Children {
        set {
            if (value == null || value.Count == 0)
                return;

            for (int i = 0; i < value.Count; ++i) {
                ConnectionComponent conn = new ConnectionComponent() {
                    Direction = ConnectionDir.DOWN,
                    Attached = value[i]
                };

                value[i].GetComponent<ConnectionComponent>()!.Attached = this;
                _ = base.AttachComponent<ConnectionComponent>(conn);
            }
        }
    }

    public ListOf()
        => _ = base.AttachComponent<ConnectionComponent>(new ConnectionComponent() { Direction = ConnectionDir.UP });
}
