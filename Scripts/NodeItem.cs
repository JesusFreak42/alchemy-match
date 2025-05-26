using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Node Item")]
public class NodeItem : ScriptableObject
{

    public string name;
    public Sprite sprite;
    public bool matchable = true;
    public bool killAtBottom = false; //if this block dies once it reaches the bottom row
    public float chanceOfUsing = 1f; //A number 0-1. When the block is randomly chosen to spawn: 0 means it's always repicked, 1 means it's never repicked, 0.5 means 50% chance of repick
    public bool aliveOnStart = true; //whether the block is part of the board on start

    public bool Equals(NodeItem i){
        return name == i.name;
    }

}

