using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector3 MovementSpeed = new Vector3(0f, 0f, 25f);

    void Update()
    {
        // Handle hay movement speed
        transform.Translate(MovementSpeed * Time.deltaTime);
    }
}
