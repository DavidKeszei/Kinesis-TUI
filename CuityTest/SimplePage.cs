using Cuity.Input;
using Cuity.Rendering;
using Cuity.UI;
using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace CuityTest;

internal class SimplePage: Page {
    private int m_count = 0;

    protected override Entity? Build() {
        return new InputDetector<UIText>() {
            OnInput = (message, text) => {
                ++m_count;

                text.Text = $"Counter: {m_count}";
                text.Foreground = RGB.Random();
            },
            Child = new UIText() {
                Text = $"Counter: {m_count}",
                Background = RGB.Black,

                Foreground = RGB.White,
                Attributes = VT100StyleFlag.NONE
            }
        };
    }
}