using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.IO;
using System.Threading;

namespace _NS_
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ExampleClass<T> : IDisposable
    {
        /// <summary>
        /// Example of using goto keyword
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static object GoToExample(int i)
        {
            #region double loop escaping
            while(true)
            {
                while(true)
                {
                    goto escLoop;
                }
            }
            escLoop:
            #endregion
            #region switch jumping
            switch(1)
            {
                case 1:
                goto case 2;
                case 2:
                goto default;
                default:
                break;
            }
            #endregion
            return null;
        }

        public void Dispose() {}

        /// <summary>
        /// Example of using implicit operator
        /// </summary>
        public static implicit operator byte(ExampleClass<T> d) { return 0; }
        /// <summary>
        /// Example of using explicit operator
        /// </summary>
        /// <param name="b"></param>
        public static explicit operator ExampleClass<T>(byte b) { return new ExampleClass<T>(); }
    }
}
