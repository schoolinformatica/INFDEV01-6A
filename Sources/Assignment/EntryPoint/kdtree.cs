using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntryPoint
{
    

    class Empty<T> : IKDTree<T>
    {
        public bool IsEmpty
        {
            get
            {
                return true;
            }
        }

        public bool IsXSorted
        {
            get
            {
                return false;
            }
        }

        public IKDTree<T> Left
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IKDTree<T> Right
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public T Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    class Node<T> : IKDTree<T>
    {
        public bool IsEmpty
        {
            get
            {
                return false;
            }
      
        }

        public bool IsXSorted { get; set; }

        public IKDTree<T> Left { get; set; }

        public IKDTree<T> Right { get; set; }

        public T Value { get; set; }

        public Node(IKDTree<T> l, T v, IKDTree<T> r, bool x)
        {
            Value = v;
            Left = l;
            Right = r;
            IsXSorted = x;
        }
    }

}
