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
            HitByHay();
            EatHay();
        }
        // Collided with edge of map:
        else if (other.CompareTag("DropSheep") && !dropped)
        {
            Drop();
        }
    }
}
