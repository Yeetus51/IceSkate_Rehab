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

    [SerializeField] private bool singleLaneMode = false;

    [Range(1, 3)]
    [SerializeField] public int maxLaneChange = 1;

    [Range(0.5f, 3)]
    [SerializeField] private float laneChangeFrequency = 1;

    [Range(0, 10)]
    [SerializeField] private int bridgeSpawnRate = 2; 

    [Space(30f)]
    [Range(0.1f,5)]
    [SerializeField] public float speed = 0.1f;

    [Range(10, 100)]
    [SerializeField] float spawnDistance = 10;

    [Range(10, 50)]
    [SerializeField] public float despawnDistance = 10;

    float timer = 0;

    GameObject previousBridgeObject; 
    GameObject previousIceObject;

    private int[] queue = new int[5];
    private int[] feasibility = new int[5]; 
    private int[,] iceMap = new int[5, 3];

    private int holeGenerationPause = 0;

    private int alwaysOpenLane = 2;
    private int changeOpenLane = 5; 
/*    private bool spawningExtraIce;
    bool once = false; */



    private Dictionary<GameObject, Queue<GameObject>> objectPools;
    private Dictionary<GameObject, GameObject> poolHolders;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

        objectPools = new Dictionary<GameObject, Queue<GameObject>>();
        poolHolders = new Dictionary<GameObject, GameObject>();

        InitializePool(iceAssets.IceFlat);
        InitializePool(iceAssets.IceHPiece);
        InitializePool(iceAssets.IceIPiece);
        InitializePool(iceAssets.IceLPiece);
        InitializePool(iceAssets.IceOPiece);
        InitializePool(iceAssets.IceUPiece);
        InitializePool(iceAssets.IceCornerPiece);
        InitializePool(iceAssets.Ice2CornerPiece);
        InitializePool(iceAssets.Ice2CornerDiagonalPiece);
        InitializePool(iceAssets.Ice3CornerPiece);
        InitializePool(iceAssets.Ice4CornerPiece);
        InitializePool(iceAssets.IceEmptyPiece);
        InitializePool(iceAssets.IceLWithCornerPiece);
        InitializePool(iceAssets.IceIWithCornerLeftPiece);
        InitializePool(iceAssets.IceIWithCornerRightPiece);
        InitializePool(iceAssets.IceIWith2CornerPiece);


        InitializePool(enviromentAssets.bridgeSidePath);
        InitializePool(enviromentAssets.bridge);
        InitializePool(enviromentAssets.bridgeClosed);
        InitializePool(enviromentAssets.bridgeLPieceOpening);
        InitializePool(enviromentAssets.bridgeOpeningConnector);
        InitializePool(enviromentAssets.bridgeSingleOpening);

        SpawnEnviroment(enviromentAssets.bridgeSidePath);

        SpawnIce();
    }

    void InitializePool(GameObject prefab)
    {
        Queue<GameObject> newPool = new Queue<GameObject>();
        GameObject poolHolder = new GameObject($"{prefab.name} Pool");
        poolHolder.transform.SetParent(this.transform);

        poolHolders[prefab] = poolHolder;

        for (int i = 0; i < 10; i++)
        {
            GameObject newObj = InitializeNewObject(prefab, poolHolder.transform);
            newObj.SetActive(false);
            newPool.Enqueue(newObj);
        }

        objectPools.Add(prefab, newPool);
    }
    GameObject InitializeNewObject(GameObject prefab, Transform Holder)
    {
        GameObject newObj = Instantiate(prefab, Holder);
        MovingObject newMoving = newObj.AddComponent<MovingObject>();
        newMoving.SetScroller(this);
        newMoving.SetPrefab(prefab);
        return newObj; 
    }
    public GameObject GetPooledObject(GameObject prefab)
    {
        if (!objectPools.ContainsKey(prefab) || objectPools[prefab].Count == 0)
        {
            GameObject newObj = InitializeNewObject(prefab, poolHolders[prefab].transform);
            newObj.SetActive(true);
            return newObj;
        }
        else
        {
            GameObject objToSpawn = objectPools[prefab].Dequeue();
            objToSpawn.SetActive(true);
            return objToSpawn;
        }
    }

    public void ReturnObjectToPool(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        objectPools[prefab].Enqueue(obj);
    }



    void SpawnIce()
    {
        if(changeOpenLane <= 0 && holeGenerationPause <= 0)
        {
            int previousLane = alwaysOpenLane; 
            changeOpenLane = Random.Range((int)(5 * (1/laneChangeFrequency)), (int)(15 * 1/laneChangeFrequency));
            alwaysOpenLane = Random.Range(Mathf.Clamp(alwaysOpenLane - maxLaneChange, 0,4), Mathf.Clamp(alwaysOpenLane + maxLaneChange +1, 0, 4));
            Debug.Log(alwaysOpenLane);

            holeGenerationPause += maxLaneChange -1;

            for (int i = 0; i < 5; i++)
            {
                queue[i] = 0;
            }
        }


        int lane = Random.Range(0, 5);
        int spawn = Random.Range(0, 10);

        if (singleLaneMode)
        {
            int length = Random.Range(1, 7);
            if (0 != alwaysOpenLane && queue[0] <= 0) queue[0] = length;
            if (1 != alwaysOpenLane && queue[1] <= 0) queue[1] = length;
            if (2 != alwaysOpenLane && queue[2] <= 0) queue[2] = length;
            if (3 != alwaysOpenLane && queue[3] <= 0) queue[3] = length;
            if (4 != alwaysOpenLane && queue[4] <= 0) queue[4] = length;
        }
        else
        {
            if (spawn < 7 && lane != alwaysOpenLane)
            {
                if (queue[lane] <= 0)
                {
                    int length = Random.Range(1, 7);
                    queue[lane] = length;
                }
            }
        }



        IncrementIce();
        changeOpenLane--; 


/*        if (spawnDistance - previousIceObject.transform.position.z > 3)
        {
            Debug.Log("Spawining new Shit" + previousIceObject.transform.position);
            if (!once)
            {
                once = true;
                //speed = 0.1f;
                SpawnIce();
                spawningExtraIce = true; 
            }
        }*/
    }
    void IncrementIce()
    {
        ShiftIceMap();
        for (int i = 0; i < queue.Length; i++)
        {
            iceMap[i, 0] = queue[i] > 0 ? 1 : 0;
            if(holeGenerationPause > 0)
            {
                iceMap[i, 0] = 0;
            }
            if (queue[i] > 0) queue[i]--; 
        }
        if(holeGenerationPause > 0)holeGenerationPause--;

        //logIceMap();
        //CheckFeasibility();
        InstantiateIce();
    }
    bool CheckFeasibility()
    {
        for (int i = 0; i < feasibility.Length; i++)
        {
            if(iceMap[i, 0] == 1)
            {
                feasibility[i] = 1; 
            }
            int inital = -1; 
            int previous = -1; 
            for (int j = 0; j < 5; j++)
            {
                if (feasibility[j] == 1)
                {
                    if (previous == -1) inital = j; 
                    previous = j;
                    continue;
                }


                if (j == 4 || previous + 1 == j && feasibility[j] != 1)
                {
                    // ended 
                    bool empty = true;
                    if (inital >= 0 && previous >= 0)
                    {
                        for (int k = inital; k < previous + 1; k++)
                        {
                            if (iceMap[k, 0] == 1) empty = false;
                        }
                    }
                    if (empty && inital >= 0 && previous >= 0)
                    {
                        for (int k = inital; k < previous + 1; k++)
                        {
                            feasibility[k] = 0;
                        }
                    }
                }
                else if (feasibility[0] == 1 && iceMap[0, 0] == 0) feasibility[0] = 0;
                else if (feasibility[4] == 1 && iceMap[0, 0] == 0) feasibility[4] = 0;
            }   
        }
        string shit = $"[{feasibility[0]},{feasibility[1]},{feasibility[2]},{feasibility[3]},{feasibility[4]}]";
        //Debug.Log(shit);
        bool gonnaKill = true;
        foreach (var item in feasibility)
        {
            if (item == 0)
            {
                gonnaKill = false;
                break;
            }
        }
        if (gonnaKill)
        {
            for (int i = 0; i < 5; i++)
            {
                queue[i] = 0;
                iceMap[i, 0] = 0;
                feasibility[i] = 0;

            }
            holeGenerationPause += 3;
            Debug.LogWarning("Was gonna kill!");
            Debug.LogError("stop");
            return false;
        }
        return true; 
    }

    // DO NOT TRY TO OPTIMIZE THIS, YOU WILL ONLY WASTE YOUR TIME, there are simply too many options 
    void InstantiateIce()
    {
        //logIceMap();
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
        GameObject newIceObject = GetPooledObject(iceObject);
        newIceObject.transform.position = position - Vector3.forward * speed;
/*        if (spawningExtraIce)
        {
            newIceObject.transform.position = position - Vector3.forward * speed + Vector3.forward;
        }*/
        newIceObject.transform.rotation = orientation;
        if (last)
        {
            previousIceObject = newIceObject;
/*            if (spawningExtraIce)
            {
                spawningExtraIce = false;
                StartCoroutine(ReenableOnce()); 
            }*/
        }
    }
