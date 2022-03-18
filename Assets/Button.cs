using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Vector2Int Position;

    public void Click()
    {
        GameManager.instance.Click(Position);
    }
}
