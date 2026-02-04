global using vt_char = Cuity.Rendering.VT100Char;
using System.Runtime.InteropServices;

namespace Cuity.Rendering; 

/// <summary>
/// Represent a VT100 emulated character on the screen.
/// </summary>
public struct VT100Char: IEquatable<VT100Char> {
    private RGB m_bg = RGB.Black;
    private RGB m_fg = RGB.White;

    private char m_character = ' ';
    private VT100StyleFlag m_styles = VT100StyleFlag.NONE;

    /// <summary>
    /// Character of the current buffer.
    /// </summary>
    public char Character { get => m_character; set => m_character = value; }

    /// <summary>
    /// Background color of the current buffer piece.
    /// </summary>
    public RGB Background { readonly get => m_bg; set => m_bg = value; }

    /// <summary>
    /// Foreground color of the current buffer piece.
    /// </summary>
    public RGB Foreground { readonly get => m_fg; set => m_fg = value; }

    /// <summary>
    /// Applied font styles to the character.
    /// </summary>
    public VT100StyleFlag Styles { get => m_styles; set => m_styles = value; }

    public static bool operator ==(VT100Char l, VT100Char r) => l.Equals(r);

    public static bool operator !=(VT100Char l, VT100Char r) => !l.Equals(r);

    public VT100Char(char character, RGB color) {
        m_character = character;
        m_bg = color;
    }

    public readonly bool Equals(VT100Char vt) 
        => vt.m_character == m_character && vt.Background.Equals(m_bg) && 
           vt.Foreground.Equals(rgb: m_fg) && 
           vt.m_styles == m_styles;

    public void Clear() {
        m_bg = RGB.Black;
        m_fg = RGB.Black;

        m_styles = VT100StyleFlag.NONE;
        m_character = ' ';
    }
}

/// <summary>
/// Represent a <see cref="RGB"/> color on the screen.
/// </summary>
[StructLayout(layoutKind: LayoutKind.Explicit)]
public struct RGB: IEquatable<RGB> {
    [FieldOffset(offset: 0)] private uint m_color = 0x00;
    [FieldOffset(0)] private byte m_red = 0x0;

    [FieldOffset(1)] private byte m_green = 0x0;
    [FieldOffset(2)] private byte m_blue = 0x0;

    public static implicit operator RGB(uint color) => new RGB(color);

    public static RGB White { get => new RGB(r: 255, g: 255, b: 255); }

    public static RGB Black { get => new RGB(r: 0, g: 0, b: 0); }

    public static RGB Purple { get => new RGB(r: 128, g: 0, b: 128); }

    public byte R { readonly get => m_red; set => m_red = value; }

    public byte G { readonly get => m_green; set => m_green = value; }

    public byte B { readonly get => m_blue; set => m_blue = value; }
         
    public RGB(byte r, byte g, byte b) {
        m_red = r;
        m_green = g;
        m_blue = b;
    }

    public RGB(uint color) => m_color = color;

    public static RGB Random() => new RGB((uint)(System.Random.Shared.NextSingle() * 0xFFFFFFFF));

    public bool Equals(RGB rgb) 
        => rgb.m_red == m_red && rgb.m_green == m_green && rgb.m_blue == m_blue;
}