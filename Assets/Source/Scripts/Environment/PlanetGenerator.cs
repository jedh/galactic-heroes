using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    public Sprite[] PlanetSprites;

    [Range(0f, 10f)]
    public float MinScale;

    [Range(0f, 10f)]
    public float MaxScale;

    [Range(0f, 1f)]
    public float ShowPlanetOdds;

    // Start is called before the first frame update
    void Start()
    {
        var showPlanetRatio = Random.Range(0f, 1f);
        if (showPlanetRatio <= ShowPlanetOdds)
        {
            var randomIndex = Random.Range(0, PlanetSprites.Length - 1);
            var sprite = PlanetSprites[randomIndex];
            if (sprite != null)
            {
                var obj = new GameObject("Planet");
                var renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = 2;
                obj.transform.Rotate(Vector3.right, 90f);
                var randomScale = Random.Range(MinScale, MaxScale);
                obj.transform.localScale = new Vector3(randomScale, randomScale, 1f);
                var randomPosition = WorldDimensionUtils.GetRandomWorldPosInCamera(Camera.main);
                obj.transform.position = new Vector3(randomPosition.x, 0f, randomPosition.z);
                obj.transform.parent = this.transform;
            }
        }
    }
}
