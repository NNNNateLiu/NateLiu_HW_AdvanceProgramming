using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class RareTireData : ScriptableObject
{
    public int Tire1Rate;
    public int Tire2Rate;
    public int Tire3Rate;
    public int Tire4Rate;

    public List<string> Tire1WeaponIDPool;
    public List<string> Tire2WeaponIDPool;
    public List<string> Tire3WeaponIDPool;
    public List<string> Tire4WeaponIDPool;

    public void Initialize()
    {
        Tire1WeaponIDPool.Clear();
        Tire2WeaponIDPool.Clear();
        Tire3WeaponIDPool.Clear();
        Tire4WeaponIDPool.Clear();
    }
}
