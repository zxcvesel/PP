using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    private void Awake()
    {
        this.itemsToDrop = new List<GameObject>();
        this.levelController = UnityEngine.Object.FindObjectOfType<LevelController>();
        this.bounds = base.transform.GetChild(0).GetComponent<Collider2D>().bounds;
    }

    private void Start()
    {
        this.audioSource = base.GetComponent<AudioSource>();
        this.connectedDoorsRooms = new Dictionary<string, GameObject>();
        for (int i = 0; i < this.doorPoints.Length; i++)
        {
            this.connectedDoorsRooms.Add(this.doorPoints[i].name, this.nextRoomsPoints[i]);
        }
        this.ready = true;
    }

    public Door GetOriginDoorInRoom()
    {
        return this.originDoorInRoom;
    }

    public Door[] GetDoorPoints()
    {
        return this.doorPoints;
    }

    public GameObject GetRoomPointFromDoorPoint(string doorPointName)
    {
        GameObject result;
        this.connectedDoorsRooms.TryGetValue(doorPointName, out result);
        return result;
    }

    public bool IsReady()
    {
        return this.ready;
    }

    public void InstantiateEnemies()
    {
        int num = UnityEngine.Random.Range(1, 5);
        GameObject randomEnemy = this.levelController.GetRandomEnemy();
        for (int i = 0; i < num; i++)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(randomEnemy, this.RandomPointInBounds(), Quaternion.identity, base.transform);
            this.quantityEnemies++;
            gameObject.SetActive(false);
        }
    }

    public void SetOriginDoor(string position)
    {
        if (position == "Left")
        {
            this.originDoorInRoom = (from d in this.doorPoints
                                     where d.name == "DoorPointRight"
                                     select d).First<Door>();
            this.doorPoints = (from d in this.doorPoints
                               where d.name != "DoorPointRight"
                               select d).ToArray<Door>();
            this.nextRoomsPoints = (from r in this.nextRoomsPoints
                                    where r.name != "RoomPointRight"
                                    select r).ToArray<GameObject>();
            return;
        }
        if (position == "Down")
        {
            this.originDoorInRoom = (from d in this.doorPoints
                                     where d.name == "DoorPointUp"
                                     select d).First<Door>();
            this.doorPoints = (from d in this.doorPoints
                               where d.name != "DoorPointUp"
                               select d).ToArray<Door>();
            this.nextRoomsPoints = (from r in this.nextRoomsPoints
                                    where r.name != "RoomPointUp"
                                    select r).ToArray<GameObject>();
            return;
        }
        if (position == "Up")
        {
            this.originDoorInRoom = (from d in this.doorPoints
                                     where d.name == "DoorPointDown"
                                     select d).First<Door>();
            this.doorPoints = (from d in this.doorPoints
                               where d.name != "DoorPointDown"
                               select d).ToArray<Door>();
            this.nextRoomsPoints = (from r in this.nextRoomsPoints
                                    where r.name != "RoomPointDown"
                                    select r).ToArray<GameObject>();
            return;
        }
        if (!(position == "Right"))
        {
            return;
        }
        this.originDoorInRoom = (from d in this.doorPoints
                                 where d.name == "DoorPointLeft"
                                 select d).First<Door>();
        this.doorPoints = (from d in this.doorPoints
                           where d.name != "DoorPointLeft"
                           select d).ToArray<Door>();
        this.nextRoomsPoints = (from r in this.nextRoomsPoints
                                where r.name != "RoomPointLeft"
                                select r).ToArray<GameObject>();
    }

    public void EnterFocus()
    {
        MonoBehaviour.print("ergwergeg");
        MonoBehaviour.print(base.gameObject.name);
        if (this.quantityEnemies > 0)
        {
            this.audioSource.Play();
            foreach (Transform transform in base.GetComponentsInChildren<Transform>(true))
            {
                if (transform.tag == "Enemy")
                {
                    transform.gameObject.SetActive(true);
                }
            }
            return;
        }
        if (base.gameObject.name.Contains("Boss"))
        {
            GameObject gameObject = base.transform.GetChild(base.transform.childCount - 1).gameObject;
            this.nextLevelDoor = GameObject.FindGameObjectWithTag("NextLevelDoor");
            if (gameObject.tag == "Enemy")
            {
                UnityEngine.Object.FindObjectOfType<GameController>().StartCoroutine("ShowImageBossFight", gameObject);
                return;
            }
        }
        else
        {
            this.OpenDoors();
        }
    }

    public void EnemyDeath()
    {
        this.quantityEnemies--;
        if (this.quantityEnemies <= 0)
        {
            foreach (GameObject original in this.itemsToDrop)
            {
                UnityEngine.Object.Instantiate<GameObject>(original, this.RandomPointInBounds(), Quaternion.identity);
            }
            this.OpenDoors();
        }
    }

    public void OpenDoors()
    {
        this.audioSource.Play();
        foreach (Door door in this.doorPoints)
        {
            if (door.isActiveAndEnabled)
            {
                door.Open();
            }
        }
        if (this.originDoorInRoom != null)
        {
            this.originDoorInRoom.GetComponent<Door>().Open();
        }
        if (this.nextLevelDoor != null)
        {
            this.nextLevelDoor.GetComponent<SpriteRenderer>().enabled = true;
            this.nextLevelDoor.GetComponent<Collider2D>().enabled = true;
        }
    }

    public Vector2 RandomPointInBounds()
    {
        return new Vector2(Random.Range(this.bounds.min.x, this.bounds.max.x), UnityEngine.Random.Range(this.bounds.min.y, this.bounds.max.y));
    }

    public GameObject GetPointZero()
    {
        return this.pointZero;
    }

    public void AddItemToDrop(GameObject itemToDrop)
    {
        this.itemsToDrop.Add(itemToDrop);
    }

    private Bounds bounds;

    [SerializeField] private GameObject[] nextRoomsPoints;
    [SerializeField] private Door[] doorPoints;
    [SerializeField] private GameObject pointZero;

    private GameObject nextLevelDoor;
    private List<GameObject> itemsToDrop;
    private Dictionary<string, GameObject> connectedDoorsRooms;
    private LevelController levelController;
    private Door originDoorInRoom;
    private AudioSource audioSource;
    private int quantityEnemies;
    private bool ready;
}