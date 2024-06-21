using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private Animator animator;
    private bool isOpened;
    private bool needKey;

    // Название сцены, в которую нужно перейти при открытии двери
    [SerializeField]
    private string targetScene;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetType(DoorType type)
    {
        StartCoroutine(SetTypeWaitingToAnimator(type));
    }

    private IEnumerator SetTypeWaitingToAnimator(DoorType type)
    {
        yield return new WaitUntil(() => animator != null);

        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
        {
            int level = gameController.GetLevel();
            animator.SetInteger("Level", level);
        }

        switch (type)
        {
            case DoorType.Normal:
                animator.SetBool("IsNormalDoor", true);
                break;
            case DoorType.Treasure:
                animator.SetBool("IsTreasureDoor", true);
                break;
            case DoorType.Boss:
                animator.SetBool("IsBossDoor", true);
                break;
        }

        if (type == DoorType.Treasure && gameController.GetLevel() > 1)
        {
            needKey = true;
        }
    }

    public void Open()
    {
        if (!needKey)
        {
            isOpened = true;
            animator.SetBool("Opened", true);
            // Перемещение игрока в другую сцену
            SceneManager.LoadScene(targetScene);
        }
    }

    public void SetIsOpened(bool isOpened)
    {
        this.isOpened = isOpened;
        animator.SetBool("Opened", isOpened);
    }

    public bool GetIsOpened()
    {
        return isOpened;
    }

    public bool GetNeedKey()
    {
        return needKey;
    }

    public void OpenWithKey()
    {
        isOpened = true;
        animator.SetBool("Opened", true);
        // Перемещение игрока в другую сцену
        SceneManager.LoadScene(targetScene);
    }

    public enum DoorType
    {
        Normal,
        Treasure,
        Boss
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpened && collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}