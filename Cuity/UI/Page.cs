using Cuity.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Represent a UI page on the screen.
/// </summary>
public abstract class Page {
    /// <summary>
    /// Build the page as <see cref="Entity"/>.
    /// </summary>
    /// <returns>Return <see cref="Entity"/> instance from the current <see cref="Page"/>.</returns>
    public abstract Entity Build();

    /// <summary>
    /// Iterate through the current <see cref="Page"/> as flat struckture.
    /// </summary>
    /// <returns>Return as <see cref="IEnumerable{T}"/> instance.</returns>
    internal IEnumerable<Entity> Iterate() {
        Entity? root = Build();

        if (root == null) yield break;
        else yield return root;

        int rootChildCount = root.Count(static x => x.Name == nameof(Child));

        Child? child = root.GetComponent<Child>();
        Entity? current = null!;

        for (int i = 0; i < rootChildCount; ++i) {
            current = child!.Attached;
            yield return current;

            
        }
    }
}
