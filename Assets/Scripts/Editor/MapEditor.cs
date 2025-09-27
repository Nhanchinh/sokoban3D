using UnityEngine;
using UnityEditor;
using System.IO;

public class MapEditor : EditorWindow
{
    private string mapName = "level1";
    private int mapWidth = 7;
    private int mapHeight = 6;
    private char[,] mapGrid;
    private Vector2 tileScrollPosition;
    private Vector2 gridScrollPosition;
    
    // Ký tự đại diện cho các loại tile
    private char[] tileChars = { '#', '.', 'P', 'B', 'G' };
    private string[] tileNames = { "Wall", "Ground", "Player", "Box", "Goal" };
    private Color[] tileColors = { Color.red, Color.white, Color.blue, Color.yellow, Color.green };
    private char selectedTile = '#';
    
    // Brush settings
    private bool isDragging = false;
    private bool isErasing = false;
    private int lastDrawX = -1;
    private int lastDrawY = -1;
    
    [MenuItem("Tools/Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<MapEditor>("Map Editor");
    }
    
    void OnEnable()
    {
        InitializeMap();
    }
    
    void OnGUI()
    {
        GUILayout.Label("Sokoban Map Editor", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        // Map settings - GỌN HỚN
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Map:", GUILayout.Width(40));
        mapName = EditorGUILayout.TextField(mapName, GUILayout.Width(80));
        GUILayout.Label("Size:", GUILayout.Width(40));
        mapWidth = EditorGUILayout.IntField(mapWidth, GUILayout.Width(50));
        GUILayout.Label("x", GUILayout.Width(15));
        mapHeight = EditorGUILayout.IntField(mapHeight, GUILayout.Width(50));
        if (GUILayout.Button("New", GUILayout.Width(40)))
        {
            InitializeMap();
        }
        if (GUILayout.Button("Resize", GUILayout.Width(50)))
        {
            ResizeMap();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // Quick templates - NHỎ HỚN
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Templates:", GUILayout.Width(70));
        if (GUILayout.Button("Empty", GUILayout.Width(50)))
        {
            CreateEmptyRoom();
        }
        if (GUILayout.Button("Simple", GUILayout.Width(50)))
        {
            CreateSimplePuzzle();
        }
        if (GUILayout.Button("Maze", GUILayout.Width(40)))
        {
            CreateMaze();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // Tile selection - GỌN HỚN
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Tiles:", GUILayout.Width(40));
        
        for (int i = 0; i < tileChars.Length; i++)
        {
            bool isSelected = selectedTile == tileChars[i];
            
            // Button nhỏ hơn
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = isSelected ? Color.cyan : tileColors[i];
            
            if (GUILayout.Button(tileChars[i].ToString(), GUILayout.Width(25), GUILayout.Height(25)))
            {
                selectedTile = tileChars[i];
            }
            
            GUI.backgroundColor = originalColor;
        }
        
        // Brush tools - GỌN HỚN
        GUILayout.FlexibleSpace();
        GUILayout.Label("Tools:", GUILayout.Width(40));
        
        if (GUILayout.Button(isErasing ? "E" : "B", GUILayout.Width(25)))
        {
            isErasing = !isErasing;
        }
        if (GUILayout.Button("F", GUILayout.Width(25)))
        {
            FillWithSelectedTile();
        }
        if (GUILayout.Button("C", GUILayout.Width(25)))
        {
            ClearMap();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // Main content area - TỐI ƯU KHÔNG GIAN
        EditorGUILayout.BeginHorizontal();
        
        // Map grid - CHIẾM 70% KHÔNG GIAN
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.7f - 10));
        GUILayout.Label("Map Grid (Click & Drag)", EditorStyles.boldLabel);
        
        if (mapGrid == null)
        {
            InitializeMap();
        }
        
        EditorGUILayout.BeginVertical("box");
        
        // Scroll cho map grid
        gridScrollPosition = EditorGUILayout.BeginScrollView(gridScrollPosition, 
            GUILayout.Height(Mathf.Min(400, mapHeight * 25 + 20)), 
            GUILayout.Width(Mathf.Min(position.width * 0.7f - 30, mapWidth * 25 + 20)));
        
        HandleMouseEvents();
        
        // Draw grid với kích thước nhỏ hơn
        for (int y = 0; y < mapHeight; y++)
        {
            EditorGUILayout.BeginHorizontal();
            
            for (int x = 0; x < mapWidth; x++)
            {
                char currentTile = mapGrid[x, y];
                string buttonText = currentTile == ' ' ? " " : currentTile.ToString();
                
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = GetTileColor(currentTile);
                
                Rect buttonRect = GUILayoutUtility.GetRect(20, 20, GUILayout.Width(20), GUILayout.Height(20));
                
                if (GUI.Button(buttonRect, buttonText))
                {
                    DrawTile(x, y);
                }
                
                HandleMouseDrag(x, y, buttonRect);
                
                GUI.backgroundColor = originalColor;
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        
        // Preview area - CHIẾM 30% KHÔNG GIAN
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.3f - 10));
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        
        if (mapGrid != null)
        {
            string preview = "";
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    char tile = mapGrid[x, y];
                    preview += tile == ' ' ? '#' : tile;
                }
                if (y < mapHeight - 1) preview += "\n";
            }
            
            GUIStyle previewStyle = new GUIStyle(EditorStyles.textArea);
            previewStyle.fontSize = 10;
            previewStyle.wordWrap = false;
            
            EditorGUILayout.TextArea(preview, previewStyle, GUILayout.ExpandHeight(true));
        }
        
        EditorGUILayout.Space(10);
        
        // Action buttons - GỌN HỚN
        if (GUILayout.Button("💾 Save", GUILayout.Height(25)))
        {
            SaveMap();
        }
        
        if (GUILayout.Button("📁 Load", GUILayout.Height(25)))
        {
            LoadMap();
        }
        
        if (GUILayout.Button("🎮 Test", GUILayout.Height(25)))
        {
            TestMap();
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
    }
    
    private void HandleMouseEvents()
    {
        Event e = Event.current;
        
        if (e.type == EventType.MouseDown)
        {
            isDragging = true;
        }
        else if (e.type == EventType.MouseUp)
        {
            isDragging = false;
            lastDrawX = -1;
            lastDrawY = -1;
        }
    }
    
    private void HandleMouseDrag(int x, int y, Rect buttonRect)
    {
        Event e = Event.current;
        
        if (isDragging && buttonRect.Contains(e.mousePosition))
        {
            if (lastDrawX != x || lastDrawY != y)
            {
                DrawTile(x, y);
                lastDrawX = x;
                lastDrawY = y;
                e.Use();
            }
        }
    }
    
    private void DrawTile(int x, int y)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            if (isErasing)
            {
                mapGrid[x, y] = ' ';
            }
            else
            {
                mapGrid[x, y] = selectedTile;
            }
        }
    }
    
