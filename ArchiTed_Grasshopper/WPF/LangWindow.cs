/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArchiTed_Grasshopper.WPF
{
    public class LangWindow : Window
    {




        public LangWindow()
        {
            //InitializeComponent();
        }


        #region Language
        public void WindowLanguageChanged(object sender, EventArgs e)
        {
            LanguageChanged();
        }
        protected virtual void LanguageChanged() { }
        #endregion

        #region OpenClosed
        protected override void OnClosed(EventArgs e)
        {
            try
            {
                LanguagableComponent.LanguageChanged -= WindowLanguageChanged;
            }
            catch
            {

            }

            base.OnClosed(e);
        }

        public override void EndInit()
        {
            base.EndInit();
            LanguageChanged();
        }
        #endregion

        public static List<ISearchItem> Search(string param, ISearchItem[] items)
        {
            if (string.IsNullOrWhiteSpace(param) || items == null || items.Length == 0)
                return new List<ISearchItem>();

            param = param.ToLower();

            string[] words = param
                                .Split(new char[] { ' ', '\u3000' }, StringSplitOptions.RemoveEmptyEntries)
                                .OrderBy(s => s.Length)
                                .ToArray();

            var q = from sentence in items.AsParallel()
                    let MLL = Mul_LnCS_Length(sentence.FindDesc.ToLower(), words)
                    where MLL >= 0
                    orderby ((MLL + 0.5) / sentence.FindDesc.Length)
                    select sentence;

            return q.ToList();

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="words">多个关键字。长度必须大于0，必须按照字符串长度升序排列。</param>
        /// <returns></returns>
        static int Mul_LnCS_Length(string sentence, string[] words)
        {
            int sLength = sentence.Length;
            int result = sLength;
            bool[] flags = new bool[sLength];
            int[,] C = new int[sLength + 1, words[words.Length - 1].Length + 1];
            //int[,] C = new int[sLength + 1, words.Select(s => s.Length).Max() + 1];
            foreach (string word in words)
            {
                int wLength = word.Length;
                int first = 0, last = 0;
                int i = 0, j = 0, LCS_L;
                //foreach 速度会有所提升，还可以加剪枝
                for (i = 0; i < sLength; i++)
                    for (j = 0; j < wLength; j++)
                        if (sentence[i] == word[j])
                        {
                            C[i + 1, j + 1] = C[i, j] + 1;
                            if (first < C[i, j])
                            {
                                last = i;
                                first = C[i, j];
                            }
                        }
                        else
                            C[i + 1, j + 1] = Math.Max(C[i, j + 1], C[i + 1, j]);

                LCS_L = C[i, j];
                if (LCS_L <= wLength >> 1)
                    return -1;

                while (i > 0 && j > 0)
                {
                    if (C[i - 1, j - 1] + 1 == C[i, j])
                    {
                        i--;
                        j--;
                        if (!flags[i])
                        {
                            flags[i] = true;
                            result--;
                        }
                        first = i;
                    }
                    else if (C[i - 1, j] == C[i, j])
                        i--;
                    else// if (C[i, j - 1] == C[i, j])
                        j--;
                }

                if (LCS_L <= (last - first + 1) >> 1)
                    return -1;
            }

            return result;
        }
    }
}
