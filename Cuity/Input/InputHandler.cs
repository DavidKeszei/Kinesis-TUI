using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cuity.Input;

public readonly record struct InputMessage(InputKey Key, InputModifier Modifier, InputActionType Action);

public class InputHandler {
    /// <summary>
    /// Treshold of the holding in ms. (350ms)
    /// </summary>
    private const int HOLD_TRESHOLD = 250;

    /// <summary>
    /// Minimum time, when we think no input was happened. (150ms)
    /// </summary>
    private const int DEAD_SPACE = 100;
     
    private (InputKey Key, InputModifier Modifier, TimeSpan When) m_startInputInfo = (InputKey.NONE, InputModifier.NONE, TimeSpan.Zero);
    private InputMessage m_lastMessage = default!;

    private InputMessage m_currentMessage = default!;

    public InputHandler() 
        => m_currentMessage = m_lastMessage = new InputMessage(InputKey.NONE, InputModifier.NONE, InputActionType.PRESS);

    public void Start() {
        float remainedDeadSpace = DEAD_SPACE;

        while (true) {
            DateTime now = DateTime.UtcNow;

            if (Console.KeyAvailable) {
                (InputKey key, InputModifier modifier) = Read(info: Console.ReadKey(true));

                if (key == InputKey.NONE) continue;

                if (m_startInputInfo.Key == key && m_startInputInfo.Modifier == modifier) remainedDeadSpace = DEAD_SPACE;
                else if (m_startInputInfo.Key != key || m_startInputInfo.Modifier != modifier) {

                    if (m_startInputInfo.When != TimeSpan.Zero) {

#if DEBUG
                        Console.WriteLine($"Fast Press time: {(now.TimeOfDay - m_startInputInfo.When).TotalMilliseconds:f0}ms");
#endif
                        m_currentMessage = new InputMessage(Key: m_startInputInfo.Key,
                                                            Modifier: m_startInputInfo.Modifier,
                                                            Action: (now.TimeOfDay - m_startInputInfo.When).TotalMilliseconds >= HOLD_TRESHOLD ? InputActionType.HOLD : InputActionType.PRESS);

                        remainedDeadSpace = DEAD_SPACE;
                    }

                    m_startInputInfo = (key, modifier, now.TimeOfDay);
                }

                continue;
            }

            if (remainedDeadSpace <= 0) {
                m_currentMessage = new InputMessage(Key: m_startInputInfo.Key,
                                                    Modifier: m_startInputInfo.Modifier,
                                                    Action: (now.TimeOfDay - m_startInputInfo.When).TotalMilliseconds >= HOLD_TRESHOLD ? InputActionType.HOLD : InputActionType.PRESS);

#if DEBUG
                Console.WriteLine($"Press time: {(now.TimeOfDay - m_startInputInfo.When).TotalMilliseconds:f0}ms");
#endif

                m_startInputInfo = (InputKey.NONE, InputModifier.NONE, TimeSpan.Zero);
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
