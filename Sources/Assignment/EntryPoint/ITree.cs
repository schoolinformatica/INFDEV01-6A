using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntryPoint
{
    interface ITree<T>
    {
        bool IsEmpty { get; }
        T Value { get; set; }
        ITree<T> Left { get; set; }
        ITree<T> Right { get; set; }
        T get_min();
    }
}
