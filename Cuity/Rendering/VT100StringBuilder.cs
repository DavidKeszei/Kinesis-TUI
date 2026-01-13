using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Cuity.Rendering;

/// <summary>
/// Helper structure for building VT100 strings without any heap-allocation.
/// </summary>
internal ref struct VT100StringBuilder {
    private const string ESC = "\x1b[";
    private const string ESC_CLEAR = "\x1b[0m";

    private const string ESC_BG = "\x1b[48;2;";
    private const string ESC_FG = "\x1b[38;2;";

    /// <summary>
    /// Maximum length of the command, which contains all of the commands.
    /// </summary>
    public const int MAX_COMMAND_LEN = 128;

    private readonly Span<char> m_stack = default!;
    private int m_position = 0;

    public VT100StringBuilder(Span<char> buffer) => m_stack = buffer;

    /// <summary>
    /// Write the position to the screen.
    /// </summary>
    /// <param name="x">X axis value of the position.</param>
    /// <param name="y">Y axis value of the position.</param>
    /// <returns>Return the current <see cref="VT100StringBuilder"/> instance.</returns>
    public VT100StringBuilder WritePosition(int x, int y) {
        ++x;
        ++y;

        m_stack[m_position++] = '\x1b';
        m_stack[m_position++] = '[';

        _ = y.TryFormat(m_stack[m_position..], out int written);
        m_position += written;

        m_stack[m_position++] = ';';

        _ = x.TryFormat(m_stack[m_position..], out written);
        m_position += written;

        m_stack[m_position++] = 'f';
        return this;
    }

    /// <summary>
    /// Write VT100 color to the screen.
    /// </summary>
    /// <param name="color">The color itself.</param>
    /// <param name="isBackground">The color is background color or not?</param>
    /// <returns>Return the current <see cref="VT100StringBuilder"/> instance.</returns>
    public VT100StringBuilder WriteColor(RGB color, bool isBackground) {
        m_stack[m_position++] = '\x1b';
        (isBackground ? ESC_BG : ESC_FG).CopyTo(m_stack[m_position..]);

        m_position += (isBackground ? ESC_BG : ESC_FG).Length;

        color.R.TryFormat(m_stack[m_position..], out int written);
        m_position += written;
        m_stack[m_position++] = ';';

        color.G.TryFormat(m_stack[m_position..], out written);
        m_position += written;
        m_stack[m_position++] = ';';

        color.B.TryFormat(m_stack[m_position..], out written);
        m_position += written;
        m_stack[m_position++] = 'm';

        return this;
    }

    public VT100StringBuilder WriteFontStyles(VT100StyleFlag flags) {
        if(flags == VT100StyleFlag.NONE || flags == 0) return this;

        /* Static stack allocated flags, which give us information about the supported flags. */
        Span<VT100StyleFlag> supportedFlags = stackalloc VT100StyleFlag[] {
            VT100StyleFlag.BOLD, VT100StyleFlag.ITALIC, VT100StyleFlag.UNDERLINE,
            VT100StyleFlag.BLINK_SLOW, VT100StyleFlag.BLINK_FAST, VT100StyleFlag.INVERSE,

            VT100StyleFlag.HIDDEN, VT100StyleFlag.STROKE_THROUGH, VT100StyleFlag.DOUBLE_UNDERLINE,
            VT100StyleFlag.OVERLINE
        };

        ESC.CopyTo(destination: m_stack[m_position..]);
        m_position += ESC.Length;

        foreach(VT100StyleFlag flag in supportedFlags) {
            if((flags & flag) == flag) {
                int code = flag switch {
                    VT100StyleFlag.BOLD => 1,
                    VT100StyleFlag.ITALIC => 3,

                    VT100StyleFlag.UNDERLINE => 4,
                    VT100StyleFlag.BLINK_SLOW => 5,

                    VT100StyleFlag.BLINK_FAST => 6,
                    VT100StyleFlag.INVERSE => 7,

                    VT100StyleFlag.HIDDEN => 8,
                    VT100StyleFlag.STROKE_THROUGH => 9,

                    VT100StyleFlag.DOUBLE_UNDERLINE => 21,
                    VT100StyleFlag.OVERLINE => 53,

                    _ => 0
                };

                flags &= ~(flag);

                code.TryFormat(destination: m_stack[m_position..], out int written);
                m_position += written;

                m_stack[m_position++] = ';';
            }
        }

        m_stack[m_position - 1] = 'm';
        return this;
    }

    /// <summary>
    /// Add a character to the screen.
    /// </summary>
    /// <param name="value">Value of the character.</param>
    /// <returns>Return the current <see cref="VT100StringBuilder"/> instance.</returns>
    public VT100StringBuilder WriteCharacter(char value) {
        m_stack[m_position++] = value;
        return this;
    }

    /// <summary>
    /// Build the underlying buffer and send to the <paramref name="destination"/>.
    /// </summary>
    /// <param name="destination">Destination of console screen.</param>
    /// <returns>Return the command length as <see cref="int"/>.</returns>
    public readonly int Build(StreamWriter destination) {
        for(int i = 0; i < m_position; ++i)
            destination.Write(value: m_stack[i]);

        destination.Write(ESC_CLEAR);
        return m_position + ESC_CLEAR.Length;
    }

    public void Clear() {
        m_position = 0;
    }
}
