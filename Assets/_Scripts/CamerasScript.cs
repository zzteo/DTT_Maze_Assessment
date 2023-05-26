using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CamerasScript : MonoBehaviour
{
    [SerializeField] GameObject[] Cameras;
    private Transform _player;
    [SerializeField] private float _cameraFollowSpeed;
    private bool _cameraFollowPlayer;
    private Generator _mazeGenerator;

    private void Awake()
    {
        _mazeGenerator = GameObject.FindGameObjectWithTag("MazeGenerator").GetComponent<Generator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SeeFullMaze();
        }

        if (_cameraFollowPlayer)
        {
            //Sets the position of the main camera to follow the player
            var targetPos = new Vector3(_player.position.x, Cameras[1].transform.position.y, _player.transform.position.z - 3f);
            Cameras[1].transform.position = Vector3.Lerp(Cameras[1].transform.position, targetPos, _cameraFollowSpeed * Time.deltaTime);
        }
    }

    //Is triggered when a new maze is generated, it sets the height of the camera high enough to see the whole maze
    public void SetMapCameraHeight(int rows, int columns)
    {
        _cameraFollowPlayer = false;

        int cameraHeight;
        if (rows >= columns)
            cameraHeight = rows;
        else
            cameraHeight = columns;

        Cameras[0].transform.position = new Vector3(0, cameraHeight + 1, 0);
    }

    public void NewMazeCameraWorkCoroutine()
    {
        //Checks if the maze is still getting built; you can't generate another maze until the previous one has finished getting build
        //It avoids zooming in to the player so you can see the maze getting built
        //Function gets called when pressing the generate button when generating the maze instatly, otherwise it gets called after the last wall was destroyed in the Generator script when the maze is generated gradually 
        Cameras[1].SetActive(false);

        if (_mazeGenerator.CanGenerate)
        StartCoroutine(NewMazeCameraWork());
    }

    //When a maze is generated it firstly shows it all from above then it zooms in to player  
    private IEnumerator NewMazeCameraWork()
    {     
        if (Cameras[1].activeInHierarchy)
            Cameras[1].SetActive(false);

        yield return new WaitForSeconds(.5f);

        Cameras[1].SetActive(true);
        _cameraFollowPlayer = true;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    //Moves camera above to see the whole maze
    private void SeeFullMaze()
    {
        if (Cameras[1].activeInHierarchy)
            Cameras[1].SetActive(false);
        else
            Cameras[1].SetActive(true);
    }
}
