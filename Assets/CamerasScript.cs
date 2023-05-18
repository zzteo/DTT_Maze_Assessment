using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CamerasScript : MonoBehaviour
{
    [SerializeField] GameObject[] Cameras;
    private Transform Player;
    [SerializeField] private float _cameraFollowSpeed;
    private bool _playerSpawned;

   

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SeeFullMaze();
        }

        if (_playerSpawned)
        {
            //sets the position of the camera that follows the player
            var targetPos = new Vector3(Player.position.x, Cameras[1].transform.position.y, Player.transform.position.z - 3f);
            Cameras[1].transform.position = Vector3.Lerp(Cameras[1].transform.position, targetPos, _cameraFollowSpeed * Time.deltaTime);
        }
        
    }
    //Is triggered when the Generate Button is pressed, it sets the height of the camera high enough to see the whole maze
    public void SetMapCameraHeight(int rows, int columns)
    {
        int cameraHeight;
        if (rows >= columns)
            cameraHeight = rows;
        else
            cameraHeight = columns;

        Cameras[0].transform.position = new Vector3(0, cameraHeight + 1, 0);
    }

    public void NewMazeCameraWorkCoroutine()
    {
        StartCoroutine(NewMazeCameraWork());
    }
    private IEnumerator NewMazeCameraWork()
    {
        _playerSpawned = false;

        if (Cameras[1].activeInHierarchy)
            Cameras[1].SetActive(false);
        yield return new WaitForSeconds(.5f);

        _playerSpawned = true;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Cameras[1].SetActive(true);
    }

    private void SeeFullMaze()
    {
        if (Cameras[1].activeInHierarchy)
            Cameras[1].SetActive(false);
        else
            Cameras[1].SetActive(true);
    }
}
