using Kinesis.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;

namespace Kinesis.UI.Components;

/// <summary>
/// Represent a style container component.
/// </summary>
public class Style: Component, IStaticType {
    private const string TYPE_NAME = "Style";
    private StyleGenericUnion m_union = default;

    /// <summary>
    /// Name of the <see cref="Style"/> component.
    /// </summary>
    public static string Name { get => TYPE_NAME; }

    /// <summary>
    /// Tagging of the <see cref="Style"/>, which indicates what kind of style property is.
    /// </summary>
    public StyleTag Tag { get => m_union.Tag; }

    /// <summary>
    /// Interact the underlying value as <see cref="int"/>.
    /// </summary>
    public int AsInt { get => m_union.GetInteger(); set => m_union.SetInteger(value); }

    /// <summary>
    /// Interact the underlying value as <see cref="RGB"/>.
    /// </summary>
    public RGB AsRGB { get => m_union.GetRGB(); set => m_union.SetRGB(value); }

    /// <summary>
    /// Interact the underlying value as <see cref="StyleFlag"/>.
    /// </summary>
    public StyleFlag AsAttribute { get => m_union.GetVT100Attr();  set => m_union.SetVT100Attr(value); }

    private Style(StyleTag tag, RGB color): base(id: ComponentRegistry.QueryComponent(TYPE_NAME)) {
        m_union = new StyleGenericUnion(tag);
        m_union.SetRGB(color);
    }

    private Style(StyleTag tag, int value): base(id: ComponentRegistry.QueryComponent(TYPE_NAME)) {
        m_union = new StyleGenericUnion(tag);
        m_union.SetInteger(value);
    }

    private Style(StyleTag tag, StyleFlag flag): base(id: ComponentRegistry.QueryComponent(TYPE_NAME)) {
        m_union = new StyleGenericUnion(tag);
        m_union.SetVT100Attr(flag);
    }

    /// <summary>
    /// Create a new <see cref="Style"/> with <see cref="RGB"/> value.
    /// </summary>
    /// <param name="tag">Tag of the style.</param>
    /// <param name="color">The color value itself.</param>
    /// <returns>Return a <see cref="Style"/> instance.</returns>
    public static Style CreateFromRGB(StyleTag tag, RGB color) => new Style(tag, color);

    /// <summary>
    /// Create a new <see cref="Style"/> with <see cref="RGB"/> value.
    /// </summary>
    /// <param name="tag">Tag of the style.</param>
    /// <param name="value">The color value itself.</param>
    /// <returns>Return a <see cref="Style"/> instance.</returns>
    public static Style CreateFromInt(StyleTag tag, int value) => new Style(tag, value);

    /// <summary>
    /// Create a new <see cref="Style"/> with <see cref="StyleFlag"/> value.
    /// </summary>
    /// <param name="tag">Tag of the style.</param>
    /// <param name="flag">The color value itself.</param>
    /// <returns>Return a <see cref="Style"/> instance.</returns>
    public static Style CreateFromAttributes(StyleTag tag, StyleFlag flag) => new Style(tag, flag);
}

/// <summary>
/// Simple union for store <see cref="Style"/> values without generic.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
internal struct StyleGenericUnion {
    [FieldOffset(0)] private int m_integer = 0;
    [FieldOffset(0)] private float m_floating = .0f;

    [FieldOffset(0)] private StyleFlag m_flag = StyleFlag.NONE;
    [FieldOffset(0)] private RGB m_color = RGB.Black;

    [FieldOffset(8)] private readonly StyleTag m_tag = StyleTag.BACKGROUND;

    /// <summary>
    /// Delimiter tag of the <see cref="StyleGenericUnion"/>.
    /// </summary>
    public readonly StyleTag Tag { get => m_tag; }

    /// <summary>
    /// Create a new <see cref="StyleGenericUnion"/>.
    /// </summary>
    /// <param name="tag">Tag of the <see cref="StyleGenericUnion"/> instance.</param>
    public StyleGenericUnion(StyleTag tag) => m_tag = tag;

    public void SetInteger(int value) {
        if (m_tag == StyleTag.BORDER_WIDTH)
            m_integer = value;
    }

    public void SetVT100Attr(StyleFlag value) {
        if (m_tag == StyleTag.FONT_ATTR)
            m_flag = value;
    }

    public void SetRGB(RGB value) {
        switch (m_tag) {
            case StyleTag.FOREGROUND:
            case StyleTag.BACKGROUND:
            case StyleTag.BORDER_COLOR:
                m_color = value;
                break;
        }
    }

    public int GetInteger() {
        if (m_tag == StyleTag.BORDER_WIDTH)
            return m_integer;

        return -1;
    }

    public StyleFlag GetVT100Attr() {
        return m_tag switch {
            StyleTag.FONT_ATTR => m_flag,
            _ => StyleFlag.NONE
        };
    }

    public RGB GetRGB() {
        return m_tag switch {
            StyleTag.FOREGROUND or StyleTag.BACKGROUND or StyleTag.BORDER_COLOR => m_color,
            _ => RGB.Black,
        };
    }
}

public enum StyleTag: byte {
    BACKGROUND,
    FOREGROUND,
    BORDER_WIDTH,
    BORDER_COLOR,
    MARGIN,
    PADDING,
    FONT_ATTR
}
