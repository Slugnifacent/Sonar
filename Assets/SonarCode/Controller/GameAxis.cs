using UnityEngine;
using System.Collections;

public class GameAxis {

    string axisName;

	// Use this for initialization
	public GameAxis(string AxisName) {
        axisName = AxisName;
	}

    public float Value
    {
        get { return Input.GetAxis(axisName); }
    }
    
}
