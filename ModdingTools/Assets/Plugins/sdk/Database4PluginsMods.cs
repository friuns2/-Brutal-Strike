
using UnityEngine;

public interface IPassive { }


public struct PosRotStruct
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
}

public enum TeamEnum
{
    None,
    Spectators,
    DeathMatch,
    Terrorists,
    CounterTerrorists, Draw,WaitPls,
    team1,team2,team3,team4,team5,team6,team7,team8,team9
}
public enum WeaponGroupE
{
    None = 0,
    Primary = 1,
    Pistol = 2,
    Knife = 3,
    Grenade = 4,
    Bomb = 5,
    Scope = 6,
    Silencer = 7,
    BodyArmor = 8,
    Helmet = 9,
    Grip = 10,
    Clip = 11,
    Stock = 12,Medkit=13
}