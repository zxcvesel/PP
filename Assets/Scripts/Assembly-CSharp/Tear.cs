using UnityEngine;

public class Tear : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private int damage = 20;
    [SerializeField] private AudioClip throwClip;   
    [SerializeField] private AudioClip touchedClip;

    private void Start()
    {
        
        audioSource = GetComponentInChildren<AudioSource>();

        
        if (audioSource != null && throwClip != null)
        {
            audioSource.PlayOneShot(throwClip);
        }
        else
        {
            Debug.LogWarning("AudioSource or throwClip is null.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        transform.localScale = new Vector3(1f, 1f, 1f);

        
        if (collision.tag != "Tear" && collision.tag != "ItemPasiveDamage")
        {
            StartAnim();
        }
    }

   
    public int GetDamage()
    {
        return damage;
    }

    
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    
    public Vector2 GetScale()
    {
        return transform.localScale;
    }

    
    public void AddScale(Vector3 addScale)
    {
        transform.localScale += addScale;
    }

    
    public void StartAnim()
    {
        GetComponent<Animator>().Play("Touch");
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        
        if (audioSource != null && touchedClip != null)
        {
            audioSource.PlayOneShot(touchedClip);
        }
        else
        {
            Debug.LogWarning("AudioSource or touchedClip is null.");
        }

        
        Invoke("DestroyTear", 0.5f);
    }

    
    public void DestroyTear()
    {
        Destroy(gameObject);
    }
}