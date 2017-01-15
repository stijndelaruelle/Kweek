﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageableObject
{
    void Damage(int health);
    IDamageableObject GetMainDamageableObject();
}
