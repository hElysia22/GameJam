using UnityEngine;

public class SetRespPos : MonoBehaviour
{
    Vector2 respPos;
    private void Awake()
    {
        respPos = new Vector2(transform.position.x, transform.position.y + 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null && other.CompareTag("Player"))
        {
            PlayerManager Pm =  other.GetComponent<PlayerManager>();
            Pm.respPos = respPos;
            Debug.Log(Pm.respPos);
        }
    }
}
