using System.Collections.Generic;
using UnityEngine;


public interface IEquipable
{
    void Equip(GameObject user);
    void Unequip(GameObject user);
}

public interface IUsable 
{
   void Use(GameObject user);
}