using Kinesis.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis.UI;

/// <summary>
/// Provides style tagging property for <see cref="IComponent"/>s.
/// </summary>
public interface IStyleComponent: IComponent {

    /// <summary>
    /// Tagging of the <see cref="Style{T}"/>, which indicates what kind of style property is.
    /// </summary>
    public StyleTag Tag { get; }
}
