using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DamageAble
{
    void takeDamage(int amount);
    void recoverDamage(int amount);
}
