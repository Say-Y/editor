//-------+----------------------------------------
//author : 배준현
//email  : outersky@gmail.com
//date   : 2017.11.16
//modify : 2017.11.16
//at     : kumoh national institude of technology
//-------+----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace editor
{
    public struct LC
    {
       public int L;
        public int C;

        public LC(int L, int C) : this()
        {
            this.L = L;
            this.C = C;
        }
    }
    enum WordType
    {
        comment = 0,
        normal = 1,
        instruction = 2,
        label = 3,
        space = 4
    }

    class Text
    {
        private List<string> Sentences;
        public List<int> fail;

        public void GetFail(string searchText)
        {
            for (int i = 0; i < searchText.Length; i++) {
                fail.Add(0);
             }
            for (int i = 1, j = 0; i < searchText.Length; i++)
            {
                while (j > 0 && searchText[i] != searchText[j]) j = fail[j - 1];
                if (searchText[i] == searchText[j]) fail[i] = ++j;
            }
        }
       
        public List<LC> Search(string searchText)
        {
            fail = new List<int>();
            List<LC> StartingPoint = new List<LC>();
            GetFail(searchText);
            for (int k = 0; k < Sentences.Count; k++)
            {
                string S = Sentences[k];
                for (int i = 0, j = 0; i < S.Length; i++)
                {
                    while (j > 0 && S[i] != searchText[j]) j = fail[j - 1];
                    if (S[i] == searchText[j])
                    {
                        if (j == searchText.Length - 1)
                        {
                            LC temp = new LC( k, i - searchText.Length + 1 );
                            StartingPoint.Add(temp);
                            j = fail[j];
                        }
                        else j++;
                    }
                }
            }
            return StartingPoint;
        }

        public Text()
        {

            Sentences = new List<string>();
            Sentences.Add("");
        }
        public List<string> GetText() => Sentences;
        public void SetText(List<string> Sentences) => this.Sentences = Sentences;
        public void Insert(int L, int C, string SubStr)
        {
            string Temp = Sentences[L];
            Temp = Temp.Substring(0, C) + SubStr + Temp.Substring(C);
            Sentences[L] = Temp;
        }

        public void LineFeed(int L, int C)
        {
            string Temp = Sentences[L];
            string Temp2 = Temp.Substring(C);
            Temp = Temp.Substring(0, C);

            Sentences[L] = Temp;
            Sentences.Insert(L + 1, Temp2);
        }
        public int Erase(int L, int C)
        {
            if (C == 0)
            {
                Sentences[L - 1] = Sentences[L - 1] + Sentences[L];
                Sentences.RemoveAt(L);
                return 1;
            }
            Sentences[L] = Sentences[L].Remove(C - 1, 1);
            return 0;
        }
        public void DErase(int L, int C)
        {
            if (L != Sentences.Count - 1 && Sentences[L].Length == C)
            {
                Sentences[L] += Sentences[L + 1];
                Sentences.RemoveAt(L + 1);
            }
            else if(Sentences[L].Length != C)
            {
                Sentences[L]= Sentences[L].Remove(C, 1);
            }
        }
    }
}
