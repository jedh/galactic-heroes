using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CanvasFlicker : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup m_CanvsGroup;

    [SerializeField]
    [Range(1, 10)]
    private int m_Frequency;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_MinAlpha;

    private Unity.Mathematics.Random m_Random;

    private int m_FlickerCounter;

    // Start is called before the first frame update
    void Start()
    {
        m_Random = new Unity.Mathematics.Random((uint)DateTime.UtcNow.Ticks);
    }

    void FixedUpdate()
    {
        if (m_CanvsGroup != null)
        {
            m_FlickerCounter += 1;
            if (m_FlickerCounter % m_Frequency == 0)
            {
                m_CanvsGroup.alpha = m_Random.NextFloat(m_MinAlpha, 1.0f);
                m_FlickerCounter = 0;
            }
        }
    }
}
