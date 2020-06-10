﻿using System;
using System.Collections.Generic;

namespace GestionBDDApp.data.utils
{
    public class CollectionsUtils
    {
        public static TValue GetOrCreate<TKey, TValue>(Dictionary<TKey, TValue> Dictionary, TKey Key, Func<TValue> DefaultValueProvider)
        {
            TValue Value;
            if (! Dictionary.TryGetValue(Key, out Value))
            {
                var DefaultValue = DefaultValueProvider.Invoke();
                Dictionary.Add(Key, DefaultValue);
                return DefaultValue;
            }
            return Value;
        }
    }
}