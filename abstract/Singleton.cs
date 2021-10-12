using System;
using System.Collections;
using System.Collections.Generic;

namespace _NS_
{
    abstract class Singleton<T> where T: new()
    {
        static T s_instance;
        public static T Instance { get { return s_instance == null ? s_instance = new T() : s_instance; } }
    }
}