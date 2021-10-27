using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsActiveController : MonoBehaviour
{
    public Transform ship;
    public float activeDistance = 120f;
    public List<GameObject> gameObjects;

    private void Awake()
    {
        if (ship == null)
            ship = GameObject.FindGameObjectWithTag("Ship").transform;
    }
    private void Start()
    {
        SetGameObjectsList();
    }

    private void FixedUpdate()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject == null)
            {
                gameObjects.Remove(gameObject);
                return;
            }
            var distance = (gameObject.transform.position - ship.position).sqrMagnitude;
            if (distance > activeDistance * activeDistance)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }

    public void SetGameObjectsList()
    {
        gameObjects = new List<GameObject>();

        if (ship == null)
            ship = GameObject.FindGameObjectWithTag("Ship").transform;

        foreach (Transform child in transform)
        {
            gameObjects.Add(child.gameObject);
        }

        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject == null)
            {
                gameObjects.Remove(gameObject);
                return;
            }

            var distance = (gameObject.transform.position - ship.position).sqrMagnitude;
            if (distance > activeDistance * activeDistance)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }
}
