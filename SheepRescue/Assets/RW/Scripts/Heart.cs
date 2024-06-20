/// 
/// Author: Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script manages the animation of the heart effect.

using UnityEngine;
using DG.Tweening;

public class Heart : MonoBehaviour
{
    void Start()
    {
        // Make the heart grow in size over 0.5 seconds and then over 0.2 seconds shrink to 0
        transform.DOScale(new Vector3(2f, 2f, 2f), 0.5f).OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, 0.2f);
        });

        // Destroy the heart after 3 seconds
        Destroy(gameObject, 3f);
    }
}
