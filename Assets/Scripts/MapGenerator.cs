using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Map;

public class MapGenerator : MonoBehaviour
{
    const float X_START = -800f;
    const float Y_START = -1500f;
    const float X_DIST = 233.333f;
    const float Y_DIST = 180f;
    const float PLACEMENT_RANDOMNESS = 40f;
    const int MAP_WIDTH = 7;
    const int PATHS = 6;
    const int FLOORS = 15; //16th node is boss

    [SerializeField]
    GameObject[] nodeVariants;

    [SerializeField]
    LegendBehavior[] legends;

    [SerializeField]
    TMP_InputField seedInput;

    [SerializeField]
    TextMeshProUGUI seedDisplay;

    [SerializeField]
    GameObject content;

    [SerializeField]
    GameObject line;

    [SerializeField]
    RandomSeedController seedController;

    [SerializeField]
    AutoScroll autoScroll;

    Node[][] mapData;
    Node start;
    Node boss;
    Node player;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        ClearMap();
        start = new Node();
        boss = new Node(NodeType.Boss, new List<Node>(), new List<Node>());
        player = start;

        seedController.GenerateRandomSeed();

        if(seedInput.text.Length > 0)
        {
            seedController.SetRandomSeed(seedInput.text);
        }

        Random.InitState(seedController.GetCurrentSeedHash());
        seedDisplay.text = seedController.GetCurrentSeed();
        seedInput.text = string.Empty;


        mapData = GenerateInitialGrid();
        CreatePaths();
        DisplayMap();
        autoScroll.StartAutoScroll();

