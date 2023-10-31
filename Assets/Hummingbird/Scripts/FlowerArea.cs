using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a collection of flower plants and attached flowers
/// </summary>
public class FlowerArea : MonoBehaviour
{

    // The diameter of the area for the agent and flowers to exist for observing relative distance from agent to flower
    public const float AreaDiameter = 20f; // Then normalize it between -1 and 1 for better calculation of NN

    // The list of all flower plants in this flower area (flower plants have multiple flowers)
    private List<GameObject> flowersPlants;

    // A lookup dictionary for looking up a flower from a nectar collider
    private Dictionary<Collider, Flower> nectarFlowerDictionary;

    /// <summary>
    /// The list of all flowers in this flower area
    /// </summary>
    public List<Flower> Flowers { get; private set; }   

    public void ResetFlowers()
    {
        // Rotate each flower plant around the Y axis and subtly around X and Z
        foreach (GameObject flowerPlant in flowersPlants)
        {
            float xRotation = UnityEngine.Random.Range(-5f, 5f);
            float yRotation = UnityEngine.Random.Range(-180f, 180f);
            float zRotation = UnityEngine.Random.Range(-5f, 5f);

            flowerPlant.transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation); // in degrees
        }

        // Reset each flower
        foreach (Flower flower in Flowers)
        {
            flower.ResetFlower();
        }
    }

    /// <summary>
    /// Gets the <see cref="Flower"/> that a nectar collider belongs to"/>
    /// </summary>
    /// <param name="nectarCollider">The nectar collider</param>
    /// <returns>The matching flower</returns>
    public Flower GetFlowerFromNectar(Collider collider)
    {
        return nectarFlowerDictionary[collider];
    }

    /// <summary>
    /// Called when the area wakes up
    /// </summary>
    private void Awake()
    {
        // Initialize variables
        flowersPlants = new List<GameObject>();
        nectarFlowerDictionary = new Dictionary<Collider, Flower>();
        Flowers = new List<Flower>();
    }

    /// <summary>
    /// Called when the game starts
    /// </summary>
    private void Start()
    {
        // Find all flowers that are children of this GameObject/Transform
        FindChildFlowers(transform); // put in Start instead of Awake since Awake could be called before all objects are instantiated
    }

    /// <summary>
    /// Recursively finds all flowers that are children of a parent transform
    /// </summary>
    /// <param name="parent">The parent of the children to check</param>
    private void FindChildFlowers(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i); // get the child at index i
            
            if (child.CompareTag("flower_plant"))
            {
                // The child is a flower plant, so add it to the list
                flowersPlants.Add(child.gameObject);

                // Recursively continue to search through the next generation of children
                FindChildFlowers(child);
            }
            else
            {
                // The child is a flower, so add it to the list
                Flower flower = child.GetComponent<Flower>();
                if (flower != null)
                {
                    Flowers.Add(flower);
                    nectarFlowerDictionary.Add(flower.nectarCollider, flower);
                    // Note: there are no flowers that are children of other flowers
                }
                else
                {
                    // Flower component not found, so check children
                    FindChildFlowers(child);
                }
            }
        }
    }   
}
