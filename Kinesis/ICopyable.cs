using System;
using System.Collections.Generic;
using System.Text;

namespace Kinesis;

public interface ICopyable<T> {

    public void CopyFrom(T from);
}
