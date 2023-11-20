using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Scroller : MonoBehaviour
{

    private List<TranslateObject> _planes = new List<TranslateObject>();

    [SerializeField]
    private EnviromentAssets enviromentAssets;

    [SerializeField]
    private IceAssets iceAssets;

    [Range(0.1f,2)]
    [SerializeField] public float speed = 0.1f;

    [Range(10, 50)]
    [SerializeField] float spawnDistance = 10;

    float timer = 0;

    MovingObject previousBridgeObject; 
    MovingObject previousIceObject;

    private int[] queue = new int[5]; 
    private int[,] iceMap = new int[5, 3];

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnObject(enviromentAssets.bridgeSidePath);
        SpawnIce();
    }
    void SpawnIce()
    {
        int lane = Random.Range(0, 5);
        int spawn = Random.Range(0, 10);
        if (spawn < 7)
        {
            if (queue[lane] > 0) return;
            int length = Random.Range(1, 10);
            queue[lane] = length;
        }


        /*        if (spawn < 5)
                {
                    if (queue[2] > 0) return;

                    queue[0] = 6;
                    queue[1] = 6;
                    queue[2] = 3;
                    queue[3] = 1;
                    queue[4] = 1;
                }*/
        IncrementIce(); 
    }
    void IncrementIce()
    {
        ShiftIceMap();
        for (int i = 0; i < queue.Length; i++)
        {
            iceMap[i, 0] = queue[i] > 0 ? 1 : 0;
            if (queue[i] > 0) queue[i]--; 
        }

        logIceMap();
        InstantiateIce();

    }
    void InstantiateIce()
    {

        for (int lane = 0; lane < 5; lane++)
        {
            Vector3 position = previousIceObject ?
                    Vector3.forward * previousIceObject.transform.position.z +
                    Vector3.forward * 1 + Vector3.forward * 1 + Vector3.right * (lane - 2) * 2 :

                    Vector3.forward * spawnDistance + Vector3.right * (lane - 2) * 2;

            int prevX = lane -1 < 0? 0 : iceMap[lane - 1, 1]; 
            int nextX = lane +1 < 5? iceMap[lane + 1, 1] : 0;

            int prevY = iceMap[lane, 2];
            int nextY = iceMap[lane, 0];

            int dTopLeft = lane - 1 < 0 ? 0 : iceMap[lane - 1, 0];
            int dTopRight= lane + 1 < 5 ? iceMap[lane + 1, 0] : 0;
            int dBottomLeft = lane - 1 < 0 ? 0 : iceMap[lane - 1, 2];
            int dBottomRight = lane + 1 < 5 ? iceMap[lane + 1, 2] : 0;

            int current = iceMap[lane, 1];

            bool toSave = lane == 4; 

            if (prevX == 0 && current == 1 && nextX == 0 &&
                prevY == 0 && nextY == 0) SpawnIceObject(position,iceAssets.IceOPiece, GetRot(Rot.BottomLeft), toSave); // Single Hole
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 0 && nextY == 0) SpawnIceObject(position, iceAssets.IceUPiece, GetRot(Rot.TopLeft), toSave); // U piece looking to the left
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 0) SpawnIceObject(position, iceAssets.IceUPiece, GetRot(Rot.BottomRight), toSave); // U piece looking to the right
            else if (prevX == 0 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 0) SpawnIceObject(position, iceAssets.IceUPiece, GetRot(Rot.BottomLeft), toSave); // U piece looking behind
            else if (prevX == 0 && current == 1 && nextX == 0 &&
                    prevY == 0 && nextY == 1) SpawnIceObject(position, iceAssets.IceUPiece, GetRot(Rot.TopRight), toSave); // U piece looking forwards
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                   prevY == 1 && nextY == 0 &&
                   dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceLWithCornerPiece, GetRot(Rot.BottomLeft), toSave); // L piece with Corner looking left behind
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 0 && nextY == 1 &&
                    dTopLeft == 0) SpawnIceObject(position, iceAssets.IceLWithCornerPiece, GetRot(Rot.TopLeft), toSave); // L piece with Corner looking left forward
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0 &&
                    dBottomRight == 0) SpawnIceObject(position, iceAssets.IceLWithCornerPiece, GetRot(Rot.BottomRight), toSave); // L piece with Corner looking right behind
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1 &&
                    dTopRight == 0) SpawnIceObject(position, iceAssets.IceLWithCornerPiece, GetRot(Rot.TopRight), toSave); // L piece with Corner looking right forward

            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 0) SpawnIceObject(position, iceAssets.IceLPiece, GetRot(Rot.BottomLeft), toSave); // L piece looking left behind
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 0 && nextY == 1) SpawnIceObject(position, iceAssets.IceLPiece, GetRot(Rot.TopLeft), toSave); // L piece looking left forward
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0) SpawnIceObject(position, iceAssets.IceLPiece, GetRot(Rot.BottomRight), toSave); // L piece looking right behind
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1) SpawnIceObject(position, iceAssets.IceLPiece, GetRot(Rot.TopRight), toSave); // L piece looking right forward

            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceIWith2CornerPiece, GetRot(Rot.BottomLeft), toSave); // I  with 2 corner piece vertical right
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0) SpawnIceObject(position, iceAssets.IceIWithCornerRightPiece, GetRot(Rot.BottomLeft), toSave); // I  with corner Right piece vertical right
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceIWithCornerLeftPiece, GetRot(Rot.BottomLeft), toSave); // I  with corner Left piece vertical right
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomRight == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.IceIWith2CornerPiece, GetRot(Rot.TopRight), toSave); // I with 2 corner piece vertical left
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomRight == 0) SpawnIceObject(position, iceAssets.IceIWithCornerRightPiece, GetRot(Rot.TopRight), toSave); // I with corner right piece vertical left
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0) SpawnIceObject(position, iceAssets.IceIWithCornerLeftPiece, GetRot(Rot.TopRight), toSave); // I with corner left piece vertical left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0 &&
                    dBottomLeft == 0 && dBottomRight == 0) SpawnIceObject(position, iceAssets.IceIWith2CornerPiece, GetRot(Rot.BottomRight), toSave); // I with 2 corner piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0 &&
                    dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceIWithCornerRightPiece, GetRot(Rot.BottomRight), toSave); // I with corner right piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0 &&
                    dBottomRight == 0) SpawnIceObject(position, iceAssets.IceIWithCornerLeftPiece, GetRot(Rot.BottomRight), toSave); // I with corner left piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1 &&
                    dTopLeft == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.IceIWith2CornerPiece, GetRot(Rot.TopLeft), toSave); // I with 2 corner  piece horizontal bottom
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1 &&
                    dTopRight == 0) SpawnIceObject(position, iceAssets.IceIWithCornerRightPiece, GetRot(Rot.TopLeft), toSave); // I with corner right piece horizontal bottom
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1 &&
                    dTopLeft == 0) SpawnIceObject(position, iceAssets.IceIWithCornerLeftPiece, GetRot(Rot.TopLeft), toSave); // I with corner left piece horizontal bottom

            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1) SpawnIceObject(position, iceAssets.IceIPiece, GetRot(Rot.BottomLeft), toSave); // I piece vertical right
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1) SpawnIceObject(position, iceAssets.IceIPiece, GetRot(Rot.TopRight), toSave); // I piece vertical left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0) SpawnIceObject(position, iceAssets.IceIPiece, GetRot(Rot.BottomRight), toSave); // I piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1) SpawnIceObject(position, iceAssets.IceIPiece, GetRot(Rot.TopLeft), toSave); // I piece horizontal bottom

            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0 && dTopRight == 0 && dBottomRight == 0) SpawnIceObject(position, iceAssets.Ice4CornerPiece, GetRot(Rot.BottomLeft), toSave); // C small 4 corner piece
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0 && dBottomRight ==0) SpawnIceObject(position, iceAssets.Ice3CornerPiece, GetRot(Rot.BottomLeft), toSave); // C small 3 corner piece bottom left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomLeft == 0 && dBottomRight == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.Ice3CornerPiece, GetRot(Rot.BottomRight), toSave); // C small 3 corner piece bottom right
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.Ice3CornerPiece, GetRot(Rot.TopLeft), toSave); // C small 3 corner piece top left
            // Not tested Yet, but I think it works.... 
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomRight == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.Ice3CornerPiece, GetRot(Rot.TopRight), toSave); // C small 3 corner piece top right

            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0 && dTopLeft == 0) SpawnIceObject(position, iceAssets.Ice2CornerPiece, GetRot(Rot.TopRight), toSave); // C small 2 corner piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomRight == 0 && dBottomLeft == 0) SpawnIceObject(position, iceAssets.Ice2CornerPiece, GetRot(Rot.BottomLeft), toSave); // C small 2 corner piece horizontal bottom
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0 && dBottomRight == 0) SpawnIceObject(position, iceAssets.Ice2CornerPiece, GetRot(Rot.BottomRight), toSave); // C small 2 corner piece vertical right
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0) SpawnIceObject(position, iceAssets.Ice2CornerPiece, GetRot(Rot.TopLeft), toSave); // C small 2 corner piece vertical left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomRight == 0) SpawnIceObject(position, iceAssets.Ice2CornerDiagonalPiece, GetRot(Rot.TopLeft), toSave); // C small 2 corner diagonal piece top left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0 && dBottomLeft == 0) SpawnIceObject(position, iceAssets.Ice2CornerDiagonalPiece, GetRot(Rot.TopRight), toSave); // C small 2 corner diagonal piece top right

            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0) SpawnIceObject(position, iceAssets.IceCornerPiece, GetRot(Rot.TopLeft), toSave); // C small corner piece top left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0) SpawnIceObject(position, iceAssets.IceCornerPiece, GetRot(Rot.TopRight), toSave); // C small corner piece top right
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceCornerPiece, GetRot(Rot.BottomLeft), toSave); // C small corner piece bottom left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomRight == 0) SpawnIceObject(position, iceAssets.IceCornerPiece, GetRot(Rot.BottomRight), toSave); // C small corner piece bottom right

            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 0) SpawnIceObject(position, iceAssets.IceHPiece, GetRot(Rot.TopLeft), toSave); // H piece Horizontal
            else if (prevX == 0 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1) SpawnIceObject(position, iceAssets.IceHPiece, GetRot(Rot.BottomLeft), toSave); // H piece Vertical
            else if (current == 0) SpawnIceObject(position, iceAssets.IceFlat, GetRot(Rot.BottomLeft), toSave); // Solid
            else if (current == 1) SpawnIceObject(position, iceAssets.IceEmptyPiece, GetRot(Rot.BottomLeft), toSave); // Empty 
        }
    }
    void SpawnIceObject(Vector3 position, GameObject iceObject, Quaternion orientation , bool last = false)
    {
        GameObject newBridge = Instantiate(iceObject, position - Vector3.forward * speed, orientation, this.transform);
        MovingObject newMoving = newBridge.AddComponent<MovingObject>();
        newMoving.SetScroller(this);
        if (last)previousIceObject = newMoving;

    }

    void ShiftIceMap()
    {
        int[,] shiftedArray = new int[5, 3];

        for (int lane = 0; lane < 5; lane++)
        {
            for (int row = 2; row > 0; row--)
            {
                shiftedArray[lane, row] = iceMap[lane, row - 1];
            }
            shiftedArray[lane, 0] = 0;
        }
        iceMap = shiftedArray;
    }
    void logIceMap()
    {
        string shitLog = $"iceMap: \n{iceMap[0,0]},{iceMap[1, 0]},{iceMap[2, 0]},{iceMap[3, 0]},{iceMap[4, 0]}\n{iceMap[0, 1]},{iceMap[1, 1]},{iceMap[2, 1]},{iceMap[3, 1]},{iceMap[4, 1]}\n{iceMap[0, 2]},{iceMap[1, 2]},{iceMap[2, 2]},{iceMap[3,2]},{iceMap[4, 2]}\n";
        Debug.Log(shitLog);
    }



    void SpawnObject(GameObject prefab,float offset = 0)
    {
        Vector3 position = previousBridgeObject ? 
            previousBridgeObject.transform.position + 
            Vector3.forward * previousBridgeObject.gameObject.GetComponent<Renderer>().bounds.size.z/2 +
            Vector3.forward * prefab.GetComponent<Renderer>().bounds.size.z/2:

            Vector3.forward * spawnDistance;

        GameObject newBridge = Instantiate(prefab, position - Vector3.forward * speed, Quaternion.Euler(-90, 0, 0), this.transform);
        MovingObject newMoving = newBridge.AddComponent<MovingObject>();
        newMoving.SetScroller(this);
        previousBridgeObject = newMoving;
    }
    void SpawnBridge()
    {
        SpawnObject(enviromentAssets.bridge);
        int[] paths = new int[5];
        bool possible = false;
        for (int i = 0; i < paths.Length; i++)
        {
            int open = Random.Range(0, 2);
            paths[i] = open;
            if (open == 1) possible = true;
        }
        if (!possible)
        {
            int open = Random.Range(0, 5);
            paths[open] = 1;
        }

        for (int i = 0; i < paths.Length; i++)
        {

            int previous = i -1 < 0? 0: paths[i-1]; 
            int current = paths[i];
            int next = i + 1 < paths.Length ? paths[i + 1] : 0;
            Vector3 position = previousBridgeObject.transform.position + Vector3.right * (i - 2)*2;

            if (previous == 0 && current == 1 && next == 0) SpawnBridgeOpening(position, enviromentAssets.bridgeSingleOpening); // Single opening 
            else if (previous == 1 && current == 1 && next == 0) SpawnBridgeOpening(position, enviromentAssets.bridgeLPieceOpening);// L piece flipped
            else if (previous == 0 && current == 1 && next == 1) SpawnBridgeOpening(position, enviromentAssets.bridgeLPieceOpening, true);// L piece 
            else if (previous == 1 && current == 1 && next == 1) SpawnBridgeOpening(position, enviromentAssets.bridgeOpeningConnector);// connector piece 
            else if (current == 0) SpawnBridgeOpening(position, enviromentAssets.bridgeClosed);// blocked
        }

/*
        string logShit = $"[{paths[0]},{paths[1]},{paths[2]},{paths[3]},{paths[4]}]"; 
        Debug.Log(logShit); */

    }
    void SpawnBridgeOpening(Vector3 position, GameObject openining, bool flip = false)
    {
        GameObject newOpening = Instantiate(openining, position, Quaternion.Euler(-90, flip?0:180, 0), this.transform);
        MovingObject newMoving = newOpening.AddComponent<MovingObject>();
        newMoving.SetScroller(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {

        if (spawnDistance - previousBridgeObject.transform.position.z > 0)
        {
            int random = Random.Range(0, 10);
/*            if (random < 2) SpawnBridge();
            else */SpawnObject(enviromentAssets.bridgeSidePath); 
        }

        if(spawnDistance - previousIceObject.transform.position.z > 0)
        {
            SpawnIce(); 
        }
    }

    Quaternion GetRot(Rot rotationType)
    {
        switch (rotationType)
        {
            case Rot.TopLeft:
                return Quaternion.Euler(-90, 90, 0);
            case Rot.TopRight:
                return Quaternion.Euler(-90, 180, 0);
            case Rot.BottomLeft:
                return Quaternion.Euler(-90, 0, 0);
            case Rot.BottomRight:
                return Quaternion.Euler(-90, -90, 0);
            default:
                return Quaternion.identity;
        }
    }


}

[Serializable]
public class EnviromentAssets
{
    public GameObject bridge;
    public GameObject bridgeClosed;
    public GameObject bridgeLPieceOpening;
    public GameObject bridgeOpeningConnector;
    public GameObject bridgeSingleOpening;
    public GameObject bridgeSidePath;
}

[Serializable]
public class IceAssets
{
    public GameObject IceFlat;
    public GameObject IceHPiece;
    public GameObject IceIPiece;
    public GameObject IceLPiece;
    public GameObject IceOPiece;
    public GameObject IceUPiece;
    public GameObject IceCornerPiece; 
    public GameObject Ice2CornerPiece; 
    public GameObject Ice2CornerDiagonalPiece;
    public GameObject Ice3CornerPiece;
    public GameObject Ice4CornerPiece;
    public GameObject IceEmptyPiece;
    public GameObject IceLWithCornerPiece;
    public GameObject IceIWithCornerLeftPiece;
    public GameObject IceIWithCornerRightPiece;
    public GameObject IceIWith2CornerPiece;
    public GameObject IceIslandCornerPiece;
    public GameObject IceSingleDeadEndPathPiece;
    public GameObject IceSinglePathPiece;
    public GameObject IceStrandedPiece;
    public GameObject IceTurnPathPiece;
}


public enum Rot
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
