using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Singleton<World>
{
    public const float LANE_DISTANCE = 5.0f;
    private static float[] lanesXs;
    public static float[] LanesXs
    {
        get { return lanesXs; }
    }
    private const int numberOfLanes = 3;

    private void Awake()
    {
        InitialiseLanes();
    }

    private static void InitialiseLanes()
    {
        lanesXs = new float[numberOfLanes];
        int middleLaneIndex = numberOfLanes / 2;
        lanesXs[middleLaneIndex] = 0f;
        float normaliser = middleLaneIndex * LANE_DISTANCE;
        for (int i = 0; i < lanesXs.Length; i++)
        {
            lanesXs[i] = (LANE_DISTANCE * i) - normaliser;
            Debug.Log("LaneZ = " + lanesXs[i]);
        }
    }

}
