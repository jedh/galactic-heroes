using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public SpriteRenderer StarRenderer { get; private set; }

    private void Awake()
    {
        StarRenderer = this.GetComponent<SpriteRenderer>();
    }
}
