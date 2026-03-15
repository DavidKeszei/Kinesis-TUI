using Kinesis.UI.Components;
using Kinesis.Rendering;

using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

public class Box: Entity {

    public Vec2 Scale { get => base.GetComponent<Transform>()!.Scale; set => base.GetComponent<Transform>()!.Scale = value; }

    public RGB Background { get => base.GetComponent<Style>()!.AsRGB; set => base.GetComponent<Style>()!.AsRGB = value; }

    public Entity Child { 
        set {
            if (value == null) return;

            _ = base.GetComponent<Hierarchy>(index: 1)!.Attached = value;
            _ = value.GetComponent<Hierarchy>(Hierarchy.Parent)!.Attached = this;
        } 
    }

    public Box() {
        _ = base.AttachComponent<Transform>(new Transform(), isUnique: true);
        _ = base.AttachComponent<BoxRenderer>(new BoxRenderer(), isUnique: true);

        _ = base.AttachComponent<RenderHierarchy>(new RenderHierarchy(), isUnique: true);
        _ = base.AttachComponent<Hierarchy>(new Hierarchy() { Direction = ConnectionDir.UP });

        _ = base.AttachComponent<Hierarchy>(new Hierarchy() { Direction = ConnectionDir.DOWN });
        _ = base.AttachComponent<Style>(Style.CreateFromRGB(StyleTag.BACKGROUND, RGB.White));
    }
}
