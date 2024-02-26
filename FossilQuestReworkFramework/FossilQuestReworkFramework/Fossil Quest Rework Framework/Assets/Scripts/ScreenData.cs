using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenData
{
    public static ScreenSize[] screenSizes = new ScreenSize[4]
    {
        new ScreenSize()
        {
            /*sizes = new Vector2[1] { new Vector2(Screen.width, Screen.height) },
            anchors = new Vector2[1] { new Vector2(0.5f, 0f)},
            pivots = new Vector2[1] { new Vector2(0.5f, 0f)},
            rotations = new Vector3[1] { new Vector3(0, 0, 0)},
            anchoredPos = new Vector3[1] { Vector3.zero }*/
            
            sizes = new Vector2[1] { new Vector2(2160, 1080) },
            anchors = new Vector2[1] { new Vector2(0.5f, 0f)},
            pivots = new Vector2[1] { new Vector2(0.5f, 0f)},
            rotations = new Vector3[1] { new Vector3(0, 0, 0)},
            anchoredPos = new Vector3[1] { new Vector3(840, 0, 0) }
        },
        new ScreenSize()
        {
            sizes = new Vector2[2] { new Vector2(2160, 1920), new Vector2(2160, 1920) },
            anchors = new Vector2[2] { new Vector2(0f, 0.5f), new Vector2(1f, 0.5f)},
            pivots = new Vector2[2] { new Vector2(0.5f, 0f), new Vector2(0.5f, 0f) },
            rotations = new Vector3[2] { new Vector3(0, 0, 270), new Vector3(0, 0, 90)},
            anchoredPos = new Vector3[2] { Vector3.zero, Vector3.zero }
        },
        new ScreenSize()
        {
            sizes = new Vector2[3] { new Vector2(2160, 1680), new Vector2(2160, 1080), new Vector2(2160, 1080) },
            anchors = new Vector2[3] { new Vector2(0f, 0.5f), new Vector2(0.5f, 0f), new Vector2(0.5f, 1f)},
            pivots = new Vector2[3] { new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)},
            rotations = new Vector3[3] { new Vector3(0, 0, 270), new Vector3(0, 0, 0), new Vector3(0, 0, 180) },
            anchoredPos = new Vector3[3] { Vector3.zero, new Vector3(840, 0, 0), new Vector3(840, 0, 0) }
},
        new ScreenSize()
        {
            sizes = new Vector2[4] { new Vector2(2160, 1080), new Vector2(1680, 1080), new Vector2(1680, 1080), new Vector2(2160, 1080) },
            anchors = new Vector2[4] { new Vector2(0f, 0.5f), new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0.5f), },
            pivots = new Vector2[4] { new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f) },
            rotations = new Vector3[4] { new Vector3(0, 0, 270), new Vector3(0, 0, 0), new Vector3(0, 0, 180), new Vector3(0, 0, 90) }
        },

    };
}

public class ScreenSize
{
    public Vector2[] sizes = new Vector2[4] { new Vector2(2160, 1080), new Vector2(1680, 1080), new Vector2(1680, 1080), new Vector2(2160, 1080) };
    public Vector2[] anchors = new Vector2[4] { new Vector2(0f, 0.5f), new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), new Vector2(1f, 0.5f), };
    public Vector2[] pivots = new Vector2[4] { new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f) };
    public Vector3[] rotations = new Vector3[4] { new Vector3(0, 0, 270), new Vector3(0, 0, 0), new Vector3(0, 0, 180), new Vector3(0, 0, 90) };

    public Vector3[] anchoredPos = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, };

}