using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject[] roomsPrefabs;
    [SerializeField] private GameObject[] enemiesPrefabs;
    [SerializeField] private GameObject[] bossPrefabs;
    [SerializeField] private GameObject[] itemsToDropPrefabs;
    [SerializeField] private GameObject[] treasureItemsPrefabs;
    [SerializeField] private GameObject keyTreasureRoomPrefab;

    private List<Room> rooms;
    private GameController game;
    private int currentRooms;
    private int maxRooms;
    private bool needKeyTreasure;
    private int doorNumberToDropKey;

    private void Start()
    {
        rooms = new List<Room>();
        game = UnityEngine.Object.FindObjectOfType<GameController>();
        currentRooms = 0;
        int level = game.GetLevel();
        maxRooms = Mathf.RoundToInt(Mathf.Sqrt(level) * 7.0f);
        if (level > 1)
        {
            needKeyTreasure = true;
            doorNumberToDropKey = UnityEngine.Random.Range(1, maxRooms - 1);
        }
        GameObject startRoom = Instantiate(roomsPrefabs[0], transform.position, Quaternion.identity);
        StartCoroutine(Generate(startRoom));
        StartCoroutine(StartGame(startRoom));
    }

    private IEnumerator Generate(GameObject roomObject)
    {
        rooms.Add(roomObject.GetComponent<Room>());
        Room room = roomObject.GetComponent<Room>();
        int maxNumberOfRooms;
        if (currentRooms == 0)
        {
            maxNumberOfRooms = 4;
        }
        else
        {
            room.InstantiateEnemies();
            maxNumberOfRooms = Mathf.Min(3, maxRooms - currentRooms);
            yield return new WaitWhile(() => room.GetOriginDoorInRoom() == null);
        }
        if (needKeyTreasure && currentRooms > doorNumberToDropKey)
        {
            room.AddItemToDrop(keyTreasureRoomPrefab);
            needKeyTreasure = false;
        }
        int numberOfRooms = UnityEngine.Random.Range(1, maxNumberOfRooms + 1);
        Door[] shuffledDoorPoints = room.GetDoorPoints().OrderBy(doorPoint => UnityEngine.Random.value).ToArray();
        currentRooms += numberOfRooms;
        float maxDistance = 4f;
        for (int i = 0; i < shuffledDoorPoints.Length; i++)
        {
            shuffledDoorPoints[i].SetType(Door.DoorType.Normal);
            if (i < numberOfRooms)
            {
                yield return new WaitUntil(delegate () { return room.IsReady(); });
                GameObject roomPointFromDoorPoint = room.GetRoomPointFromDoorPoint(shuffledDoorPoints[i].name);
                Vector2 direction = roomPointFromDoorPoint.transform.position - shuffledDoorPoints[i].transform.position;
                RaycastHit2D[] results = new RaycastHit2D[3];
                if (shuffledDoorPoints[i].GetComponent<Collider2D>().Raycast(direction, results, maxDistance) <= 1)
                {
                    if (roomPointFromDoorPoint != null)
                    {
                        string originDoor = shuffledDoorPoints[i].gameObject.name.Replace("DoorPoint", "");
                        GameObject newRoom = Instantiate(GetNormalRoom(), roomPointFromDoorPoint.transform.position, Quaternion.identity);
                        newRoom.GetComponent<Room>().SetOriginDoor(originDoor);
                        newRoom.GetComponent<Room>().GetOriginDoorInRoom().SetType(Door.DoorType.Normal);
                        if (UnityEngine.Random.Range(-2, 2) > 0)
                        {
                            int randomItemIndex = UnityEngine.Random.Range(0, itemsToDropPrefabs.Length);
                            newRoom.GetComponent<Room>().AddItemToDrop(itemsToDropPrefabs[randomItemIndex]);
                        }
                        StartCoroutine(Generate(newRoom));
                    }
                }
                else
                {
                    currentRooms--;
                    shuffledDoorPoints[i].gameObject.SetActive(false);
                }
            }
            else
            {
                shuffledDoorPoints[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator StartGame(GameObject startRoom)
    {
        Player player = UnityEngine.Object.FindObjectOfType<Player>();
        player.gameObject.SetActive(false);
        yield return new WaitWhile(delegate () { return currentRooms < maxRooms; });
        GenerateBossRoom();
        GenerateTreasureRoom();
        game.StartCoroutine("FadeOutWaitingStartImage");
        yield return new WaitUntil(delegate () { return game.GetStartedLevel(); });
        player.gameObject.SetActive(true);
        player.GetComponent<Collider2D>().enabled = true;
        player.SetActiveCollider(true);
        startRoom.GetComponent<Room>().EnterFocus();
    }

    public void GenerateTreasureRoom()
    {
        List<Room> shuffledRooms = rooms.OrderBy(room => UnityEngine.Random.value).ToList();
        bool generated = false;
        float distance = 4f;
        foreach (Room room in shuffledRooms)
        {
            foreach (Door door in room.GetDoorPoints())
            {
                if (!door.isActiveAndEnabled && !generated)
                {
                    GameObject roomPointFromDoorPoint = room.GetRoomPointFromDoorPoint(door.name);
                    Vector2 direction = roomPointFromDoorPoint.transform.position - door.transform.position;
                    RaycastHit2D[] results = new RaycastHit2D[3];
                    if (door.GetComponent<Collider2D>().Raycast(direction, results, distance) <= 1)
                    {
                        door.gameObject.SetActive(true);
                        door.SetType(Door.DoorType.Treasure);
                        if (roomPointFromDoorPoint != null)
                        {
                            string originDoor = door.gameObject.name.Replace("DoorPoint", "");
                            GameObject newRoom = Instantiate(GetTreasureRoom(), roomPointFromDoorPoint.transform.position, Quaternion.identity);
                            newRoom.GetComponent<Room>().SetOriginDoor(originDoor);
                            newRoom.GetComponent<Room>().GetOriginDoorInRoom().SetType(Door.DoorType.Treasure);
                            Door[] doorPoints = newRoom.GetComponent<Room>().GetDoorPoints();
                            foreach (Door d in doorPoints)
                            {
                                d.gameObject.SetActive(false);
                            }
                            int randomTreasureIndex = UnityEngine.Random.Range(0, treasureItemsPrefabs.Length);
                            Instantiate(treasureItemsPrefabs[randomTreasureIndex], newRoom.transform.position, Quaternion.identity);
                            generated = true;
                        }
                    }
                }
            }
        }
    }

    public void GenerateBossRoom()
    {
        List<Room> shuffledRooms = rooms.OrderBy(room => UnityEngine.Random.value).ToList();
        bool generated = false;
        float distance = 4f;
        foreach (Room room in shuffledRooms)
        {
            foreach (Door door in room.GetDoorPoints())
            {
                if (!door.isActiveAndEnabled && !generated)
                {
                    GameObject roomPointFromDoorPoint = room.GetRoomPointFromDoorPoint(door.name);
                    Vector2 direction = roomPointFromDoorPoint.transform.position - door.transform.position;
                    RaycastHit2D[] results = new RaycastHit2D[3];
                    if (door.GetComponent<Collider2D>().Raycast(direction, results, distance) <= 1)
                    {
                        door.gameObject.SetActive(true);
                        door.SetType(Door.DoorType.Boss);
                        if (roomPointFromDoorPoint != null)
                        {
                            string originDoor = door.gameObject.name.Replace("DoorPoint", "");
                            GameObject newRoom = Instantiate(GetBossRoom(), roomPointFromDoorPoint.transform.position, Quaternion.identity);
                            newRoom.GetComponent<Room>().SetOriginDoor(originDoor);
                            newRoom.GetComponent<Room>().GetOriginDoorInRoom().SetType(Door.DoorType.Boss);
                            Door[] doorPoints = newRoom.GetComponent<Room>().GetDoorPoints();
                            Instantiate(GetRandomBoss(), newRoom.transform.position, Quaternion.identity, newRoom.transform).SetActive(false);
                            int randomTreasureIndex = UnityEngine.Random.Range(0, treasureItemsPrefabs.Length);
                            GameObject itemToDrop = treasureItemsPrefabs[randomTreasureIndex];
                            newRoom.GetComponent<Room>().AddItemToDrop(itemToDrop);
                            foreach (Door d in doorPoints)
                            {
                                d.gameObject.SetActive(false);
                            }
                            generated = true;
                        }
                    }
                }
            }
        }
    }

    public GameObject GetNormalRoom()
    {
        return roomsPrefabs[0];
    }

    public GameObject GetTreasureRoom()
    {
        return roomsPrefabs[2];
    }

    public GameObject GetBossRoom()
    {
        return roomsPrefabs[1];
    }

    public GameObject GetRandomBoss()
    {
        return bossPrefabs[UnityEngine.Random.Range(0, bossPrefabs.Length)];
    }

    public GameObject GetRandomEnemy()
    {
        return enemiesPrefabs[UnityEngine.Random.Range(0, enemiesPrefabs.Length)];
    }
}