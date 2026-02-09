using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity;

public interface ICopyable<in TSelf> {

    public void Copy(TSelf other);
}
