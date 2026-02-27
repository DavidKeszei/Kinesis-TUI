using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Kinesis.Input;

/// <summary>
/// Represent a standard input on the Windows platform. 
/// </summary>
internal partial class WindowsInputBackend: IInputBackend {
    #region CONSTANTS

    private const string KERNEL32_LIB = "kernel32.dll";
    private const string USER32_LIB = "user32.dll";

    private const nint INVALID_HND = -1;
    private const uint MANUAL_PROCESSING = 0x0001;

    private const uint STD_IN = uint.MaxValue - 10 + 1;
    private const int MAX_CHAR = 1;

    private const string DEDICATED_THREAD_NAME = "<Thread> Input::Native";

    #endregion

    private Task m_rawInputTask = null!;
    private nint m_handle = nint.Zero;

    private (char Key, InputModifier Modifiers) m_message = ('\0', InputModifier.NONE);
    private int m_canRead = 0;

    public bool HasInput { get => IsKeyDown(character: (short)m_message.Key); }

    #region NATIVE_IMPL

    [LibraryImport(libraryName: KERNEL32_LIB, EntryPoint = "GetStdHandle")]
    private static partial nint GetStandardHandle(uint type);

    [LibraryImport(libraryName: KERNEL32_LIB, EntryPoint = "ReadConsoleW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [return: MarshalAs(unmanagedType: UnmanagedType.Bool)]
    private static partial bool ReadConsole(nint hnd, ref char buffer, uint length, out uint readCount, nint opt);

    [LibraryImport(libraryName: KERNEL32_LIB, EntryPoint = "SetConsoleMode")]
    [return: MarshalAs(unmanagedType: UnmanagedType.Bool)]
    private static partial bool SetMode(nint handle, uint flags);

    [LibraryImport(libraryName: USER32_LIB, EntryPoint = "GetAsyncKeyState")]
    private static partial short GetKeyState(int modifier);

    [LibraryImport(libraryName: USER32_LIB, EntryPoint = "VkKeyScanW")]
    private static partial short GetCode(int character);

    #endregion

    /// <summary>
    /// Create new <see cref="WindowsInputBackend"/> instance.
    /// </summary>
    /// <returns>Return a fresh <see cref="WindowsInputBackend"/> instance. If something goes wrong, then return <see cref="IInputBackend.ERR"/>.</returns>
    public static IInputBackend Init() {
        WindowsInputBackend backend = new WindowsInputBackend();
        backend.m_handle = GetStandardHandle(STD_IN);

        if(backend.m_handle == INVALID_HND) return IInputBackend.ERR;
        if(!SetMode(handle: backend.m_handle, flags: MANUAL_PROCESSING))
            return IInputBackend.ERR;

        backend.m_rawInputTask = Task.Run(() => {
            Thread.CurrentThread.Name = DEDICATED_THREAD_NAME;

            while(true) {
                if(backend.Read(out char character, out InputModifier modifiers)) {
                    backend.m_message = (character, modifiers);
                    Interlocked.Increment(ref backend.m_canRead);
                }
            }
        });

        return backend;
    }

    public (char Key, InputModifier Modifiers) ReadInput() {
        if(m_canRead == 0)
            return ('\0', InputModifier.NONE);

        Interlocked.Decrement(ref m_canRead);
        return m_message;
    }

    /// <summary>
    /// Read one key from the console input.
    /// </summary>
    /// <returns>If anything in the input, return <see langword="true"/>. Otherwise return <see langword="false"/>.</returns>
    private bool Read(out char character, out InputModifier modifiers) {
        Span<int> modifiersCodes = stackalloc int[5] {
            0xA0,
            0xA1,
            0xA2,
            0xA3,
            0x12,
        };

        character = '\0';
        modifiers = InputModifier.NONE;

        bool success = ReadConsole(hnd: m_handle, buffer: ref character, length: MAX_CHAR, out uint _, opt: nint.Zero);
        for(byte i = 0; i < modifiersCodes.Length; ++i) {

            if(GetKeyState(modifiersCodes[i]) < 0)
                modifiers |= (InputModifier)modifiersCodes[i];
        }

        return success;
    }

    private bool IsKeyDown(short character) {
        short code = GetCode(character);

        /* Clean up the flag bits (SHIFT, CTRL, ALT, RESERVED)*/
        code &= ~(1 << 8);
        code &= ~(1 << 9);
        code &= ~(1 << 10);
        code &= ~(1 << 11);

        return BitConverter.IsLittleEndian ? (GetKeyState(code) & (1 << 15)) != 0 : (GetKeyState(code) & (1 << 0)) != 0;
    }
}