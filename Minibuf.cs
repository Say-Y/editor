using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace editor
{
    enum MinibufMode
    {
        Run,
        Search,
        SearchAndReplace
    }
    class Minibuf
    {
        private string buf;
        
        public Minibuf()
        {

        }

        public void Clear()
        {
            buf = "";
        }

        public void Insert(string a)
        {
            buf += a;
        }

        public void Run()
        {
            switch (buf)
            {
                case "run":

                    break;
                case "search":

                    break;
                case "search-and-replace":

                    break;
            }
        }
    }
}
