using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    public enum BackdropEffectType
    {
        HORIZONTAL,
        VERTICAL,
        FIXED
    }
    [SerializeField] private CamFollowController camFollowController;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (camFollowController != null)
            {
                camFollowController.sceneTransitionTween(new Vector3(transform.position.x, transform.position.y, -10f));
            }
        }
    }
}