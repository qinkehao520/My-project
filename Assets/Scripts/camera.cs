using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Cemera : MonoBehaviour
{
    [SerializeField] private Transform man;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(man.position.x, man.position.y, transform.position.z);
    }
}