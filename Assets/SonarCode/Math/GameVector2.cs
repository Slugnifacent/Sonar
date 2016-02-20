using UnityEngine;
using System.Collections;

public class GameVector2 : MonoBehaviour {
    
    public Vector2 vector;

    public GameVector2()
    {
    }

    public static Vector2 ZERO
    {
        get { return Vector2.zero; } 
    }

    public float X
    {
        get { return vector.x; }
    }

    public  float Y
    {
        get { return vector.y; }
    }
}
