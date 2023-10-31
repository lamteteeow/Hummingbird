using System.Collections;
using System.Collections.Generic;
using Unity.Sentis;
using UnityEngine;

/// <summary>
/// Manages a single flower with nectar
/// </summary>
public class Flower : MonoBehaviour
{
    [Tooltip("The color when the flower is full")]
    public Color fullFlowerColor = new Color(1f, 0f, 0.3f); //pink-ish
    [Tooltip("The color when the flower is empty")]
    public Color emptyFlowerColor = new Color(.5f, 0f, 1f); //purple-ish

    /// <summary>
    /// The trigger collider representing the nectar
    /// </summary>
    [HideInInspector]
    public Collider nectarCollider;

    // The solid collider representing the flower petals
    private Collider flowerCollider;

    // The flower's material
    private Material flowerMaterial;

    // Accessors

    /// <summary>
    /// A vector pointing straight out of the flower
    /// </summary>
    public Vector3 FlowerUpVector
    {
        get
        {
            return nectarCollider.transform.up;
        }
    }

    /// <summary>
    /// The center position of the flower
    /// </summary>
    public Vector3 FlowerCenterPosition
    {
        get
        {
            return nectarCollider.transform.position;
        }
    }

    /// <summary>
    /// The amount of nectar remaining in the flower
    /// </summary>
    public float NectarAmount { get; private set; }

    /// <summary>
    /// Whether the flower has any nectar remaining
    /// </summary>
    public bool HasNectar
    {
        get
        {
            return NectarAmount > 0f;
        }
    }

    /// <summary>
    /// Attempts to remove nectar from the flower
    /// </summary>
    /// <param name="amount">The amount of nectar remove</param>
    /// <returns>The actual amount successfully removed</returns>
    public float Feed(float amount)
    {   
        // Track how much nectar was successfully taken (cannot take more than available)
        float nectarTaken = Mathf.Clamp(amount, 0f, NectarAmount);

        // Subtract the nectar amount
        NectarAmount -= amount;
        if (NectarAmount <= 0)
        {
            // No nectar remaining
            NectarAmount = 0;

            // Disable the flower and collider (helps performance)
            nectarCollider.gameObject.SetActive(false);
            flowerCollider.gameObject.SetActive(false);

            // Change the flower color to reflect empty
            flowerMaterial.SetColor("_BaseColor", emptyFlowerColor);
        }
        // Return the amount of nectar successfully taken
        return nectarTaken;
    }

    /// <summary>
    /// Resets the flower
    /// </summary>
    public void ResetFlower()
    {
        // Refill the nectar
        NectarAmount = 1f;
        
        // Enable the flower and collider
        flowerCollider.gameObject.SetActive(true);
        nectarCollider.gameObject.SetActive(true);

        // Change the flower color to reflect full
        flowerMaterial.SetColor("_BaseColor", fullFlowerColor);
    }

    /// <summary>
    /// Called when the flower wakes up
    /// </summary>
    private void Awake()
    {
        // Find the flower's mesh renderer and get the material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        flowerMaterial = meshRenderer.material; // flower has only one material in mesh renderer

        // Find the flower and nectar collider
        flowerCollider = transform.Find("FlowerCollider").GetComponent<Collider>();
        nectarCollider = transform.Find("FlowerNectarCollider").GetComponent<Collider>();

        // Reset the flower
        ResetFlower();
    }
}
