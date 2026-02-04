using Cuity.Rendering;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a text renderer component.
/// </summary>
public class TextRenderer: RenderComponent {
    private const string NAME_OF = "TextRenderer";

    private char[] m_buffer = null!;
    private int m_len = 0;

    /// <summary>
    /// Name of the <see cref="TextRenderer"/>.
    /// </summary>
    public override string Name { get => NAME_OF; }

    /// <summary>
    /// Current text text of the <see cref="TextRenderer"/>.
    /// </summary>
    public string Value { 
        get => new string(value: m_buffer.AsSpan()[..m_len]);
        set => SetText(text: value);
    }

    public TextRenderer() { }

    public TextRenderer(string text) {
        m_buffer = text.ToCharArray();
        m_len = text.Length;
    }

    internal override void Render(in Canvas buffer, int version, params IEnumerable<IStyleComponent> styles) {
        if (buffer.Scale.Y == 0 || buffer.Scale.X == 0)
            return;

        if(m_entityVersion != version)
            CacheStyles(styles);

        IStyleComponent? bg = null!;
        IStyleComponent? fg = null!;
        IStyleComponent? attr = null!;

        bool isMissing = (!m_cache.TryGetValue(key: StyleTag.BACKGROUND, out bg) && bg is not Style<RGB>) ||
                         (!m_cache.TryGetValue(key: StyleTag.FOREGROUND, out fg) && fg is not Style<RGB>);

        _ = m_cache.TryGetValue(key: StyleTag.FONT_ATTR, out attr);
        (int X, int Y) requiredScale = (X: m_len / buffer.Scale.Y, Y: m_len % buffer.Scale.Y);

        for(int x = 0; x < buffer.Scale.X && x <= requiredScale.X; ++x) {
            for(int y = 0; y < buffer.Scale.Y && y <= requiredScale.Y; ++y) {

                ref vt_char ch = ref buffer[x, y];

                if(isMissing) {
                    if(y % 2 == 0) ch.Background = x % 2 == 0 ? RGB.Purple : RGB.Black;
                    else ch.Background = x % 2 != 0 ? RGB.Black : RGB.Purple;

                    ch.Character = ' ';
                }
                else {
                    ch.Character = m_buffer[x];
                    ch.Background = (bg as Style<RGB>)!.Value;

                    ch.Foreground = (fg as Style<RGB>)!.Value;
                    ch.Styles = (attr as Style<VT100StyleFlag> ?? new Style<VT100StyleFlag>(tag: StyleTag.FONT_ATTR, value: VT100StyleFlag.NONE)).Value;
                }
            }
        }

        m_entityVersion = version;
    }

    protected override void CacheStyles(IEnumerable<IStyleComponent> styles) {
        m_cache.Clear();
        foreach(IStyleComponent style in styles) {
            
            switch(style.Tag) {
                case StyleTag.FOREGROUND:
                    m_cache.Add(StyleTag.FOREGROUND, style);
                    break;

                case StyleTag.BACKGROUND:
                    m_cache.Add(StyleTag.BACKGROUND, style);
                    break;

                case StyleTag.FONT_ATTR:
                    m_cache.Add(StyleTag.FONT_ATTR, style);
                    break;
            }
        }
    }

    private void SetText(ReadOnlySpan<char> text) {
        if (m_len < text.Length) {
            m_len = text.Length;
            m_buffer = new char[m_len];
        }

        for (int i = 0; i < m_len; ++i) {
            if (text.Length <= i) break;
            else m_buffer[i] = text[i];
        }
    } 
}
