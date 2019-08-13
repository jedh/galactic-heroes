using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldGenerator : MonoBehaviour
{
    [Range(0f, 1000f)]
    public float MinStars;

    [Range(0f, 1000f)]
    public float MaxStars;

    [Range(0f, 10f)]
    public float MinScale;

    [Range(0f, 10f)]
    public float MaxScale;

    public Gradient ColorRange;

    [SerializeField]
    private GameObject m_StarObject;

    // Start is called before the first frame update
    void Start()
    {
        var starCount = Random.Range(MinStars, MaxStars);
        for (int i = 0; i < starCount; i++)
        {
            var obj = GameObject.Instantiate(m_StarObject, this.transform);
            var randomScale = Random.Range(MinScale, MaxScale);
            obj.transform.localScale = new Vector3(randomScale, randomScale, 1f);
            var randomPosition = WorldDimensionUtils.GetRandomWorldPosInCamera(Camera.main) * 3f;
            obj.transform.position = new Vector3(randomPosition.x, 0f, randomPosition.z);
            SetRandomColor(obj);
        }
    }

    private void SetRandomColor(GameObject starObject)
    {
        var star = starObject.GetComponent<Star>();
        var randomIndex = Random.Range(0, ColorRange.colorKeys.Length - 1);
        star.StarRenderer.color = ColorRange.colorKeys[randomIndex].color;
    }
}
