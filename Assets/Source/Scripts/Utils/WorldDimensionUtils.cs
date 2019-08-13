using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDimensionUtils
{
    public static Vector3 GetRandomWorldPosInCamera(Camera camera)
    {
        var screenPoint = new Vector3(camera.scaledPixelWidth, 0f, camera.scaledPixelHeight);
        var worldDimensions = camera.ScreenToWorldPoint(screenPoint);
        var randomPosition = new Vector3
        (
            Random.Range(-worldDimensions.x, worldDimensions.x),
            Random.Range(-worldDimensions.y, worldDimensions.y),
            Random.Range(-worldDimensions.z, worldDimensions.z)
        );

        return randomPosition;
    }
}
