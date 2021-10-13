using System;
using System.Collections.Generic;
using System.Text;

namespace Networking
{
    public static class ExtensionMethod
    {
        public static string EnumerateToString<T>(this IEnumerable<T> enumerable, string separator = null, int separatorRepetitiveness = 0)
        {
            return EnumerateToString<T>(enumerable.GetEnumerator(), separator, separatorRepetitiveness);
        }
        public static string EnumerateToString<T>(this IEnumerator<T> enumerator, string separator = null, int separatorRepetitiveness = 0)
        {
            StringBuilder builder = new StringBuilder();
            if (enumerator.MoveNext())
            {
                builder.Append(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    int i = 0;
                    if (i == separatorRepetitiveness)
                    {
                        if (separator != null)
                        {
                            builder.Append(separator);
                        }
                        i = 0;
                    }
                    else
                    { i++; }

                    builder.Append(enumerator.Current);
                }
            }
            return builder.ToString();
        }
    }
}
