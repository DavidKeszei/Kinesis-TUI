using Cuity.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a style container component.
/// </summary>
public class Style<T>: IStyleComponent {
    private const string NAME_OF = "Style";

    private T m_value = default!;
    private StyleTag m_tagging = StyleTag.BACKGROUND;

    public string Name { get => NAME_OF; }

    /// <summary>
    /// Stored color of the <see cref="Style{T}"/> as <typeparamref name="T"/>.
    /// </summary>
    public T Value { get => m_value; set => m_value = value; }

    /// <summary>
    /// Tagging of the <see cref="Style{T}"/>, which indicates what kind of style property is.
    /// </summary>
    public StyleTag Tag { get => m_tagging; set => m_tagging = value; }

    public Style() { }

    public Style(StyleTag tag, T value) {
        m_value = value;
        m_tagging = tag;
    }
}

public enum StyleTag: byte {
    BACKGROUND,
    FOREGROUND,
    BORDER_WIDTH,
    BORDER_COLOR,
    FONT_ATTR
}
