using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // Make sure you have DOTween imported

public class Heart : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(new Vector3(2f, 2f, 2f), 0.5f).OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, 0.2f);
        });

        // Destroy the GameObject after 5 seconds
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // You can leave this empty if no additional logic is needed
    }
}
