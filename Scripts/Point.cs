using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point{
    
    public int x;
    public int y;

    public Point(int nx, int ny){
        x = nx;
        y = ny;
    }

    public bool Equals(Point p){
        return (x == p.x && y == p.y);
    }

    public void Add(Point p){
        x += p.x;
        y += p.y;
    }

    public void Multiply(int m){
        x *= m;
        y *= m;
    }

    public Vector2 ToVector(){
        return new Vector2(x,y);
    }

    public static Point FromVector(Vector2 v){
        return new Point((int) v.x, (int) v.y);
    }

    public static Point Add(Point p, Point o){
        return new Point(p.x + o.x, p.y + o.y);
    }

    public static Point Multiply(Point p, int m){
        return new Point(p.x * m, p.y * m);
    }

    public static Point Clone(Point p){
        return new Point(p.x, p.y);
    }

    public static Point Zero(){
        return new Point(0,0);
    }

    public static Point One(){
        return new Point(1,1);
    }

    public static Point Up(){
        return new Point(0,1);
    }

    public static Point Down(){
        return new Point(0,-1);
    }

    public static Point Right(){
        return new Point(1,0);
    }

    public static Point Left(){
        return new Point(-1,0);
    }

}
