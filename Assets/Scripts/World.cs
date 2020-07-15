using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Singleton<World>
{
    public float horizontalLaneSpacing = 5.0f;
    public static float HorizontalLaneSpacing
    {
        get { return Instance.horizontalLaneSpacing; }
    }

    public const float LANE_VERTICAL_SPACING = 3.5f;
    public const float FLOOR_LEVEL = 0f;


    public static Vector2[,] LanesXYs { get; private set; }
    public const int NUMBER_OF_LANES = 3;
    public const int NUMBER_OF_VERTICAL_DIVISIONS_PER_LANE = 2;

    private void Awake()
    {
        InitialiseLanes();
    }

    private void InitialiseLanes()
    {
        LanesXYs = new Vector2[NUMBER_OF_LANES, NUMBER_OF_VERTICAL_DIVISIONS_PER_LANE];
        int middleLaneIndex = NUMBER_OF_LANES / 2;
        float xNormaliser = middleLaneIndex * horizontalLaneSpacing;
        for (int x = 0; x < NUMBER_OF_LANES; x++)
        {
            for (int y = 0; y < NUMBER_OF_VERTICAL_DIVISIONS_PER_LANE; y++)
            {
                LanesXYs[x,y] = new Vector2
                    (((horizontalLaneSpacing * x) - xNormaliser), y * LANE_VERTICAL_SPACING);
            }
        }
    }

}
