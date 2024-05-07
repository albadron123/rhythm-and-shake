using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePoint : MonoBehaviour
{
    bool isAnnihilating = false;

    [SerializeField]
    int intensity = 100;

    [SerializeField]
    GameObject moneyParticlePrefab;

    float timer = 0;

    float initialZRot = 0;

    int dir = 1;

    [SerializeField]
    float annihilationTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        initialZRot = transform.rotation.eulerAngles.z;
        Annihilate();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnnihilating)
        {
            timer += Time.deltaTime;
            //shaking party!!!
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 
                                                  Mathf.Clamp(transform.rotation.eulerAngles.z + dir * 100 * Time.deltaTime, initialZRot-10f, initialZRot+10f));
            if (Random.value > 0.95f) 
                dir = -dir;
            if (timer > annihilationTime / intensity)
            {
                timer = 0;
                Instantiate(moneyParticlePrefab, transform.position + new Vector3(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), -5), Quaternion.identity);
            }
        }
    }


    void Annihilate()
    {
        Destroy(gameObject, annihilationTime);
        isAnnihilating = true;


    }
}
