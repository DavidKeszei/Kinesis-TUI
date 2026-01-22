using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Cuity.Input;

/// <summary>
/// Single input message from the standard input stream.
/// </summary>
/// <param name="Key">Actual key value as <see cref="char"/>.</param>
/// <param name="Modifier">Currently pressed modifiers with the <see cref="Key"/>.</param>
/// <param name="Action">Current action of the message.</param>
public readonly record struct InputMessage(char Key, InputModifier Modifier, InputAction Action);

/// <summary>
/// Represent a unified source of the inputs.
/// </summary>
public class InputHandler {
    /// <summary>
    /// Indicates the wait time between two sampling. (5ms)
    /// </summary>
    private const int POOLING_TIME = 5;

    /// <summary>
    /// Minimum time, when we think no input was happened and we fire that. (10ms)
    /// </summary>
    private const int DEAD_ZONE = 10;

    /// <summary>
    /// Minimum time, when we think the press is long-press. (200ms)
    /// </summary>
    private const int HOLD_THRESHHOLD = 75;

    private readonly IInputBackend m_backend = null!;
    private (char Key, InputModifier Modifier, TimeSpan When) m_startInputInfo = ('\0', InputModifier.NONE, TimeSpan.Zero);

    public InputHandler() 
        => m_backend = RuntimeInformation.IsOSPlatform(osPlatform: OSPlatform.Windows) ? WindowsInputBackend.Init() : null!;

    /// <summary>
    /// Listen inputs from standard input.
    /// </summary>
    public void Listen() {
        float deadZoneTime = DEAD_ZONE;

        InputMessage message = default;
        InputAction lastAction = InputAction.PRESS;

        while (true) {
            DateTime now = DateTime.UtcNow;

            if (m_backend.HasInput) {
                (char character, InputModifier modifiers) = m_backend.ReadInput();

                if (character == '\0') {
                    Thread.Sleep(millisecondsTimeout: POOLING_TIME);
                    continue;
                }

                if (m_startInputInfo.Key == character && m_startInputInfo.Modifier == modifiers) {
                    if ((now.TimeOfDay - m_startInputInfo.When).TotalMilliseconds >= HOLD_THRESHHOLD && lastAction != InputAction.HOLD) {

                        lastAction = InputAction.HOLD;
                        message = new InputMessage(Key: m_startInputInfo.Key,
                                                   Modifier: m_startInputInfo.Modifier,
                                                   Action: InputAction.HOLD);
                    }
                }
                else if (m_startInputInfo.Key != character || m_startInputInfo.Modifier != modifiers) {
                    if (m_startInputInfo.When != TimeSpan.Zero) {

                        message = new InputMessage(Key: m_startInputInfo.Key,
                                                   Modifier: m_startInputInfo.Modifier,
                                                   Action: InputAction.PRESS);

                        deadZoneTime = DEAD_ZONE;
                    }

                    m_startInputInfo = (character, modifiers, now.TimeOfDay);
                }

                continue;
            }

            Thread.Sleep(millisecondsTimeout: POOLING_TIME);

            if (deadZoneTime <= 0) {
                if (lastAction != InputAction.HOLD) {
                    message = new InputMessage(Key: m_startInputInfo.Key,
                                                Modifier: m_startInputInfo.Modifier,
                                                Action: InputAction.PRESS);
                }

                m_startInputInfo = ('\0', InputModifier.NONE, TimeSpan.Zero);

                deadZoneTime = DEAD_ZONE;
                lastAction = InputAction.PRESS;
                continue;
            }

            if (m_startInputInfo.When != TimeSpan.Zero) {
                float inputTime = (float)(DateTime.UtcNow - now).TotalMilliseconds;
                deadZoneTime -= inputTime;
            }
        }

    }
}
