using System;

public class NameID 
{
    [FieldAtrStart(inherit = true)] public string name = "";
    // [BsonId]
    public int id;
    public static implicit operator NameID(NameIDBase dd)
    {
        return new NameID() {name = dd.name, id = dd.id};
    }
}



public abstract class NameIDBase //use this for base! to avoid whole supper class serialized in mongo db 
{

    // [BsonId]
    
    public int id;
    public string name = "";
    // public NameIDBase nameID { get { return new NameIDBase() {id = id, name = name}; } }
}

[Flags]
public enum GameType
{
    Any = ~0, Classic=2, DeathMatch=4, TDM=8, Mod=32, Survival=64,Mission=128,
    zombieMode = 256
}
[Flags]
public enum SupportedPlatforms
{
    unity3dwindows = 1,
    unity3dandroid = 2,
    unity3dios = 4,
    unity3dwebgl = 8,
    All = ~0,
}

public static class ext234
{
    public static  unsafe int GetHashCode2(this string _str)
    {
        fixed (char* str = _str)
        {
            char* chPtr = str;
            int num = 352654597;
            int num2 = num;
            int* numPtr = (int*)chPtr;
            for (int i = _str.Length; i > 0; i -= 4)
            {
                num = (((num << 5) + num) + (num >> 27)) ^ numPtr[0];
                if (i <= 2)
                {
                    break;
                }
                num2 = (((num2 << 5) + num2) + (num2 >> 27)) ^ numPtr[1];
                numPtr += 2;
            }
            return (num + (num2 * 1566083941));
        }
    }
}
