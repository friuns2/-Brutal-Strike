using System;
using System.Collections.Generic;

public class MyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    public Func<TValue> def;
    public MyDictionary()
    {

    }
    public MyDictionary(Func<TValue> a)
    {
        def = a;
    }
    public new TValue this[TKey key]
    {
        get
        {
            TValue v;
            if (TryGetValue(key, out v))
                return v;
            return base[key] = def != null ? def() : Activator.CreateInstance<TValue>();
        }
        set { base[key] = value; }
    }
}


#if UNITY_5_3_OR_NEWER
public class MyDictionarySerializable<TKey, TValue> : SerializableDictionary<TKey, TValue>
{
    public Func<TValue> def;
    public MyDictionarySerializable()
    {

    }
    public MyDictionarySerializable(Func<TValue> a)
    {
        def = a;
    }
    public new TValue this[TKey key]
    {
        get
        {
            TValue v;
            if (TryGetValue(key, out v))
                return v;
            return base[key] = def != null ? def() : Activator.CreateInstance<TValue>();
        }
        set { base[key] = value; }
    }
}
#endif