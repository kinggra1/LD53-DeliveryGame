using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAround : MonoBehaviour
{
    public float maxTravelDistance = 0.2f;
    public bool rotate = true;

    private float randomTimeOffset;
    private float xMultipler;
    private float yMultipler;
    private float rotationSpeed;

    private float startingRotation;
    private Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = this.transform.localPosition;
        startingRotation = this.transform.localRotation.eulerAngles.z;

        randomTimeOffset = Random.value * 100f;
        xMultipler = Random.Range(0.5f, 2f);
        yMultipler = Random.Range(0.5f, 2f);

        rotationSpeed = Random.Range(-30f, 30f);
    }

    // Update is called once per frame
    void Update()
    {
        float timer = Time.time;


        this.transform.localPosition =
            startingPos +
            new Vector3(
                Mathf.Sin(timer * xMultipler + randomTimeOffset * Mathf.Deg2Rad),
                Mathf.Sin(timer * yMultipler + randomTimeOffset * Mathf.Deg2Rad),
                0f) * maxTravelDistance;

        if (rotate) {
            this.transform.localRotation = Quaternion.Euler(0f, 0f, startingRotation + timer * rotationSpeed);
        }
    }
}
