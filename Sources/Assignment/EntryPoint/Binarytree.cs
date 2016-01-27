using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntryPoint
{
    class BinaryTree<T> : ITree<T>
    {

        public bool IsEmpty
        {
            get
            {
                return false;
            }

        }


        public ITree<T> Left { get; set; }

        public ITree<T> Right { get; set; }

        public T Value { get; set; }

        public BinaryTree(ITree<T> l, T v, ITree<T> r)
        {
            Value = v;
            Left = l;
            Right = r;
        }

        public T get_min()
        {
            if (Left.IsEmpty)
                return Value;

            return Left.get_min();
        }
    }

    class BinaryEmpty<T> : ITree<T>
    {
        public bool IsEmpty
        {
            get
            {
                return true;
            }
        }

        public ITree<T> Left
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITree<T> Right
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public T get_min()
        {
            throw new NotImplementedException();
        }

       
        public T Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ITree<T> ITree<T>.Left
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        ITree<T> ITree<T>.Right
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        T ITree<T>.Value
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
