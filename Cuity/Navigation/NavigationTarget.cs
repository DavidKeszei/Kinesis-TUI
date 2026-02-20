using Cuity.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.Navigation;

/// <summary>
/// Represent a target for a navigation.
/// </summary>
/// <param name="Creation">Creation method for a <see cref="UI.Page"/>.</param>
/// <param name="Page">The page itself.</param>
internal record NavigationTarget(Func<ISystemProvider, Page> Creation, Page? Page);
