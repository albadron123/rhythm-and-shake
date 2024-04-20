using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    [SerializeField] GameObject backgroundParticlePrefab;
    [SerializeField] List<Sprite> particleSprites;

    [SerializeField]
    float cellSide;
    [SerializeField]
    Vector2Int gridSize;
    [SerializeField]
    Vector3 initialPoint;

    [SerializeField]
    float maxVelocity;
    
    public List<GameObject> backgroundParticles;

    [SerializeField]
    Color col1;
    [SerializeField]
    Color col2;

    void Start()
    {
        backgroundParticles = new List<GameObject>();
        GenerateGridUncertain();
    }

    void GenerateGridUncertain()
    {
        for (float x = initialPoint.x; x < initialPoint.x + gridSize.x * cellSide + 1; x+=cellSide)
        {
            for (float y = initialPoint.y; y < initialPoint.y + gridSize.y * cellSide + 1; y += cellSide)
            {
                GameObject instance = Instantiate(backgroundParticlePrefab, 
                                                  new Vector3(x + Random.Range(-cellSide / 3, cellSide / 3), y + Random.Range(-cellSide / 3, cellSide / 3), 30), 
                                                  Quaternion.Euler(0,0,Random.Range(0,360)));
                instance.GetComponent<SpriteRenderer>().sprite = particleSprites[Random.Range(0, particleSprites.Count)];
                backgroundParticles.Add(instance);
            }
        }
    }

    void Update()
    {
        
    }

}
