using Cuity.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI.Components;

/// <summary>
/// Represent a component, which can drawing a box to the screen.
/// </summary>
public class BoxRenderer: RenderComponent {
    private const string NAME_OF = "BoxRenderer";

    /// <summary>
    /// Name of the <see cref="BoxRenderer"/>.
    /// </summary>
    public override string Name { get => NAME_OF; }

    /// <summary>
    /// Render a box to the specific <paramref name="buffer"/> area.
    /// </summary>
    /// <param name="buffer">Target buffer of the rendering. This can be smaller, than the requested scale.</param>
    internal override void Render(in Canvas buffer, int version, params IEnumerable<IStyleComponent> styles) {
        if(base.m_entityVersion != version) {
            m_entityVersion = version;
            CacheStyles(styles);
        }

        for (int x = 0; x < buffer.Scale.X; ++x) {

            for (int y = 0; y < buffer.Scale.Y; ++y) {
                ref vt_char ch = ref buffer[x, y];

                /* Source-like visual debug, if the background as visual effect not exists or just wrong type/typo. */
                if(!m_cache.TryGetValue(key: StyleTag.BACKGROUND, out IStyleComponent? bg) || bg is not Style<RGB>) {
                    if(y % 2 != 0) ch.Background = x % 2 != 0 ? RGB.Purple : new RGB(r: 0, g: 0, b: 0);
                    else ch.Background = x % 2 == 0 ? RGB.Purple : new RGB(r: 0, g: 0, b: 0);

                    ch.Character = ' ';
                }
                else {
                    ch.Background = (bg as Style<RGB>)!.Value;
                    ch.Character = ' ';
                }
            }
        }
    }

    protected override void CacheStyles(IEnumerable<IStyleComponent> styles) {
        m_cache.Clear();

        foreach(IStyleComponent style in styles) {
            if(style.Tag == StyleTag.BACKGROUND) {
                m_cache.Add(style.Tag, style);
                return;
            }
        }
    }
}
