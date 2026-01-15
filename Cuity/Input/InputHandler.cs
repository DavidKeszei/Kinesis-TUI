using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Cuity.Input;

public readonly record struct InputMessage(InputKey Key, InputModifier Modifier, InputAction Action);

public class InputHandler {
    /// <summary>
    /// Threshold of the holding in ms. (200ms)
    /// </summary>
    private const int HOLD_TRESHOLD = 200;

    /// <summary>
    /// Minimum time, when we think no input was happened & fire. (50ms)
    /// </summary>
    private const int DEAD_SPACE = 50;

    /// <summary>
    /// Minimal window value between the "ghost" input and long-press input.
    /// </summary>
    private const int GHOST_WINDOW = 35;
     
    private (InputKey Key, InputModifier Modifier, TimeSpan When) m_startInputInfo = (InputKey.NONE, InputModifier.NONE, TimeSpan.Zero);
    private InputMessage m_currentMessage = default!;

    private bool m_isLongPress = false;

    public InputHandler() 
        => m_currentMessage = new InputMessage(InputKey.NONE, InputModifier.NONE, InputAction.PRESS);

    public void Start() {
        float remainedDeadSpace = DEAD_SPACE;

        while (true) {
            DateTime now = DateTime.UtcNow;
            int pressTime = (int)(now - m_startInputInfo.When).TimeOfDay.TotalMilliseconds;

            if (Console.KeyAvailable) {
                (InputKey key, InputModifier modifier) = Read(info: Console.ReadKey(true));

                if (key == InputKey.NONE) continue;

                if (m_startInputInfo.Key == key && m_startInputInfo.Modifier == modifier) {
                    /* 35ms window between ghost & long press */
                    Console.WriteLine((now.TimeOfDay - m_startInputInfo.When).TotalMilliseconds);

                    if(!m_isLongPress && pressTime >= HOLD_TRESHOLD) {

                        m_currentMessage = new InputMessage(Key: m_startInputInfo.Key,
                                                            Modifier: m_startInputInfo.Modifier,
                                                            Action: InputAction.HOLD);

                        m_isLongPress = true;
                        Console.WriteLine($"<INPUT> Key: {m_currentMessage.Key}, Time: {pressTime}ms, {m_isLongPress}");
                    }

                    remainedDeadSpace = DEAD_SPACE;
                }
                else if (m_startInputInfo.Key != key || m_startInputInfo.Modifier != modifier) {
                    if (m_startInputInfo.When != TimeSpan.Zero) {

                        m_currentMessage = new InputMessage(Key: m_startInputInfo.Key,
                                                            Modifier: m_startInputInfo.Modifier,
                                                            Action: InputAction.PRESS);

                        remainedDeadSpace = DEAD_SPACE;
                    }

                    m_startInputInfo = (key, modifier, now.TimeOfDay);
                }

                continue;
            }

            if (remainedDeadSpace <= 0) {
                if(m_currentMessage.Action != InputAction.HOLD && m_currentMessage.Key != m_startInputInfo.Key) {
                    m_currentMessage = new InputMessage(Key: m_startInputInfo.Key,
                                                        Modifier: m_startInputInfo.Modifier,
                                                        Action: InputAction.PRESS);

                    Console.WriteLine($"<INPUT> Key: {m_currentMessage.Key}, Time: {pressTime}ms");
                }

                m_startInputInfo = (InputKey.NONE, InputModifier.NONE, TimeSpan.Zero);
                m_currentMessage = new InputMessage(InputKey.NONE, InputModifier.NONE, InputAction.PRESS);

                m_isLongPress = false;
                remainedDeadSpace = DEAD_SPACE;

                continue;
            }

            if(m_startInputInfo.When != TimeSpan.Zero)
                remainedDeadSpace -= (float)(DateTime.UtcNow - now).TotalMilliseconds;
        }
    }

    private (InputKey, InputModifier) Read(ConsoleKeyInfo info) {
        InputKey key = info.Key switch {
            ConsoleKey.A => InputKey.A,
            ConsoleKey.S => InputKey.S,
            _ => InputKey.NONE
        };

        InputModifier modifier = info.Modifiers switch {
            ConsoleModifiers.Shift   => InputModifier.SHIFT,
            ConsoleModifiers.Alt     => InputModifier.ALT,

            ConsoleModifiers.Control => InputModifier.CTRL,
            _ => InputModifier.NONE
        };

        return (key, modifier);
    }
}
