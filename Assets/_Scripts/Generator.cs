using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Cell {
	public bool visited;
	public GameObject north;//1
	public GameObject east;//2
	public GameObject west;//3
	public GameObject south;//4
}
public class Generator : MonoBehaviour {

	[SerializeField] private Slider _rowsInput, _columnsInput;
	[SerializeField] private TextMeshProUGUI _rowsText, _columnsText;
	[SerializeField] private GameObject _wall, _ground;
	private GameObject _wallHolder, _tempWall;
	[SerializeField] private Material _wallMat;
	[HideInInspector] public int Row , Column;
	[SerializeField] private float _wallLength = 1.0f;
	[HideInInspector] public Vector3 StartPosition;

	private int _currentCell = 0;
	private Cell[] _cells;
	private int _totalCells;
	private int _currentNeighbour = 0;
	private int _backingUp = 0;
	private List<int> _cellList;

	[SerializeField] private bool _generateMazeInstantly;
	[SerializeField] private bool _generateEnvironmentalObjects;

	[HideInInspector] public bool CanGenerate = true;
	[SerializeField] private GameObject _finishLine;
	[SerializeField] private float _finishLineOffset_X;
	[SerializeField] private float _finishLineOffset_Y;
	[SerializeField] private float _finishLineOffset_Z;
	[SerializeField] private GameObject _player;
	[SerializeField] private ParticleSystem _particlesWallDestroyed;
	[SerializeField] private GameObject[] _environment_sideObjects;
	[SerializeField] private GameObject[] _environment_pebbles;

	private MeshCombiner _meshCombiner;
	[SerializeField] private CamerasScript _camerasScript;

	private void Awake()
    {
		_meshCombiner = gameObject.AddComponent<MeshCombiner>();
    }
    public void GenerateNewMaze()
	{
       
        if (!CanGenerate)//Checks if the maze has finished building before starting to build another one 
        {
			return;
        }

			Row = (int)_rowsInput.value;
			Column = (int)_columnsInput.value;
			_totalCells = Row * Column;		

			DestroyPreviousMaze(); 
            CreateWall();
			SpawnPlayer();
			SpawnFinishLine(_finishLine);
			_camerasScript.SetMapCameraHeight(Row, Column);
	}

	//Generates all the possible walls of the maze based on the rows and columns input 
	public void CreateWall()
	{
        _wallHolder = new GameObject
        {
            name = "Walls",
            tag = "Walls"
        };

		//Destroys previous environmental objects if they exist
        if (GameObject.FindGameObjectWithTag("Environment") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Environment"));
        }


        StartPosition = new Vector3((-Column / 2) + _wallLength / 2, 0.0f, (-Row / 2) + _wallLength / 2);
		Vector3 myPos;

		//Generate columns
		for (int i = 0; i < Row; i++)
		{
			for (int j = 0; j <= Column; j++)
			{
				myPos = new Vector3(StartPosition.x + (j * _wallLength) - _wallLength/2 ,0.0f, StartPosition.z +(i * _wallLength) - _wallLength/2);
				_tempWall = Instantiate(_wall,myPos,Quaternion.identity) as GameObject;
				_tempWall.name = "column " + i + "," + j;		
				_tempWall.transform.parent = _wallHolder.transform;
			}
		}

		//Generate rows
		for (int i = 0; i <= Row; i++)
		{
			for (int j = 0; j < Column; j++)
			{
				myPos = new Vector3(StartPosition.x + (j * _wallLength) , 0.0f, StartPosition.z +( i * _wallLength) - _wallLength);
				_tempWall = Instantiate(_wall,myPos,Quaternion.Euler(0,90,0)) as GameObject;
				_tempWall.name = "row " + i + "," + j;
				_tempWall.transform.parent = _wallHolder.transform;
			}
		}

        if (_generateEnvironmentalObjects)
        {
			var environment = new GameObject();
			environment.name = "Environment";
			var randomRotation = Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0));

			//Generate environmental objects on the side of the maze  
			for (int i = 0; i < Row; i++)
			{		
				var myPosLeft = new Vector3(StartPosition.x - Random.Range(1f, 3f), 0, StartPosition.z + (i * _wallLength));
				var myPosRight = new Vector3(Mathf.Abs(StartPosition.x) + Random.Range(1f + _wallLength / 3, 3f), 0f, StartPosition.z + (i * _wallLength));

				var environmentalObjectLeft = Instantiate(_environment_sideObjects[Random.Range(0, _environment_sideObjects.Length)], myPosLeft, randomRotation);
				environmentalObjectLeft.transform.parent = environment.transform;

				var environmentalObjectRight = Instantiate(_environment_sideObjects[Random.Range(0, _environment_sideObjects.Length)], myPosRight, randomRotation);
				environmentalObjectRight.transform.parent = environment.transform;
			}

			//Generate environmental objects along the top wall 
			for (int i = 0; i < Column + 1; i++)
			{
				if (i < Column / 2 - 2 || i > Column / 2 + 2)
				{
					var myPosTop = new Vector3(StartPosition.x + (i * _wallLength) - _wallLength / 2, 0.0f, Mathf.Abs(StartPosition.z) + Random.Range(_wallLength/2, _wallLength));
					var environmentalObject = Instantiate(_environment_sideObjects[Random.Range(0, _environment_sideObjects.Length)], myPosTop, randomRotation);
					environmentalObject.transform.parent = environment.transform;
				}
			}

