﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PhyGames
{
    [CreateAssetMenu(fileName = "Symbol", menuName = "NBack/Create a new symbol", order = 0)]
    public class Symbol : ScriptableObject
    {
        public string id;
        public Sprite image;
    }
}
