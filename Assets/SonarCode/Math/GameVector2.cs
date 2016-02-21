using UnityEngine;
using System.Collections;

public class GameVector2 : MonoBehaviour {
    
    Vector2 vector;
    public readonly static GameVector2 ZERO = new GameVector2(0,0);

    public GameVector2()
    {
        vector = new Vector2();
    }

    public GameVector2(float X, float Y)
    {
        vector = new Vector2(this.X,this.Y);
    }

    public static float Distance(GameVector2 One, GameVector2 Two)
    {
        return Vector2.Distance(One.vector, Two.vector);
    }

    public void Normalize()
    {
        vector.Normalize();
    }

    public float X
    {
        get { return vector.x; }
        set { vector.x = value; }
    }

    public  float Y
    {
        get { return vector.y; }
        set { vector.y = value; }
    }

    public static GameVector2 operator+(GameVector2 op1, GameVector2 op2)
    {
        GameVector2 temp = new GameVector2();
        temp.vector = op1.vector + op2.vector;
        return temp;
    }

    public static GameVector2 operator *(GameVector2 op1, float op2)
    {
        GameVector2 temp = new GameVector2();
        temp.vector = op1.vector * op2;
        return temp;
    }

    public static GameVector2 operator *(GameVector2 op1, GameVector2 op2)
    {
        GameVector2 temp = new GameVector2();
        temp.vector.x = op1.vector.x * op2.vector.x;
        temp.vector.y = op1.vector.y * op2.vector.y;
        return temp;
    }

    public static GameVector2 operator -(GameVector2 op1, GameVector2 op2)
    {
        GameVector2 temp = new GameVector2();
        temp.vector = op1.vector - op2.vector;
        return temp;
    }
}
