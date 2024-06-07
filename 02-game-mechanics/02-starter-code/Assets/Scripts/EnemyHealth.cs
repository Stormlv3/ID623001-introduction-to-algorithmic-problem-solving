using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour 
{
    public float maxHealth = 100;
    public float currentHealth = 100;
    private float originalXScale;

    private void Start() 
    {
        // The health bar takes note of its initial x scale, so that it can
        // rescale itself relative to that initial scale.
        originalXScale = gameObject.transform.localScale.x;
    }

    private void Update() 
    {
        // newScale is going to be what we set the scale to. Initially, it's
        // just whatever the current scale is.
        Vector3 newScale = gameObject.transform.localScale;
        
        // TODO: Update the x value of newScale. The new value should be a number
        // between 0 and originalXScale based on our currentHealth and maxHealth
        // I.E. if currentHealth is 0, x scale should be 0. If currentHealth
        // == maxHealth, then x scale should be originalXScale.

        gameObject.transform.localScale = newScale;
    }
}