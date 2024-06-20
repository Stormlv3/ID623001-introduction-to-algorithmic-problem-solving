/// 
/// Author: Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script handles the sheep, such as movement and what happens
/// when they are hit by a haybale or when they drop off the map.

using UnityEngine;
using UnityEngine.Events; 

public class Sheep : MonoBehaviour
{
    public class SheepEvent : UnityEvent<Sheep> { }

    public SheepEvent OnAteHay = new SheepEvent();
    public SheepEvent OnDropped = new SheepEvent();

    public float runSpeed;
    public float dropDestroyDelay;
    private bool dropped;
    private Collider myCollider;
    private Rigidbody myRigidbody;

    public GameObject FeedbackHeart;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Make the sheep move forward
        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
    }

    private void HitByHay()
    {
        // If in contact with the hay destory the sheep
        Destroy(gameObject);
    }

    public void EatHay()
    {
        // When hay is eaten play the appropriate sound effect and tween animation. Also, increase score
        SFXManager.Instance.PlaySheepHitSFX();
        GameManager.Instance.SaveSheep();
        Vector3 heartPosition = transform.position + new Vector3(0, 5, 0);
        Quaternion heartRotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
        Instantiate(FeedbackHeart, heartPosition, heartRotation);
        OnAteHay?.Invoke(this);

    }

    private void Drop()
    {
        // If sheep drops off map, play appropriate sound effect, remove a life and animate the sheep falling
        SFXManager.Instance.PlaySheepDropSFX();
        GameManager.Instance.DroppedSheep();
        dropped = true;
        myRigidbody.useGravity = true;
        myCollider.isTrigger = false;
        OnDropped?.Invoke(this);
        Destroy(gameObject, dropDestroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collided with hay
        if (other.CompareTag("Hay"))
        {
            Destroy(other.gameObject);
            EatHay();
            HitByHay();
        }

        // Collided with edge of map (drop)
        else if (other.CompareTag("DropSheep") && !dropped)
        {
            Drop();
        }
    }
}
