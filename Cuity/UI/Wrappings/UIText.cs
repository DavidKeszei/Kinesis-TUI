using Cuity.Rendering;
using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Represent a simple text on the screen.
/// </summary>
public class UIText: Entity {

    /// <summary>
    /// Underlying text value of the <see cref="UIText"/>.
    /// </summary>
    public string Text {
        get {
            return base.GetComponent<TextRenderer>()!.Value;
        }
        set {
            if (string.IsNullOrEmpty(value))
                return;

            base.GetComponent<TextRenderer>()!.Value = value;
            base.GetComponent<Transform>()!.Scale = (X: value.Length, Y: 1);
        }
    }

    /// <summary>
    /// Background of the <see cref="UIText"/>.
    /// </summary>
    public RGB Background { get => base.GetComponent<Style<RGB>>(index: 0)!.Value; set => base.GetComponent<Style<RGB>>(index: 0)!.Value = value; }

    /// <summary>
    /// Foreground/Text color of the <see cref="UIText"/>.
    /// </summary>
    public RGB Foreground { get => base.GetComponent<Style<RGB>>(index: 1)!.Value; set => base.GetComponent<Style<RGB>>(index: 1)!.Value = value; }

    /// <summary>
    /// Style indicators of the <see cref="UIText"/>.
    /// </summary>
    public VT100StyleFlag Attributes { get => base.GetComponent<Style<VT100StyleFlag>>()!.Value; set => base.GetComponent<Style<VT100StyleFlag>>()!.Value = value; }

    public UIText() {
        base.AttachComponent<Transform>(new Transform() { Scale = (X: 0, Y: 1) }, isUnique: true);
        base.AttachComponent<TextRenderer>(component: new TextRenderer(), isUnique: true);

        base.AttachComponent<Style<RGB>>(component: new Style<RGB>(tag: StyleTag.BACKGROUND, value: RGB.Black));
        base.AttachComponent<Style<RGB>>(component: new Style<RGB>(tag: StyleTag.FOREGROUND, value: RGB.White));

        base.AttachComponent<Style<VT100StyleFlag>>(component: new Style<VT100StyleFlag>(tag: StyleTag.FONT_ATTR, value: VT100StyleFlag.NONE), isUnique: true);
    }
}
