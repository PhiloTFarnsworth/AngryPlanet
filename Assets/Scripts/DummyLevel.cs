using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DummyLevel : MonoBehaviour
{
    public AnimatedTile protoTile;

    public void newMap() {
        var tileSet = Random.Range(0,8); 
        foreach(Vector3Int HexCoord in HexCoordinateUtility.HexRange(new Vector3Int(0,0,0), 15)) {

            //Translate our cube coordinates and place the tile.
            Vector3Int OffPosition = HexCoordinateUtility.CubeToOffset(HexCoord);
            Vector3 WorldPosition = GetComponent<GridLayout>().CellToWorld(OffPosition);

            //Choose a random tile from the starting tiles.
            
            int SpriteChoice = Random.Range(tileSet*4, tileSet*4 + 3);
            //create our animation arrays
            Sprite[] Animation = new Sprite[8];
            //Choose the animation set
            for (int i = 0; i < 8; i++) {
                Animation[i] = (AssetReference.AnimatedTiles[SpriteChoice*8+i]);
            }
            
            //Use our created AnimatedTile to create a new one
            AnimatedTile newTile = protoTile;
            newTile.m_AnimatedSprites = Animation;

            //Set that tile.
            GetComponent<Tilemap>().SetTile(OffPosition, newTile);
            
            //Rotate Tile to increase variety.  Scale down as well.
            int rotation = Random.Range(0, 6);
            GetComponent<Tilemap>().SetTransformMatrix(OffPosition, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f,0f, rotation*60f), new Vector3(.67f,.67f,0)));

            if (Random.Range(0, 8) < 4) {        
                GameObject Growth = Instantiate(AssetReference.Prefabs["Plant"], WorldPosition, Quaternion.identity);
                var aPlant = Growth.GetComponent<Plant>();
                Growth.transform.parent = transform;
                aPlant.SetToken("Plant", HexCoord, 1, false);
                aPlant.Init(new List<int> {-1}, 4, true);
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        newMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
