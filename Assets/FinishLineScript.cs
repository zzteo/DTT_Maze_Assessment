using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {           
            Destroy(collision.gameObject.transform.parent.gameObject);
        }
    }
}
