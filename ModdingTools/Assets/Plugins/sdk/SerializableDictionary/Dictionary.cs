//using System.Collections.Generic;
//using System.Runtime.Serialization;
//using JetBrains.Annotations;

//public class Dictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>
//{

//    public Dictionary()
//    {
//    }
//    public Dictionary(IEqualityComparer<TKey> comparer) : base(comparer)
//    {
//    }
//    public Dictionary([NotNull] IDictionary<TKey, TValue> dictionary) : base(dictionary)
//    {
//    }
//    public Dictionary(int capacity) : base(capacity)
//    {
//    }
//    public Dictionary([NotNull] IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
//    {
//    }
//    public Dictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
//    {
//    }
//    protected Dictionary(SerializationInfo info, StreamingContext context) : base(info, context)
//    {
//    }
//}