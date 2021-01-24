using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlink : MonoBehaviour
{
    public List<Light> lights;
    private bool isOn;
    private float deltaTime;
    private float maxTimeOn = 1f;
    private float maxTimeOff = 0.9f;
    private float timeToSwap;

    private void Awake()
    {
        isOn = true;
        timeToSwap = maxTimeOn;
        deltaTime = 0;
    }

    void Update()
    {
        deltaTime += Time.deltaTime;
        if (deltaTime >= timeToSwap)
        {
            SwapLights();
        }
    }

    private void SwapLights()
    {
        isOn = !isOn;
        deltaTime = 0;
        if (isOn)
        {
            timeToSwap = Random.Range(0, maxTimeOn);
        } else
        {
            timeToSwap = Random.Range(0, maxTimeOff);
        }
        foreach(Light light in lights)
        {
            light.enabled = isOn;
        }
    }
}
