using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Parsers
{
    /// <summary>
    /// Simple .csv parser.
    /// Note: assume, that files doesn't contain commas and quotes in values
    /// </summary>
    class CSVParser
    {
        public static string[][] Parse(string data)
        {
            int h = GetHeight(data);
            int w = GetWidth(data);

            // allocate matrix
            string[][] table = new string[h][];
            for (int k = 0; k < h; k++)
            {
                table[k] = new string[w];
            }

            int i = 0, j = 0;

            StringBuilder builder = new StringBuilder();

            foreach (var c in data)
            {
                if (c != ',')
                {
                    if (c != '\n')
                    {
                        // value
                        builder.Append(c);
                    }
                    else
                    {
                        table[i][j] = builder.ToString();
                        builder.Clear();

                        // next line
                        i++;
                        j = 0;
                    }
                }
                else
                {
                    table[i][j] = builder.ToString();
                    j++;

                    builder.Clear();
                }
            }

            return table;
        }

        static int GetWidth(string data)
        {
            // just count commas in first line
            int commas = 0;

            foreach (var c in data)
            {
                if (c == '\n')
                {
                    break;
                }

                if (c == ',')
                {
                    commas++;
                }
            }

            return commas + 1;
        }

        static int GetHeight(string data)
        {
            return data.Count(c => c == '\n') + 1;
        }
    }
}
