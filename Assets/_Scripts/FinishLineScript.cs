using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineScript : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObjectsToBeActivated;

    //After the finishing line has been instantiated in the Generator script, it destroys the walls in front of it so the player can reach it
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {           
            Destroy(collision.gameObject.transform.parent.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerScript>().enabled = false; //Deactivates the player movement
            collision.gameObject.GetComponent<Animator>().enabled = false; 

            //Activates the player win gameobject and the other environmental gameobjects 
           foreach (GameObject gameObject in _gameObjectsToBeActivated)
            {
                gameObject.SetActive(true);
                StartCoroutine(WinRoutine());
            }
            
            IEnumerator WinRoutine()
            {
                SoundManager.Instance.PlayAudio(3, 0.2f);
                yield return new WaitForSeconds(5.4f);
                SoundManager.Instance.PlayAudio(5, 0.2f);
                yield return new WaitForSeconds(7.6f);
                SoundManager.Instance.PlayAudio(4, 0.1f);
                yield return new WaitForSeconds(1.5f);

                var ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_Script>();
                ui.FoldUnfold_UI();
            }
            
        }
    }
}
