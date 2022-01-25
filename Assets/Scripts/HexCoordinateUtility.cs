using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexCoordinateUtility : MonoBehaviour
{
    //Red Blob Games helpfully provided some pseudo-code to do some common Hex math we'll need
    public static Vector3Int CubeToOffset(Vector3Int cube) {
        //The bitwise allows us to catch odd negative numbers, which is nice. 
        int column = cube.x + (cube.z - (cube.z&1)) / 2;
        int row = cube.z;
        return new Vector3Int(column, row, 0);
    }

    public static Vector3Int OffsetToCube(Vector3Int UnityCoord) {
        int x = UnityCoord.x - (UnityCoord.y - (UnityCoord.y&1)) / 2;
        int z = UnityCoord.y;
        int y = -x-z;
        return new Vector3Int(x, y, z);
    }

    //Red Blob again coming through in the clutch.
    public static int HexDistance(Vector3Int AHex, Vector3Int BHex) {
        int FirstCompare = Math.Max(Math.Abs(AHex.x - BHex.x), Math.Abs(AHex.y - BHex.y)); 
        return Math.Max(FirstCompare, Math.Abs(AHex.z - BHex.z));
    }

    //We'll run these clockwise, starting at the top right.  
    public static Dictionary<string, Vector3Int> cubeDirections = new Dictionary<string, Vector3Int>(){
        {"NE", new Vector3Int(0, -1, 1)},
        {"E", new Vector3Int(1, -1, 0)},
        {"SE", new Vector3Int(1, 0, -1)},
        {"SW", new Vector3Int(0, 1, -1)},
        {"W", new Vector3Int(-1, 1, 0)},
        {"NW", new Vector3Int(-1, 0, 1)},
    };

    //Red Blob strikes again, we need to find movement range for many reasons, but most importantly
    //for our level generation.  Since there won't be a hard border on any side (besides the south's
    //plant growth), we want to want the player to be a on a treadmill of sorts.
    public static List<Vector3Int> HexRange(Vector3Int Coord, int Distance) {
        List<Vector3Int> inRange = new List<Vector3Int>();
        for (int i = -Distance; i <= Distance; i++ ) {
            for (int j = Math.Max(-Distance, -i-Distance); j <= Math.Min(Distance, -i + Distance); j++) {
                int k = -i-j;
                //These are all relative to a center Hex, so we have to add this i,j,k to our original coord.
                Vector3Int AdjustedHex = Coord + new Vector3Int(i, j, k);
                inRange.Add(AdjustedHex);
            }
        }
        return inRange;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
