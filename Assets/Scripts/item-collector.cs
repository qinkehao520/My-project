using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AllControl;

public class item_collector : MonoBehaviour
{
    int collects = GameManager.Instance.score;

    [SerializeField] private Text collectsText;
    [SerializeField] private AudioSource collectSoundEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("collect"))
        {
            collectSoundEffect.Play();  
            Destroy(collision.gameObject);
            collects
                ++;
            collectsText.text = "Collects: " + collects;

            GameManager.Instance.score = collects;
        }
    }
}