using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Scroller : MonoBehaviour
{
    private List<TranslateObject> _planes = new List<TranslateObject>();


    [SerializeField] private BridgeRoadAssets bridgeRoadAssets;

    
    [SerializeField] private IceAssets iceAssets;

    
    [SerializeField] private ObstacleAssets obstacleAssets;

    [SerializeField] List<GameObject> houseAssets = new List<GameObject>(); 

    [SerializeField] private CollectableAssets collectableAssets; 
    [SerializeField] GameObject waterAsset; 


    [SerializeField] private bool tutorialMode = false; 
    [SerializeField] private bool singleLaneMode = false;
    [SerializeField] private bool breakTime = false;

    [Range(1, 3)]
    [SerializeField] public int maxLaneChange = 1;

    [Range(0, 3)]
    [SerializeField] public int laneChangeGap = 1;

    [Range(0.5f, 2)]
    [SerializeField] private float laneChangeFrequency = 1;

    [Range(0, 10)]
    [SerializeField] private int bridgeSpawnRate = 2;

    [Range(0, 10)]
    [SerializeField] private int obstacleSpawnRate = 2;
    

    [Range(0, 1)]
    [SerializeField] private float collectableSpawnRate = 0.5f;
    [SerializeField] private float collectableSpawnHeight = 2;
    [Range(0,6)]
    [SerializeField] private int obstacleSpawnOffset; 

    [SerializeField]
    private ObstacleOptions obstacleOptions; 

    [Space(30f)]
    [Range(0.05f,2)]
    [SerializeField] public float speed = 0.1f;

    [Range(10, 100)]
    [SerializeField] float spawnDistance = 10;

    [Range(10, 50)]
    [SerializeField] public float despawnDistance = 10;

    [Range(0,5)]
    [SerializeField] private float tutorialNoticeDistance = 1; 
    [SerializeField] private GameObject tutorialPrefab; 

    float timer = 0;

    GameObject previousBridgeObject; 
    GameObject previousHouseObject; 
    GameObject previousWaterObject; 
    GameObject previousIceObject;

    private int[] queue = new int[5];
    private int[,] iceMap = new int[5, 3];

    private int holeGenerationPause = 0;

    private int alwaysOpenLane = 2;
    private int changeOpenLaneIn = 5;

    private int spawnIceCallCount; 
    private int spawnIceCallCountSection; 
    private int initialSpawnIceCallCountSection; 

    [SerializeField] UiSettingsManager uiSettingsManager; 
/*    private bool spawningExtraIce;
    bool once = false; */



    private Dictionary<GameObject, Queue<GameObject>> objectPools;
    private Dictionary<GameObject, GameObject> poolHolders;

    public void SetLevels(List<LevelSection> sections){

        if(sections.Count > 0){
            SetLevelSection(sections, 0); 
        } 
    }
    void SetLevelSection(List<LevelSection> sections, int index){
        LevelSection section = sections[index]; 
        index++; 

        uiSettingsManager.SetCurrentPlayingSection(section); 

        tutorialMode = section.tutorialMode; 
        singleLaneMode = section.singleLaneMode; 
        maxLaneChange = section.maxLaneChange;
        laneChangeGap = section.laneChangeGap; 
        laneChangeFrequency = section.laneChangeFrequency; 
        bridgeSpawnRate = section.bridgeSpawnRate; 
        obstacleOptions = section.obstacleOptions;
        obstacleSpawnRate = section.obstacleSpawnRate; 
        speed = section.speed; 
        breakTime = section.breakTime; 


        if(index < sections.Count){
            StartCoroutine(WaitForIceSections(sections[index-1].distance,() => SetLevelSection(sections, index))); 
        }
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

        //Timer.OnExeciseRest += Break; 

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


        InitializePool(bridgeRoadAssets.bridgeSidePath);
        InitializePool(bridgeRoadAssets.bridge);
        InitializePool(bridgeRoadAssets.bridgeClosed);
        InitializePool(bridgeRoadAssets.bridgeLPieceOpening);
        InitializePool(bridgeRoadAssets.bridgeOpeningConnector);
        InitializePool(bridgeRoadAssets.bridgeSingleOpening);

        InitializePool(obstacleAssets.IceDebris);
        InitializePool(obstacleAssets.OrangeCone);
        InitializePool(obstacleAssets.Sledge);

        foreach(GameObject house in houseAssets){
            InitializePool(house); 
        }

        InitializePool(collectableAssets.coco);
        InitializePool(collectableAssets.hotdog);
        InitializePool(collectableAssets.soup);

        InitializePool(waterAsset); 
        InitializePool(tutorialPrefab); 


        SpawnBridgeRoads(bridgeRoadAssets.bridgeSidePath);
        SpawnRandomHouse();
        SpawnWater(waterAsset);
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
        bool laneChanged = false;
        int previousLane = -1; 

        if (changeOpenLaneIn <= 0 && holeGenerationPause <= 0)
        {
            previousLane = alwaysOpenLane; 
            changeOpenLaneIn = Random.Range((int)(5 * (1/laneChangeFrequency)), (int)(15 * 1/laneChangeFrequency));
            alwaysOpenLane = Random.Range(Mathf.Clamp(alwaysOpenLane - maxLaneChange, 0,5), Mathf.Clamp(alwaysOpenLane + maxLaneChange +1, 0, 5));

            changeOpenLaneIn += laneChangeGap;

            holeGenerationPause = laneChangeGap > 1? laneChangeGap: 0;

            laneChangeGap = laneChangeGap == 0 ? Mathf.Abs(previousLane - alwaysOpenLane) > 1 ? 1 : laneChangeGap : laneChangeGap; 

            laneChanged = true;

            if(tutorialMode && previousLane != alwaysOpenLane){
                if(previousLane > alwaysOpenLane)SpawnTutorial(Tutorialtype.MoveLeft,tutorialPrefab);
                else SpawnTutorial(Tutorialtype.MoveRight,tutorialPrefab);
            }


            for (int i = 0; i < 5; i++)
            {
                queue[i] = 0;
            }

            if (changeOpenLaneIn > 5 && !breakTime)
            {
                QueueObstacles(changeOpenLaneIn);
                QueueCollectables(changeOpenLaneIn);
            }
        }


        int lane = Random.Range(0, 5);
        int spawn = Random.Range(0, 10);
        int length = Random.Range(1, 7);

        if (singleLaneMode)
        {
            for (int i = 0; i < 5; i++)
            {
                if (laneChanged && laneChangeGap != 0)
                {
                    int small = previousLane;
                    int big = alwaysOpenLane;
                    if (previousLane > alwaysOpenLane)
                    {
                        small = alwaysOpenLane;
                        big = previousLane;
                    }

                    if (i >= small && i <= big)
                    {
                        continue;
                    }
                }
                if (i != alwaysOpenLane && queue[i] <= 0) queue[i] = changeOpenLaneIn;
            }
        }
        else
        {
            if (spawn < 7 && lane != alwaysOpenLane )
            {
                if (laneChanged)
                {

                }
                if (queue[lane] <= 0)
                {
                    queue[lane] = length;
                }
            }
        }
        if(breakTime){
            for (int i = 0; i < 5; i++)
            {
                queue[i] = 0; 
            }
        }



        IncrementIce();
        changeOpenLaneIn--;

        spawnIceCallCount--; 
        spawnIceCallCountSection--; 


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
    void SpawnRandomHouse(){
        int index = Random.Range(0,houseAssets.Count); 
        if(houseAssets.Count >  0){
            SpawnHouse(houseAssets[index]); 
        }
    }

    void SpawnHouse(GameObject prefab){

        Vector3 position = previousHouseObject ? 
            previousHouseObject.transform.position + 
            Vector3.forward * previousHouseObject.GetComponent<Renderer>().bounds.size.z/2 +
            Vector3.forward * prefab.GetComponent<Renderer>().bounds.size.z/2:

            Vector3.forward * spawnDistance;

        GameObject newObject = GetPooledObject(prefab);
        newObject.transform.position = position - Vector3.forward * speed;
        newObject.transform.rotation = Quaternion.Euler(-90, 0, 0); 
        previousHouseObject = newObject;
    }
    void SpawnWater(GameObject prefab){
        Vector3 position = previousWaterObject ? 
            previousWaterObject.transform.position + 
            Vector3.forward * previousWaterObject.GetComponent<Renderer>().bounds.size.z/2 +
            Vector3.forward * prefab.GetComponent<Renderer>().bounds.size.z/2:

            Vector3.forward * spawnDistance;

        GameObject newObject = GetPooledObject(prefab);
        newObject.transform.position = position - Vector3.forward * speed;
        newObject.transform.rotation = Quaternion.Euler(-90, 0, 0); 
        previousWaterObject = newObject;
    }

    private void QueueCollectables(int freeSpace){
        int spawn = Random.Range(0, 1);
        if (spawn >= collectableSpawnRate) return;

        int type = Random.Range(0, 3); // Incease this if added more Collectables 
        int side = Random.Range(0,2);

        if(alwaysOpenLane == 4 && side == 0)side = 1;
        else if(alwaysOpenLane == 0 && side == 1) side = 0; 


        int startPosition = Random.Range(obstacleSpawnOffset, freeSpace);
        int amount = Random.Range(1, freeSpace - startPosition); 

        Vector3 offset = Vector3.up * collectableSpawnHeight + (side==0?Vector3.right:-Vector3.right);  

        switch(type){
            case 0:
                StartCoroutine(WaitForIce(startPosition, () => SpawnMultipleCollectables(collectableAssets.coco,amount,alwaysOpenLane,Quaternion.identity,tag = "CollectableCoco", offset)));
            break;
            case 1:
                StartCoroutine(WaitForIce(startPosition, () => SpawnMultipleCollectables(collectableAssets.hotdog,amount,alwaysOpenLane,Quaternion.identity,tag = "CollectableHotdog", offset)));
            break;
            case 2:
                StartCoroutine(WaitForIce(startPosition, () => SpawnMultipleCollectables(collectableAssets.soup,amount,alwaysOpenLane,Quaternion.identity, tag = "CollectableSoup", offset)));
            break;
        }
    }

    private void QueueObstacles(int freeSpace)
    {
        List<int> options = obstacleOptions.GetOptionAmount(); 
        int spawn = Random.Range(0, 10);
        if (spawn >= obstacleSpawnRate || options.Count <= 0) return;

        int type = Random.Range(0, options.Count);
        type = options[type]; 

        int startPosition = Random.Range(obstacleSpawnOffset, freeSpace);



        int amount = Random.Range(1, freeSpace - startPosition); 
        switch (type)
        {
            case (int)ObstacleType.Jump:
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    StartCoroutine(WaitForIce(startPosition, () => SpawnSingleObstacle(obstacleAssets.IceDebris, alwaysOpenLane, GetRot(Rot.TopRight))));
                    if(tutorialMode) SpawnTutorial(Tutorialtype.Jump,tutorialPrefab); 
                }
                else
                {
                    StartCoroutine(WaitForIce(startPosition, () => SpawnSingleObstacle(obstacleAssets.Sledge, alwaysOpenLane, GetRot(Rot.TopRight))));
                    if(tutorialMode) SpawnTutorial(Tutorialtype.Jump,tutorialPrefab); 
                }
                break;
            case (int)ObstacleType.LeftLegUp:
                StartCoroutine(WaitForIce(startPosition, () => SpawnMultipleObstacles(obstacleAssets.OrangeCone, amount, alwaysOpenLane, GetRot(Rot.BottomLeft),"LeftLegUp")));
                if(tutorialMode) SpawnTutorial(Tutorialtype.LeftLegUp,tutorialPrefab); 
                break;
            case (int)ObstacleType.RightLegUp:
                StartCoroutine(WaitForIce(startPosition, () => SpawnMultipleObstacles(obstacleAssets.OrangeCone, amount, alwaysOpenLane, GetRot(Rot.TopRight))));
                if(tutorialMode) SpawnTutorial(Tutorialtype.RightLegUp,tutorialPrefab); 
                break;
        }
    }
    IEnumerator WaitForIce(int numberOfCalls, Action method)
    {
        spawnIceCallCount = numberOfCalls; 
        yield return new WaitUntil(() => spawnIceCallCount <= 0);

        if(holeGenerationPause == 0)method?.Invoke(); 
    }

    IEnumerator WaitForIceSections(int numberOfCalls, Action method)
    {
        spawnIceCallCountSection = numberOfCalls; 
        initialSpawnIceCallCountSection = spawnIceCallCountSection; 
        yield return new WaitUntil(() => spawnIceCallCountSection <= 0);

        method?.Invoke(); 
    }
    void SpawnSingleObstacle(GameObject prefab, int lane, Quaternion orientation)
    {
        GameObject newObject = GetPooledObject(prefab);

        Vector3 position = Vector3.forward * previousIceObject.transform.position.z  + Vector3.forward *2 - Vector3.forward * speed + Vector3.right * (lane-2) * 2;

        newObject.transform.position = position;
        newObject.transform.rotation = orientation; 
    }

    void SpawnMultipleCollectables(GameObject prefab, int amount, int lane, Quaternion orientation, string tag, Vector3 offset)
    {
        SpawnMultipleObjects(prefab,amount,lane,orientation,tag, offset);
    }
    void SpawnMultipleObstacles(GameObject prefab, int amount, int lane, Quaternion orientation, string tag = "")
    {
        SpawnMultipleObjects(prefab,amount,lane,orientation,tag);
    }
    void SpawnMultipleObjects(GameObject prefab, int amount, int lane, Quaternion orientation, string tag = ""){
        SpawnMultipleObjects(prefab, amount, lane, orientation,tag,Vector3.zero); 
    }
    void SpawnMultipleObjects(GameObject prefab, int amount, int lane, Quaternion orientation, string tag, Vector3 offset)
    {
        if (amount == 0) return; 

        GameObject newObject = GetPooledObject(prefab);
        Vector3 position = Vector3.forward * previousIceObject.transform.position.z + Vector3.forward * 2 - Vector3.forward * speed + Vector3.right * (lane - 2) * 2;
        position += offset; 
        newObject.transform.position = position;
        newObject.transform.rotation = orientation;

        if(tag != "") newObject.tag = tag; 

        StartCoroutine(WaitForIce(1, () => SpawnMultipleObjects(prefab, amount - 1,lane, orientation, tag, offset)));
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
        InstantiateIce();

    }

    void SpawnTutorial(Tutorialtype type, GameObject prefab){

        GameObject newTutorial = GetPooledObject(prefab); 

        newTutorial.transform.position = Vector3.forward * (spawnDistance - tutorialNoticeDistance);


        switch(type){
            case Tutorialtype.MoveLeft:
                newTutorial.tag = "TutorialMoveLeft";
            break;
            case Tutorialtype.MoveRight:
                newTutorial.tag = "TutorialMoveRight";
            break;

            case Tutorialtype.Jump:
                newTutorial.tag = "TutorialJump";
            break;

            case Tutorialtype.RightLegUp:
                newTutorial.tag = "TutorialRightLegUp";
            break;

            case Tutorialtype.LeftLegUp:
                newTutorial.tag = "TutorialLeftLegUp";
            break;
            
            case Tutorialtype.Crouch:
                newTutorial.tag = "TutorialCrouch";
            break;
        }


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

            bool isLast = lane == 4; 

            if (prevX == 0 && current == 1 && nextX == 0 &&
                prevY == 0 && nextY == 0) SpawnIceObject(position,iceAssets.IceOPiece, GetRot(Rot.BottomLeft), isLast); // Single Hole
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 0 && nextY == 0) SpawnIceObject(position, iceAssets.IceUPiece, GetRot(Rot.TopLeft), isLast); // U piece looking to the left
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 0) SpawnIceObject(position, iceAssets.IceUPiece, GetRot(Rot.BottomRight), isLast); // U piece looking to the right
            else if (prevX == 0 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 0) SpawnIceObject(position, iceAssets.IceUPiece, GetRot(Rot.BottomLeft), isLast); // U piece looking behind
            else if (prevX == 0 && current == 1 && nextX == 0 &&
                    prevY == 0 && nextY == 1) SpawnIceObject(position, iceAssets.IceUPiece, GetRot(Rot.TopRight), isLast); // U piece looking forwards
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                   prevY == 1 && nextY == 0 &&
                   dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceLWithCornerPiece, GetRot(Rot.BottomLeft), isLast); // L piece with Corner looking left behind
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 0 && nextY == 1 &&
                    dTopLeft == 0) SpawnIceObject(position, iceAssets.IceLWithCornerPiece, GetRot(Rot.TopLeft), isLast); // L piece with Corner looking left forward
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0 &&
                    dBottomRight == 0) SpawnIceObject(position, iceAssets.IceLWithCornerPiece, GetRot(Rot.BottomRight), isLast); // L piece with Corner looking right behind
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1 &&
                    dTopRight == 0) SpawnIceObject(position, iceAssets.IceLWithCornerPiece, GetRot(Rot.TopRight), isLast); // L piece with Corner looking right forward

            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 0) SpawnIceObject(position, iceAssets.IceLPiece, GetRot(Rot.BottomLeft), isLast); // L piece looking left behind
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 0 && nextY == 1) SpawnIceObject(position, iceAssets.IceLPiece, GetRot(Rot.TopLeft), isLast); // L piece looking left forward
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0) SpawnIceObject(position, iceAssets.IceLPiece, GetRot(Rot.BottomRight), isLast); // L piece looking right behind
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1) SpawnIceObject(position, iceAssets.IceLPiece, GetRot(Rot.TopRight), isLast); // L piece looking right forward

            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceIWith2CornerPiece, GetRot(Rot.BottomLeft), isLast); // I  with 2 corner piece vertical right
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0) SpawnIceObject(position, iceAssets.IceIWithCornerRightPiece, GetRot(Rot.BottomLeft), isLast); // I  with corner Right piece vertical right
            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceIWithCornerLeftPiece, GetRot(Rot.BottomLeft), isLast); // I  with corner Left piece vertical right
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomRight == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.IceIWith2CornerPiece, GetRot(Rot.TopRight), isLast); // I with 2 corner piece vertical left
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomRight == 0) SpawnIceObject(position, iceAssets.IceIWithCornerRightPiece, GetRot(Rot.TopRight), isLast); // I with corner right piece vertical left
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0) SpawnIceObject(position, iceAssets.IceIWithCornerLeftPiece, GetRot(Rot.TopRight), isLast); // I with corner left piece vertical left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0 &&
                    dBottomLeft == 0 && dBottomRight == 0) SpawnIceObject(position, iceAssets.IceIWith2CornerPiece, GetRot(Rot.BottomRight), isLast); // I with 2 corner piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0 &&
                    dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceIWithCornerRightPiece, GetRot(Rot.BottomRight), isLast); // I with corner right piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0 &&
                    dBottomRight == 0) SpawnIceObject(position, iceAssets.IceIWithCornerLeftPiece, GetRot(Rot.BottomRight), isLast); // I with corner left piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1 &&
                    dTopLeft == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.IceIWith2CornerPiece, GetRot(Rot.TopLeft), isLast); // I with 2 corner  piece horizontal bottom
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1 &&
                    dTopRight == 0) SpawnIceObject(position, iceAssets.IceIWithCornerRightPiece, GetRot(Rot.TopLeft), isLast); // I with corner right piece horizontal bottom
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1 &&
                    dTopLeft == 0) SpawnIceObject(position, iceAssets.IceIWithCornerLeftPiece, GetRot(Rot.TopLeft), isLast); // I with corner left piece horizontal bottom

            else if (prevX == 1 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1) SpawnIceObject(position, iceAssets.IceIPiece, GetRot(Rot.BottomLeft), isLast); // I piece vertical right
            else if (prevX == 0 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1) SpawnIceObject(position, iceAssets.IceIPiece, GetRot(Rot.TopRight), isLast); // I piece vertical left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 0) SpawnIceObject(position, iceAssets.IceIPiece, GetRot(Rot.BottomRight), isLast); // I piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 1) SpawnIceObject(position, iceAssets.IceIPiece, GetRot(Rot.TopLeft), isLast); // I piece horizontal bottom

            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0 && dTopRight == 0 && dBottomRight == 0) SpawnIceObject(position, iceAssets.Ice4CornerPiece, GetRot(Rot.BottomLeft), isLast); // C small 4 corner piece
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0 && dBottomRight ==0) SpawnIceObject(position, iceAssets.Ice3CornerPiece, GetRot(Rot.BottomLeft), isLast); // C small 3 corner piece bottom left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomLeft == 0 && dBottomRight == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.Ice3CornerPiece, GetRot(Rot.BottomRight), isLast); // C small 3 corner piece bottom right
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.Ice3CornerPiece, GetRot(Rot.TopLeft), isLast); // C small 3 corner piece top left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomRight == 0 && dTopRight == 0) SpawnIceObject(position, iceAssets.Ice3CornerPiece, GetRot(Rot.TopRight), isLast); // C small 3 corner piece top right

            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0 && dTopLeft == 0) SpawnIceObject(position, iceAssets.Ice2CornerPiece, GetRot(Rot.TopRight), isLast); // C small 2 corner piece horizontal top
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomRight == 0 && dBottomLeft == 0) SpawnIceObject(position, iceAssets.Ice2CornerPiece, GetRot(Rot.BottomLeft), isLast); // C small 2 corner piece horizontal bottom
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0 && dBottomRight == 0) SpawnIceObject(position, iceAssets.Ice2CornerPiece, GetRot(Rot.BottomRight), isLast); // C small 2 corner piece vertical right
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomLeft == 0) SpawnIceObject(position, iceAssets.Ice2CornerPiece, GetRot(Rot.TopLeft), isLast); // C small 2 corner piece vertical left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0 && dBottomRight == 0) SpawnIceObject(position, iceAssets.Ice2CornerDiagonalPiece, GetRot(Rot.TopLeft), isLast); // C small 2 corner diagonal piece top left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0 && dBottomLeft == 0) SpawnIceObject(position, iceAssets.Ice2CornerDiagonalPiece, GetRot(Rot.TopRight), isLast); // C small 2 corner diagonal piece top right

            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopLeft == 0) SpawnIceObject(position, iceAssets.IceCornerPiece, GetRot(Rot.TopLeft), isLast); // C small corner piece top left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dTopRight == 0) SpawnIceObject(position, iceAssets.IceCornerPiece, GetRot(Rot.TopRight), isLast); // C small corner piece top right
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomLeft == 0) SpawnIceObject(position, iceAssets.IceCornerPiece, GetRot(Rot.BottomLeft), isLast); // C small corner piece bottom left
            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 1 && nextY == 1 &&
                    dBottomRight == 0) SpawnIceObject(position, iceAssets.IceCornerPiece, GetRot(Rot.BottomRight), isLast); // C small corner piece bottom right

            else if (prevX == 1 && current == 1 && nextX == 1 &&
                    prevY == 0 && nextY == 0) SpawnIceObject(position, iceAssets.IceHPiece, GetRot(Rot.TopLeft), isLast); // H piece Horizontal
            else if (prevX == 0 && current == 1 && nextX == 0 &&
                    prevY == 1 && nextY == 1) SpawnIceObject(position, iceAssets.IceHPiece, GetRot(Rot.BottomLeft), isLast); // H piece Vertical
            else if (current == 0) SpawnIceObject(position, iceAssets.IceFlat, GetRot(Rot.BottomLeft), isLast); // Solid
            else if (current == 1) SpawnIceObject(position, iceAssets.IceEmptyPiece, GetRot(Rot.BottomLeft), isLast); // Empty 
        }
    }
    void SpawnIceObject(Vector3 position, GameObject iceObject, Quaternion orientation , bool last = false)
    {
        GameObject newIceObject = GetPooledObject(iceObject);
        newIceObject.transform.position = position - Vector3.forward * speed ;
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



    void SpawnBridgeRoads(GameObject prefab,float offset = 0)
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
        SpawnBridgeRoads(bridgeRoadAssets.bridge);
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
        if(breakTime){
            for (int i = 0; i < 5; i++)
            {
                paths[i] = 1; 
            }
        }

        for (int i = 0; i < paths.Length; i++)
        {

            int previous = i -1 < 0? 0: paths[i-1]; 
            int current = paths[i];
            int next = i + 1 < paths.Length ? paths[i + 1] : 0;
            Vector3 position = previousBridgeObject.transform.position + Vector3.right * (i - 2)*2;

            if (previous == 0 && current == 1 && next == 0) SpawnBridgeOpening(position, bridgeRoadAssets.bridgeSingleOpening); // Single opening 
            else if (previous == 1 && current == 1 && next == 0) SpawnBridgeOpening(position, bridgeRoadAssets.bridgeLPieceOpening);// L piece flipped
            else if (previous == 0 && current == 1 && next == 1) SpawnBridgeOpening(position, bridgeRoadAssets.bridgeLPieceOpening, true);// L piece 
            else if (previous == 1 && current == 1 && next == 1) SpawnBridgeOpening(position, bridgeRoadAssets.bridgeOpeningConnector);// connector piece 
            else if (current == 0) SpawnBridgeOpening(position, bridgeRoadAssets.bridgeClosed);// blocked
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

    public void UpdateCurrentSectionValues(LevelSection section){

        if(breakTime){
            int distanceDifference = initialSpawnIceCallCountSection - section.breakDistance; 
            spawnIceCallCountSection -= distanceDifference; 
            initialSpawnIceCallCountSection = spawnIceCallCountSection;
        }
        if(!breakTime){
            int distanceDifference = initialSpawnIceCallCountSection - section.distance; 
            spawnIceCallCountSection -= distanceDifference; 
            initialSpawnIceCallCountSection = spawnIceCallCountSection;

            tutorialMode = section.tutorialMode; 
            singleLaneMode = section.singleLaneMode; 
            obstacleOptions = section.obstacleOptions;

            maxLaneChange = section.maxLaneChange;
            laneChangeGap = section.laneChangeGap;
            laneChangeFrequency = section.laneChangeFrequency; 
            bridgeSpawnRate = section.bridgeSpawnRate;
            obstacleSpawnRate = section.obstacleSpawnRate;
            speed = section.speed;  
        }
    }
    public void SkipCurrentSection(){
        spawnIceCallCountSection = 0; 
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
            else SpawnBridgeRoads(bridgeRoadAssets.bridgeSidePath); 
        }

        if (spawnDistance - previousIceObject.transform.position.z > 0)
        {
            SpawnIce();
        }

        if(spawnDistance - previousHouseObject.transform.position.z > 0){
            SpawnRandomHouse(); 
        }
        if(spawnDistance - previousWaterObject.transform.position.z > 0){
            SpawnWater(waterAsset); 
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
public class BridgeRoadAssets
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

[Serializable]
public class ObstacleAssets
{
    public GameObject OrangeCone;
    public GameObject IceDebris;
    public GameObject Sledge;
}



[Serializable]
public class CollectableAssets{
    public GameObject coco; 
    public GameObject hotdog; 
    public GameObject soup; 


}


[Serializable]
public class ObstacleOptions
{
    public bool jump;
    public bool rightLeg;
    public bool leftLeg; 
    public bool crouch; // MISSING IMPLEMENTATION 

    public List<int> GetOptionAmount()
    {
        List<int> options = new List<int>();
        if (jump) options.Add(0); 
        if (rightLeg) options.Add(1);
        if (leftLeg) options.Add(2);
        return options; 
    }

    public ObstacleOptions(bool pJump = false, bool pRightLeg = false, bool pLeftLeg = false, bool pCrouch = false){
        jump = pJump; 
        rightLeg = pRightLeg; 
        leftLeg = pLeftLeg; 
    }
}


public enum Rot
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

public enum ObstacleType
{
    Jump = 0,
    RightLegUp = 1,
    LeftLegUp = 2
}

public enum Tutorialtype
{
    MoveLeft = 0,
    MoveRight = 1,
    Jump = 2,
    RightLegUp = 3,
    LeftLegUp = 4,
    Crouch = 5
}
