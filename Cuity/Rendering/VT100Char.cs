global using vt_char = Cuity.Rendering.VT100Char;
using System.Runtime.InteropServices;

namespace Cuity.Rendering; 

/// <summary>
/// Represent a VT100 emulated character on the sceen.
/// </summary>
public struct VT100Char: IEquatable<VT100Char> {
    private char m_character = ' ';
    private RGB m_rgb = RGB.White;

    /// <summary>
    /// Character of the current buffer.
    /// </summary>
    public char Character { get => m_character; set => m_character = value; }

    /// <summary>
    /// Color of the current buffer.
    /// </summary>
    public RGB Color { get => m_rgb; set => m_rgb = value; }

    public static bool operator ==(VT100Char l, VT100Char r) => l.Equals(r);

    public static bool operator !=(VT100Char l, VT100Char r) => !l.Equals(r);

    public VT100Char(char character, RGB color) {
        m_character = character;
        m_rgb = color;
    }

    public readonly bool Equals(VT100Char vt) => vt.m_character == m_character && vt.Color.Equals(m_rgb);
}

/// <summary>
/// Represent a <see cref="RGB"/> color on the screen.
/// </summary>
[StructLayout(layoutKind: LayoutKind.Explicit)]
public struct RGB: IEquatable<RGB> {
    [FieldOffset(offset: 0)] private int m_color = 0x00;
    [FieldOffset(0)] private byte m_red = 0x0;

    [FieldOffset(1)] private byte m_green = 0x0;
    [FieldOffset(2)] private byte m_blue = 0x0;

    public static implicit operator RGB(int color) => new RGB(color);

    public static RGB White { get => new RGB(r: 255, g: 255, b: 255); }

    public byte R { readonly get => m_red; set => m_red = value; }

    public byte G { readonly get => m_green; set => m_green = value; }

    public byte B { readonly get => m_blue; set => m_blue = value; }
         
    public RGB(byte r, byte g, byte b) {
        m_red = r;
        m_green = g;
        m_blue = b;
    }

    public RGB(int color) => m_color = color;

    public bool Equals(RGB rgb) 
        => rgb.m_red == m_red && rgb.m_green == m_green && rgb.m_blue == m_blue;
}