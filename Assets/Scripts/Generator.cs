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
	[SerializeField] CamerasScript CamerasScript;
	[SerializeField] private Slider rowsInput, columnsInput;
	[SerializeField] private TextMeshProUGUI rowsText, columnsText;
	[SerializeField] GameObject wall, ground;
	private GameObject wallHolder, tempWall;
	[SerializeField] Material wallMat;
	[HideInInspector] public int row , column;
	public float wallLength = 1.0f;
	[HideInInspector] public Vector3 startPosition;
	private int currentCell = 0;
	public Cell[] cells;
	private int totalCells;
	private int currentNeighbour = 0;
	private int backingUp = 0;
	List<int> cellList;

	[SerializeField] private bool generateMazeInstantly;
	[SerializeField] PlayerScript PlayerScript;
	[SerializeField] private bool canGenerate = true;
	[SerializeField] private GameObject _finishLine;
	[SerializeField] private float _finishLineOffset;
	[SerializeField] private GameObject _cow;
	public CowScript CowScript;
	[SerializeField] private GameObject Player;	 

	MeshCombiner MeshCombiner;

    private void Awake()
    {
		MeshCombiner = gameObject.AddComponent<MeshCombiner>();
    }
    public void GenerateNewMaze()
	{
        //Finding the parent object of the walls if exists
        /*GameObject previousMaze = GameObject.FindGameObjectWithTag("Walls");*/
        if (!canGenerate)
        {
			return;
        }

		try
		{
	
			row = (int)rowsInput.value;
			column = (int)columnsInput.value;

			totalCells = row * column;

			CamerasScript.SetMapCameraHeight(row, column);

			/*Transform cameraPosition = camera.transform;
			int cameraHeight;
			if (row >= column)
				cameraHeight = row;
			else
				cameraHeight = column;

            cameraPosition.position = new Vector3(0, cameraHeight + 1, 0);   */      

			DestroyPreviousMaze();

            /*//Destroying previous maze if exist
			if (previousMaze != null)
			{
				Destroy(previousMaze);
				*//*Destroy(groundObject);*//*
			}*/          

            CreateWall();
            /*PlayerScript.SpawnPlayer(new Vector3(Mathf.Round(Random.Range(startPosition.x, Mathf.Abs(startPosition.x))) + 0.5f, 0, startPosition.z - 0.5f));*/

            if (GameObject.FindGameObjectWithTag("Cow") != null)
            {
                Destroy(GameObject.FindGameObjectWithTag("Cow"));
            }
            var cow = Instantiate(_cow, new Vector3(Mathf.Round(Random.Range(startPosition.x, Mathf.Abs(startPosition.x))) + 0.5f, 0.2f, Mathf.Round(Random.Range(startPosition.z, Mathf.Abs(startPosition.z)))), Quaternion.identity);
            cow.tag = "Cow";
            CowScript = cow.GetComponent<CowScript>();

            if (GameObject.FindGameObjectWithTag("Player"))
			{
				Destroy(GameObject.FindGameObjectWithTag("Player"));
			}
			Instantiate(Player, new Vector3(Mathf.Round(Random.Range(startPosition.x, Mathf.Abs(startPosition.x))) + 0.5f, 0, startPosition.z - 0.5f), Quaternion.identity);

			SpawnFinishLine(_finishLine);
		}
		catch (System.FormatException e)
		{
			Debug.Log(e);
		}
	}
	/// <summary>
	/// Creating wall gameobject based on rows and columns given
	/// </summary>
	public void CreateWall()
	{
        wallHolder = new GameObject
        {
            name = "Walls",
            tag = "Walls"
        };

        //create a script that has OnEnable() and add the anim you want for the maze when the game starts like a rotation and then a small shake on the ground after that some particle effects of ground being hit and sound

        startPosition = new Vector3((-column / 2) + wallLength / 2, 0.0f, (-row / 2) + wallLength / 2);
		Vector3 myPos;

		//for creating columns
		for (int a = 0; a < row; a++)
		{
			for (int i = 0; i <= column; i++)
			{
				myPos = new Vector3(startPosition.x + (i * wallLength) - wallLength/2 ,0.0f, startPosition.z +(a * wallLength) - wallLength/2);
				tempWall = Instantiate(wall,myPos,Quaternion.identity) as GameObject;
				tempWall.name = "column " + a + "," + i;		
				tempWall.transform.parent = wallHolder.transform;
			}
		}

		//for creating rows
		for (int a = 0; a <= row; a++)
		{
			for (int b = 0; b < column; b++)
			{
				myPos = new Vector3(startPosition.x + (b * wallLength) , 0.0f, startPosition.z +( a * wallLength) - wallLength);
				tempWall = Instantiate(wall,myPos,Quaternion.Euler(0,90,0)) as GameObject;
				tempWall.name = "row " + a + "," + b;
				tempWall.transform.parent = wallHolder.transform;
			}
		}
		
		CreateCells();
	}

	/// <summary>
	/// Assigning created walls to the cells direction (north,east,west,south)
	/// </summary>
	public void CreateCells()
	{
		cellList = new List<int>();
		int children = wallHolder.transform.childCount;
		GameObject[] allWalls = new GameObject[children];
		cells = new Cell[totalCells];

		int eastWestProccess = 0;
		int childProcess = 0;
		int termCount = 0;
		int cellProccess = 0;

		//Assigning all the walls to the allwalls array
		for (int i = 0; i < children; i++)
		{
			allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
		}

		//Assigning walls to the cells
		for (int j = 0; j < column; j++)
		{
			cells[cellProccess] = new Cell();

			cells[cellProccess].west = allWalls[eastWestProccess];
			cells[cellProccess].south = allWalls[childProcess + (column + 1) * row];
			termCount++;
			childProcess++;
			cells[cellProccess].north = allWalls[(childProcess + (column + 1) * row) + column - 1];
			eastWestProccess++;
			cells[cellProccess].east = allWalls[eastWestProccess];

			cellProccess++;
			if (termCount == column && cellProccess < cells.Length)
			{
				eastWestProccess++;
				termCount = 0;
				j = -1;
			}
		}
		

		if (generateMazeInstantly)
        {
			CreateMaze();		
		}
		else
        {
			StartCoroutine(CreateMazeCoroutine());
		}	
	}

	/// <summary>
	/// Getting a random neighbour if not visited and wall between them
	/// </summary>
	void GiveMeNeighbour()
	{
		int length = 0;
		int[] neighbour = new int[4];
		int[] connectingWall = new int[4];
		int check = 0;
		check = (currentCell + 1) / column;
		check -=1;
		check *= column;
		check += column;
		//north
		if (currentCell + column < totalCells)
		{
			if (cells[currentCell + column].visited == false)
			{
				neighbour[length] = currentCell + column;
				connectingWall[length] = 1;
				length++;
			}
		}
		//east
		if (currentCell + 1 < totalCells && (currentCell + 1) != check)
		{
			if (cells[currentCell + 1].visited == false)
			{
				neighbour[length] = currentCell + 1;
				connectingWall[length] = 2;
				length++;
			}
		}
		//west
		if (currentCell - 1 >= 0 && currentCell != check)
		{
			if (cells[currentCell - 1].visited == false)
			{
				neighbour[length] = currentCell - 1;
				connectingWall[length] = 3;
				length++;
			}
		}
		//south
		if (currentCell - column >=  0)
		{
			if (cells[currentCell - column].visited == false)
			{
				neighbour[length] = currentCell - column;
				connectingWall[length] = 4;
				length++;
			}
		}

		//Getting random neighbour and destroying the wall
		if (length != 0)
		{
			int randomNeighbour = Random.Range(0,length);
			currentNeighbour = neighbour[randomNeighbour];
			DestroyWall(connectingWall[randomNeighbour]);
		}
		else if (backingUp > 0)
		{
			currentCell = cellList[backingUp];
			backingUp--;
		}
	}

	void CreateMaze()
	{
		bool startedBuilding = false;
		int visitedCells = 0;
		while(visitedCells < totalCells)
		{
			canGenerate = false;

			if(startedBuilding)
			{
                GiveMeNeighbour();          

                if (!cells[currentNeighbour].visited && cells[currentCell].visited)
				{			
					cells[currentNeighbour].visited = true;
					visitedCells++;
					cellList.Add(currentCell);
					currentCell = currentNeighbour;
		
					if (cellList.Count > 0)
						backingUp = cellList.Count - 1;
				}
			}
			else
			{
				currentCell = Random.Range(0,totalCells);
				cells[currentCell].visited = true;
				visitedCells++;
				startedBuilding = true;
			}
		}
		/*MeshCombiner.StartCoroutine(MeshCombiner.CombineMeshesDelay(wallHolder, "MazeWithCombinedMeshes", wallMat, 0.1f));*/
		/*PlayerScript.DestroyWallInFrontOfThePlayer();*/
		StartCoroutine(MeshCombiner.CombineMeshesDelay(wallHolder, "MazeWithCombinedMeshes", "Walls", wallMat, 0.1f));


		canGenerate = true;
	}

	IEnumerator CreateMazeCoroutine()
	{
		bool startedBuilding = false;
		int visitedCells = 0;
		while (visitedCells < totalCells)
		{
			canGenerate = false;

			if (startedBuilding)
			{
				GiveMeNeighbour();
				
				if (!cells[currentNeighbour].visited && cells[currentCell].visited)
				{
				
					cells[currentNeighbour].visited = true;
					visitedCells++;
					cellList.Add(currentCell);
					currentCell = currentNeighbour;

					if (cellList.Count > 0)
						backingUp = cellList.Count - 1;
				}
			}
			else
			{
				currentCell = Random.Range(0, totalCells);
				cells[currentCell].visited = true;
				visitedCells++;
				startedBuilding = true;
			}
			yield return new WaitForSeconds(0.01f);
		}
		/*PlayerScript.DestroyWallInFrontOfThePlayer();*/
		MeshCombiner.CombineMeshes(wallHolder, "MazeWithCombinedMeshes", "Walls", wallMat);
		canGenerate = true;
	}

	void DestroyWall(int neighbour)
	{
		switch (neighbour)
		{
			//case 1 means north wall
			case 1 : 
				Destroy(cells[currentCell].north);
			break;

			//case 2 means east wall
			case 2 : 
				Destroy(cells[currentCell].east);
			break;
			
			//case 3 means west wall
			case 3 :
				Destroy(cells[currentCell].west);
			break;
			
			//case 4 means south wall
			case 4 : 
				Destroy(cells[currentCell].south);	
			break;
			
			default:
			break;
		}
	}

	public void GenerateMazeInstatly_Bool()
    {
		if (!generateMazeInstantly)
			generateMazeInstantly = true;
		else
			generateMazeInstantly = false;

    }

	private void DestroyPreviousMaze()
    {
	if (GameObject.FindGameObjectsWithTag("Walls") != null) //for the generate maze coroutine
		{
			Debug.Log("Previous Maze Destroyed");
			Destroy(GameObject.FindWithTag("Walls"));
		}
	}

	private void SpawnFinishLine(GameObject FinishLine)
    {
		var finishLine = GameObject.FindGameObjectWithTag("FinishLine");

		if (finishLine != null)
        {
			Destroy(finishLine.gameObject);
        }
        
		var endPosition = new Vector3(0, 0, Mathf.Abs(startPosition.z) + _finishLineOffset);
		finishLine = Instantiate(FinishLine, endPosition, _finishLine.transform.rotation);
		finishLine.tag = "FinishLine";	
    }

	public void DisplayRowsText()
    {
		rowsText.text = "Rows: " + rowsInput.value.ToString();
    }

	public void DisplayColumnsText()
    {
		columnsText.text = "Columns: " + columnsInput.value.ToString();
    }

	public void AddRows()
    {
		rowsInput.value++;
    }

	public void DecreaseRows()
	{
		rowsInput.value--;
	}

	public void AddColumns()
    {
		columnsInput.value++;
    }

	public void DecreaseColumns()
    {
		columnsInput.value--;
    }

	public void SpawnCow()
    {
		/*      
        transform.position = new Vector3(MazeGenerator.startPosition.x + 0.5f + (Random.Range(0, MazeGenerator.column) * MazeGenerator.wallLength) - MazeGenerator.wallLength / 2, 0.0f, MazeGenerator.startPosition.z + (Random.Range(0, MazeGenerator.row) * MazeGenerator.wallLength) - MazeGenerator.wallLength / 2);         
*/
	}
}