    private void InitializeMap()
    {
        mapGrid = new char[mapWidth, mapHeight];
        CreateEmptyRoom();
    }
    
    private void ResizeMap()
    {
        char[,] oldGrid = mapGrid;
        mapGrid = new char[mapWidth, mapHeight];
        
        if (oldGrid != null)
        {
            int minWidth = Mathf.Min(mapWidth, oldGrid.GetLength(0));
            int minHeight = Mathf.Min(mapHeight, oldGrid.GetLength(1));
            
            for (int x = 0; x < minWidth; x++)
            {
                for (int y = 0; y < minHeight; y++)
                {
                    mapGrid[x, y] = oldGrid[x, y];
                }
            }
        }
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (mapGrid[x, y] == '\0' || mapGrid[x, y] == ' ')
                {
                    mapGrid[x, y] = '.';
                }
            }
        }
    }
    
    private void CreateEmptyRoom()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (x == 0 || x == mapWidth - 1 || y == 0 || y == mapHeight - 1)
                {
                    mapGrid[x, y] = '#';
                }
                else
                {
                    mapGrid[x, y] = '.';
                }
            }
        }
        
        if (mapWidth > 2 && mapHeight > 2)
        {
            mapGrid[mapWidth / 2, mapHeight / 2] = 'P';
        }
    }
    
    private void CreateSimplePuzzle()
    {
        CreateEmptyRoom();
        
        if (mapWidth >= 5 && mapHeight >= 3)
        {
            mapGrid[2, mapHeight - 2] = 'B';
            mapGrid[mapWidth - 3, mapHeight - 2] = 'G';
        }
    }
    
    private void CreateMaze()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (x == 0 || x == mapWidth - 1 || y == 0 || y == mapHeight - 1)
                {
                    mapGrid[x, y] = '#';
                }
                else if ((x + y) % 3 == 0)
                {
                    mapGrid[x, y] = '#';
                }
                else
                {
                    mapGrid[x, y] = '.';
                }
            }
        }
    }
    
    private void FillWithSelectedTile()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (mapGrid[x, y] == ' ')
                {
                    mapGrid[x, y] = selectedTile;
                }
            }
        }
    }
    
    private void ClearMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                mapGrid[x, y] = ' ';
            }
        }
    }
    
    private Color GetTileColor(char tile)
    {
        switch (tile)
        {
            case '#': return Color.red;
            case '.': return Color.white;
            case 'P': return Color.blue;
            case 'B': return Color.yellow;
            case 'G': return Color.green;
            default: return Color.gray;
        }
    }
    
    private void SaveMap()
    {
        if (mapGrid == null) return;
        
        string mapContent = "";
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                mapContent += mapGrid[x, y] == ' ' ? '#' : mapGrid[x, y];
            }
            if (y < mapHeight - 1) mapContent += "\n";
        }
        
        string path = $"Assets/Resources/Maps/{mapName}.txt";
        
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllText(path, mapContent);
        AssetDatabase.Refresh();
        
        Debug.Log($"Map saved to: {path}");
        EditorUtility.DisplayDialog("Success", $"Map saved as {mapName}.txt", "OK");
    }
    
    private void LoadMap()
    {
        string path = $"Assets/Resources/Maps/{mapName}.txt";
        
        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            string[] lines = content.Replace("\r", "").Split('\n');
            
            if (lines.Length > 0)
            {
                mapHeight = lines.Length;
                mapWidth = lines[0].Length;
                
                mapGrid = new char[mapWidth, mapHeight];
                
                for (int y = 0; y < mapHeight; y++)
                {
                    string line = lines[y];
                    for (int x = 0; x < mapWidth && x < line.Length; x++)
                    {
                        mapGrid[x, y] = line[x];
                    }
                }
                
                Debug.Log($"Map loaded from: {path}");
                EditorUtility.DisplayDialog("Success", $"Map loaded from {mapName}.txt", "OK");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Error", $"Map file not found: {path}", "OK");
        }
    }
    
    private void TestMap()
    {
        SaveMap();
        EditorUtility.DisplayDialog("Test Map", $"Map {mapName} saved! Run the game to test.", "OK");
    }
}