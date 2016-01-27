using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntryPoint
{
    interface IKDTree<T>
    {
        bool IsXSorted { get; }
        bool IsEmpty { get; }
        T Value { get; }
        IKDTree<T> Left { get; }
        IKDTree<T> Right { get; }

    }
}