/*    IEnumerator ReenableOnce()
    {
        yield return new WaitForFixedUpdate();
        once = false; 
    }*/

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



    void SpawnEnviroment(GameObject prefab,float offset = 0)
    {

        Vector3 position = previousBridgeObject ? 
            previousBridgeObject.transform.position + 
            Vector3.forward * previousBridgeObject.GetComponent<Renderer>().bounds.size.z/2 +
            Vector3.forward * prefab.GetComponent<Renderer>().bounds.size.z/2:

            Vector3.forward * spawnDistance;

        GameObject newObject = GetPooledObject(prefab);
        newObject.transform.position = position - Vector3.forward * speed;
        newObject.transform.rotation = Quaternion.Euler(-90, 0, 0); 
        previousBridgeObject = newObject;
    }
    void SpawnBridge()
    {
        holeGenerationPause += 15; 
        SpawnEnviroment(enviromentAssets.bridge);
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
        GameObject newObject = GetPooledObject(openining);
        newObject.transform.position = position;
        newObject.transform.rotation = Quaternion.Euler(-90, flip ? 0 : 180, 0); 
    }

    // Update is called once per frame
    void Update()
    {
/*        if (Input.GetKeyDown(KeyCode.O))
        {
            once = false; 
        }*/
    }
    private void FixedUpdate()
    {

        if (spawnDistance - previousBridgeObject.transform.position.z > 0)
        {
            int random = Random.Range(0, 10);
            if (random < bridgeSpawnRate) SpawnBridge();
            else SpawnEnviroment(enviromentAssets.bridgeSidePath); 
        }

        if (spawnDistance - previousIceObject.transform.position.z > 0)
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
}


public enum Rot
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
