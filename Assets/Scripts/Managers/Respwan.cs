using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Respwan : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.SoftResetLevel();
            CameraManager cam = Camera.main.GetComponent<CameraManager>();
            cam?.SnapToTarget();
        }
    }
}