        foreach(LegendBehavior legend in legends)
        {
            legend.Populate();
        }
    }

    private Node[][] GenerateInitialGrid()
    {
        Node[][] tempData = new Node[FLOORS][];

        for (int i = 0; i < FLOORS; i++)
        {
            tempData[i] = new Node[MAP_WIDTH];

            for (int j = 0; j < MAP_WIDTH; j++)
            {
                tempData[i][j] = new Node();
            }
        }

        return tempData;
    }

    private void CreatePaths()
    {
        for (int i = 0; i < PATHS; i++)
        {
            int x = Random.Range(0, MAP_WIDTH);

            while (mapData[0][x].GetNextNodes().Count >= 2)
            {
                x++;

                if (x == MAP_WIDTH)
                {
                    x = 0;
                }
            }

            Node current = mapData[0][x];
            Node next = current;

            current.AddPrevNode(start);
            start.AddNextNode(current);

            AssignNodeType(current, 0);

            for (int j = 1; j < FLOORS; j++)
            {
                switch (x)
                {
                    case 0:

                        if (mapData[j - 1][x + 1].GetNextNodes().Contains(mapData[j][x]))
                        {
                            break;
                        }

                        x += Random.Range(0, 2);

                        break;

                    case 6:

                        if (mapData[j - 1][x - 1].GetNextNodes().Contains(mapData[j][x]))
                        {
                            break;
                        }

                        x += Random.Range(-1, 1);

                        break;

                    default:

                        if (mapData[j - 1][x + 1].GetNextNodes().Contains(mapData[j][x]) && mapData[j - 1][x - 1].GetNextNodes().Contains(mapData[j][x]))
                        {
                            break;
                        }
                        else if (mapData[j - 1][x + 1].GetNextNodes().Contains(mapData[j][x]))
                        {
                            x += Random.Range(-1, 1);
                        }
                        else if (mapData[j - 1][x - 1].GetNextNodes().Contains(mapData[j][x]))
                        {
                            x += Random.Range(0, 2);
                        }
                        else
                        {
                            int temp = Random.Range(-1, 2);
                            int tries = 0;

                            while (mapData[j][x + temp].GetPrevNodes().Count >= 2 && tries < 3)
                            {
                                temp++;

                                if (temp == 2)
                                {
                                    temp = -1;
                                }

                                tries++;
                            }

                            x += temp;
                        }

                        break;
                }

                next = mapData[j][x];

                current.AddNextNode(next);
                next.AddPrevNode(current);

                if (next.GetNodeType() == NodeType.Blank || next.GetNodeType() == current.GetNodeType())
                {
                    AssignNodeType(next, j);
                }

                current = next;

                if(j == 14)
                {
                    current.AddNextNode(boss);
                    boss.AddPrevNode(current);
                }
            }
        }

        RemoveNodes();
    }

    private void AssignNodeType(Node node, int height)
    {
        bool repeat = true;

        while (repeat)
        {
            int chance = Random.Range(0, 100);

            switch (height)
            {
                case 0:
                    
                    node.SetNodeType(NodeType.Combat);
                    
                    break;

                case 1:
                case 2:
                case 3:
                case 4:

                    if (chance < 55)
                    {
                        node.SetNodeType(NodeType.Combat);
                    }
                    else if (chance < 86)
                    {
                        node.SetNodeType(NodeType.Event);
                    }
                    else
                    {
                        node.SetNodeType(NodeType.Shop);
                    }

                    break;

                case 8:

                    node.SetNodeType(NodeType.Treasure);
                    
                    break;

                case 13:

                    if (chance < 48)
                    {
                        node.SetNodeType(NodeType.Combat);
                    }
                    else if (chance < 73)
                    {
                        node.SetNodeType(NodeType.Event);
                    }
                    else if (chance < 92)
                    {
                        node.SetNodeType(NodeType.Elite);
                    }
                    else
                    {
                        node.SetNodeType(NodeType.Shop);
                    }

                    break;

                case 14:

                    node.SetNodeType(NodeType.Campfire);

                    break;

                default:

                    if (chance < 45)
                    {
                        node.SetNodeType(NodeType.Combat);
                    }
                    else if (chance < 67)
                    {
                        node.SetNodeType(NodeType.Event);
                    }
                    else if (chance < 83)
                    {
                        node.SetNodeType(NodeType.Elite);
                    }
                    else if (chance < 95)
                    {
                        node.SetNodeType(NodeType.Campfire);
                    }
                    else
                    {
                        node.SetNodeType(NodeType.Shop);
                    }

                    break;
            }

            repeat = false;

            if (height == 0 || height == 8 || height == 14)
            {
                break;
            }

            foreach (Node p in node.GetPrevNodes())
            {
                if (NodeRepeat(p, node))
                {
                    repeat = true;
                }

                if (repeat)
                {
                    break;
                }
            }

            if (!repeat)
            {
                foreach (Node n in node.GetNextNodes())
                {
                    if (NodeRepeat(n, node))
                    {
                        repeat = true;
                        break;
                    }
                }
            }
        }
    }

    private void RemoveNodes()
    {
        for (int y = 0; y < FLOORS; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                if (mapData[y][x].GetNextNodes().Count == 0 && mapData[y][x].GetPrevNodes().Count == 0)
                {
                    mapData[y][x] = null;
                }
            }
        }
    }

    private bool NodeRepeat(Node x, Node y)
    {
        if((x.GetNodeType() == NodeType.Elite || x.GetNodeType() == NodeType.Shop || x.GetNodeType() == NodeType.Campfire) && x.GetNodeType() == y.GetNodeType())
        {
            return true;
        }

        return false;
    }

    private void DisplayMap()
    {
        for (int y = 0; y < FLOORS; y++)
        {
            float yPos = Y_START + (Y_DIST * y);

            for (int x = 0; x < MAP_WIDTH; x++)
            {
                float xPos = X_START + (X_DIST * x);

                if (mapData[y][x] != null)
                {
                    if (y == 8)
                    {
                        mapData[y][x].SetNodeType(NodeType.Treasure);
                    }

                    if (y == 14)
                    {
                        mapData[y][x].SetNodeType(NodeType.Campfire);
                    }

                    GameObject node = nodeVariants[((int)mapData[y][x].GetNodeType())];
                    Vector3 coords = new Vector3(xPos + Random.Range(PLACEMENT_RANDOMNESS * -1f, PLACEMENT_RANDOMNESS),
                        yPos + (Random.Range(PLACEMENT_RANDOMNESS * -1f, PLACEMENT_RANDOMNESS) / 2f));

                    mapData[y][x].SetGameNode(Instantiate(node, coords, Quaternion.identity));
                    mapData[y][x].GetGameNode().transform.SetParent(content.transform, false);

                    mapData[y][x].GetGameNode().GetComponent<NodeButtonFunction>().SetNode(mapData[y][x]);

                    if (y != 0)
                    {
                        foreach (Node n in mapData[y][x].GetPrevNodes())
                        {
                            GameObject temp = Instantiate(line, coords, Quaternion.identity);
                            temp.transform.SetParent(content.transform, false);
                            temp.transform.SetAsFirstSibling();
                            Vector3 rotCoords = mapData[y][x].GetGameNode().transform.localPosition - n.GetGameNode().transform.localPosition;
                            float angle = 90f + Mathf.Atan2(rotCoords.y, rotCoords.x) * 180 / Mathf.PI;
                            temp.transform.rotation = Quaternion.Euler(0, 0, angle);
                            float height = Mathf.Sqrt((rotCoords.x * rotCoords.x) + (rotCoords.y * rotCoords.y));
                            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(10, height);
                        }
                    }
                }
            }
        }

        foreach (Node n in start.GetNextNodes())
        {
            n.Activate();
            n.GetGameNode().GetComponentInChildren<Image>().color = new Color(0, 0, 0, 1);
        }

        boss.SetGameNode(Instantiate(nodeVariants[7], new Vector3(-100f, 1300f, 0f), Quaternion.identity));
        boss.GetGameNode().transform.SetParent(content.transform, false);
        boss.GetGameNode().GetComponent<NodeButtonFunction>().SetNode(boss);

        foreach (Node n in boss.GetPrevNodes())
        {
            GameObject temp = Instantiate(line, new Vector3(-100f, 1300f, 0f), Quaternion.identity);
            temp.transform.SetParent(content.transform, false);
            temp.transform.SetSiblingIndex(1);
            Vector3 rotCoords = boss.GetGameNode().transform.localPosition - n.GetGameNode().transform.localPosition;
            float angle = 90f + Mathf.Atan2(rotCoords.y, rotCoords.x) * 180 / Mathf.PI;
            temp.transform.rotation = Quaternion.Euler(0, 0, angle);
            float height = Mathf.Sqrt((rotCoords.x * rotCoords.x) + (rotCoords.y * rotCoords.y));
            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(10, height);
        }
    }

    private void ClearMap()
    {
        if (mapData != null)
        {
            start.Delete();
            boss.Delete();

            for (int y = 0; y < FLOORS; y++)
            {
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    if (mapData[y][x] != null)
                    {
                        mapData[y][x].Delete();
                    }
                }
            }

            foreach (GameObject l in GameObject.FindGameObjectsWithTag("Line"))
            {
                GameObject.Destroy(l);
            }

            foreach (LegendBehavior legend in legends)
            {
                legend.Clear();
            }
        }
    }

    public void SelectNode(GameObject next)
    {
        next.GetComponentInChildren<Button>().enabled = false;

        foreach (Node n in player.GetNextNodes())
        {
            if (n.GetGameNode() != next)
            {
                n.Deactivate();
                float rg = 100f / 255f;
                float b = 70f / 255f;
                n.GetGameNode().GetComponentInChildren<Image>().color = new Color(rg, rg, b, 1f);
            }
            else
            {
                player = n;
            }
        }

        //go into encounter

        player.Complete();
    }
}