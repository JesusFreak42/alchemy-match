using System.Collections;
using UnityEngine;

[System.Serializable]
public class ArrayLayout {
    
    [System.Serializable]
    public struct RowData{
        public bool[] row;
    }

    public Grid grid;
    public RowData[] rows = new RowData[14]; //grid of 7x7

}
