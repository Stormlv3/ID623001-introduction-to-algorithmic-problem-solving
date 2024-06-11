using System.Collections;
using System.Collections.Generic;
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
        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
    }

    private void HitByHay()
    {
        Destroy(gameObject);
    }

    public void EatHay()
    {
        Vector3 heartPosition = transform.position + new Vector3(0, 5, 0);
        Quaternion heartRotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
        Instantiate(FeedbackHeart, heartPosition, heartRotation);
        OnAteHay?.Invoke(this);

    }

    private void Drop()
    {
        dropped = true;
        myRigidbody.useGravity = true;
        myCollider.isTrigger = false;
        OnDropped?.Invoke(this);
        Destroy(gameObject, dropDestroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collided with hay:
        if (other.CompareTag("Hay"))
        {
            Destroy(other.gameObject);
            EatHay();
            HitByHay();
        }

        // Collided with edge of map:
        else if (other.CompareTag("DropSheep") && !dropped)
        {
            Drop();
        }
    }
}
