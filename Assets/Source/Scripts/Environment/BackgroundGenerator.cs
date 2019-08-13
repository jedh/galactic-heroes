using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    public Sprite[] BackgroundSprites;

    // Start is called before the first frame update
    void Start()
    {
        var randomIndex = Random.Range(0, BackgroundSprites.Length - 1);
        var sprite = BackgroundSprites[randomIndex];
        if (sprite != null)
        {
            var obj = new GameObject("Background Image");
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 1;
            obj.transform.Rotate(Vector3.right, 90f);
            var randomPosition = WorldDimensionUtils.GetRandomWorldPosInCamera(Camera.main);
            obj.transform.position = new Vector3(randomPosition.x, 0f, randomPosition.z);
            obj.transform.parent = this.transform;
        }
    }
}