			//Generate rocks on the ground
			for(int i = 0; i < Row; i += 3)
            {
				for (int j = 0; j <= Column; j += 3) 
                {
					var myPosPebbles = new Vector3(Random.Range(StartPosition.x + (j * _wallLength) - _wallLength / 2, StartPosition.x + (j * _wallLength) + _wallLength), 0.0f, StartPosition.z + (i * _wallLength) - _wallLength / 2);
					var pebbles = Instantiate(_environment_pebbles[Random.Range(0, _environment_pebbles.Length)], myPosPebbles, randomRotation);
                    pebbles.transform.parent = environment.transform;
                }
			}
			_meshCombiner.CombineMeshes(environment, "Environment", "Environment", _wallMat); //Combining the meshes to improve performance
		}
		CreateCells();
	}

	//Assigning created walls to the cells direction (north,east,west,south)
	public void CreateCells()
	{
		_cellList = new List<int>();
		int children = _wallHolder.transform.childCount;
		GameObject[] allWalls = new GameObject[children];
		_cells = new Cell[_totalCells];

		int eastWestProccess = 0;
		int childProcess = 0;
		int termCount = 0;
		int cellProccess = 0;

		//Assigning the walls to an array
		for (int i = 0; i < children; i++)
		{
			allWalls[i] = _wallHolder.transform.GetChild(i).gameObject;
		}

		//Assigning walls to the cells
		for (int j = 0; j < Column; j++)
		{
			_cells[cellProccess] = new Cell();

			_cells[cellProccess].west = allWalls[eastWestProccess];
			_cells[cellProccess].south = allWalls[childProcess + (Column + 1) * Row];
			termCount++;
			childProcess++;
			_cells[cellProccess].north = allWalls[(childProcess + (Column + 1) * Row) + Column - 1];
			eastWestProccess++;
			_cells[cellProccess].east = allWalls[eastWestProccess];

			cellProccess++;
			if (termCount == Column && cellProccess < _cells.Length)
			{
				eastWestProccess++;
				termCount = 0;
				j = -1;
			}
		}

		if (_generateMazeInstantly)//Either instantly generates a maze or gradually 
        {
			CreateMaze();		
		}
		else
        {
			StartCoroutine(CreateMazeCoroutine());
		}	
	}

	//Checks a random neighbour to see if it has been visited and if there is a wall inbetween 
	void GiveMeNeighbour()
	{
		int length = 0;
		int[] neighbour = new int[4];
		int[] connectingWall = new int[4];
		int check = 0;
		check = (_currentCell + 1) / Column;
		check -=1;
		check *= Column;
		check += Column;
		//north
		if (_currentCell + Column < _totalCells)
		{
			if (_cells[_currentCell + Column].visited == false)
			{
				neighbour[length] = _currentCell + Column;
				connectingWall[length] = 1;
				length++;
			}
		}
		//east
		if (_currentCell + 1 < _totalCells && (_currentCell + 1) != check)
		{
			if (_cells[_currentCell + 1].visited == false)
			{
				neighbour[length] = _currentCell + 1;
				connectingWall[length] = 2;
				length++;
			}
		}
		//west
		if (_currentCell - 1 >= 0 && _currentCell != check)
		{
			if (_cells[_currentCell - 1].visited == false)
			{
				neighbour[length] = _currentCell - 1;
				connectingWall[length] = 3;
				length++;
			}
		}
		//south
		if (_currentCell - Column >=  0)
		{
			if (_cells[_currentCell - Column].visited == false)
			{
				neighbour[length] = _currentCell - Column;
				connectingWall[length] = 4;
				length++;
			}
		}

		//Gets random neighbour and destroys the wall
		if (length != 0)
		{
			int randomNeighbour = Random.Range(0,length);
			_currentNeighbour = neighbour[randomNeighbour];
			DestroyWall(connectingWall[randomNeighbour]);
           
		}
		else if (_backingUp > 0)
		{
			_currentCell = _cellList[_backingUp];
			_backingUp--;
		}
	}

	void CreateMaze()
	{
		bool startedBuilding = false;
		int visitedCells = 0;
		while(visitedCells < _totalCells)
		{
			CanGenerate = false;

			if(startedBuilding)
			{
                GiveMeNeighbour();          

                if (!_cells[_currentNeighbour].visited && _cells[_currentCell].visited)
				{			
					_cells[_currentNeighbour].visited = true;
					visitedCells++;
					_cellList.Add(_currentCell);
					_currentCell = _currentNeighbour;
		
					if (_cellList.Count > 0)
						_backingUp = _cellList.Count - 1;
				}
			}
			else
			{
				_currentCell = Random.Range(0,_totalCells);
				_cells[_currentCell].visited = true;
				visitedCells++;
				startedBuilding = true;
			}
		}
		//Combine all the meshes of the maze into one to improve perfermonce
		StartCoroutine(_meshCombiner.CombineMeshesIn4Delay(_wallHolder, "MazeWithCombinedMeshes", "Walls", _wallMat, 0.1f));
		CanGenerate = true;
	}

	//Function called to generate the maze gradually, the walls are destroyed one by one 
	IEnumerator CreateMazeCoroutine()
	{
		bool startedBuilding = false;
		int visitedCells = 0;
		while (visitedCells < _totalCells)
		{
			CanGenerate = false;

			if (startedBuilding)
			{
				GiveMeNeighbour();
				
				if (!_cells[_currentNeighbour].visited && _cells[_currentCell].visited)
				{
				
					_cells[_currentNeighbour].visited = true;
					visitedCells++;
					_cellList.Add(_currentCell);
					_currentCell = _currentNeighbour;

					if (_cellList.Count > 0)
						_backingUp = _cellList.Count - 1;
				}
			}
			else
			{
				_currentCell = Random.Range(0, _totalCells);
				_cells[_currentCell].visited = true;
				visitedCells++;
				startedBuilding = true;
			}
			yield return new WaitForSeconds(0.01f);
		}
		_meshCombiner.CombineMeshesIn4(_wallHolder, "MazeWithCombinedMeshes", "Walls", _wallMat);
		CanGenerate = true;

        _camerasScript.NewMazeCameraWorkCoroutine(); //Switches the camera to player after the maze has finished building 
    }

	void DestroyWall(int neighbour)
	{
		switch (neighbour)
		{
			//case 1 means north wall
			case 1 :
				GraduallyGeneratingMaze_Vizualization(_cells[_currentCell].north);
				Destroy(_cells[_currentCell].north);
			break;

			//case 2 means east wall
			case 2 :
				GraduallyGeneratingMaze_Vizualization(_cells[_currentCell].east);
				Destroy(_cells[_currentCell].east);
			break;
			
			//case 3 means west wall
			case 3 :
				GraduallyGeneratingMaze_Vizualization(_cells[_currentCell].west);
				Destroy(_cells[_currentCell].west);
			break;
			
			//case 4 means south wall
			case 4 :
				GraduallyGeneratingMaze_Vizualization(_cells[_currentCell].east);
				Destroy(_cells[_currentCell].south);
			break;
			
			default:
			break;
		}
	}

	//Instantiates particles effect at the position of every wall that gets destroyed
	private void GraduallyGeneratingMaze_Vizualization(GameObject wall) 
    {
		if (!_generateMazeInstantly && wall != null)
		{
			var particles = Instantiate(_particlesWallDestroyed, wall.transform.position, Quaternion.identity);
            Destroy(particles.gameObject, 2);
        }
	}

	public void GenerateMazeInstatly_Bool()
    {
		if (!_generateMazeInstantly)
			_generateMazeInstantly = true;
		else
			_generateMazeInstantly = false;

    }

	private void DestroyPreviousMaze()
    {
	if (GameObject.FindGameObjectsWithTag("Walls") != null) //for the generate maze coroutine
		{
			GameObject[] previousMaze = GameObject.FindGameObjectsWithTag("Walls");
			
			foreach (var maze in previousMaze)
            {
				Destroy(maze);
            }			
		}
	}

	private void SpawnPlayer()
    {
		if (GameObject.FindGameObjectWithTag("Player"))
		{
			Destroy(GameObject.FindGameObjectWithTag("Player"));
		}
		Instantiate(_player, new Vector3(Mathf.Round(Random.Range(StartPosition.x, Mathf.Abs(StartPosition.x))) + 0.5f, 0, StartPosition.z - 0.5f), Quaternion.identity);//Instantiates player at random position on the first row
		SoundManager.Instance.PlayAudio(2, 1); //play falling player audio 
	}

	private void SpawnFinishLine(GameObject FinishLine)
    {
		if (GameObject.FindGameObjectWithTag("FinishLine") != null)
        {
			Destroy(GameObject.FindGameObjectWithTag("FinishLine"));
        }
        
		var endPosition = new Vector3(_finishLineOffset_X, _finishLineOffset_Y, Mathf.Abs(StartPosition.z) + _finishLineOffset_Z);
		var finishLine = Instantiate(FinishLine, endPosition, _finishLine.transform.rotation);
		finishLine.tag = "FinishLine";	
    }

	public void DisplayRowsText()
    {
		_rowsText.text = "Rows: " + _rowsInput.value.ToString();
    }

	public void DisplayColumnsText()
    {
		_columnsText.text = "Columns: " + _columnsInput.value.ToString();
    }

	public void AddRows()
    {
		_rowsInput.value++;
    }

	public void DecreaseRows()
	{
		_rowsInput.value--;
	}

	public void AddColumns()
    {
		_columnsInput.value++;
    }

	public void DecreaseColumns()
    {
		_columnsInput.value--;
    }

	public void SetEnvironmentOption() //gets triggered when pressing the environement toggle box in the menu 
    {
		if (_generateEnvironmentalObjects)
			_generateEnvironmentalObjects = false;
		else
			_generateEnvironmentalObjects= true;
	}
}





