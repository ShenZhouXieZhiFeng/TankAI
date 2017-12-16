using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathController : MonoBehaviour {

    public GameObject WallPrefab;
    public GameObject StartPrefab;
    public GameObject EndPrefab;
    public GameObject RoutePrefab;

    public Text GeneTex;

    public int[,] Map;//真实地图
    public int[,] TempMap;//用来缓存本次测试的路径地图

    private Vector2 StartPos;
    private Vector2 EndPos;

    private int mapHeight;
    private int mapWidth;

    List<GameObject> RouteList = new List<GameObject>();

    private void Awake()
    {
        Map = new int[10, 15] {
             {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
             {1, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1},
             {8, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1},
             {1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1},
             {1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1},
             {1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1},
             {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1},
             {1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 5},
             {1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1},
             {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };
        mapHeight = 10;
        mapWidth = 15;
        StartPos = new Vector2(0, 2);
        EndPos = new Vector2(14, 7);
        TempMap = new int[10, 15];
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                GameObject target = null;
                if (Map[i, j] == 1)
                {
                    target = WallPrefab;
                }
                else if (Map[i, j] == 8)
                {
                    target = StartPrefab;
                }
                else if (Map[i, j] == 5)
                {
                    target = EndPrefab;
                }
                if (target != null)
                {
                    GameObject np = Instantiate(target);
                    np.transform.position = new Vector3(i, 0, j);
                    np.transform.parent = transform;
                }
            }
        }
        ClearRoute();
    }

    public void SetGeneCount(int count) {
        GeneTex.text = "遗传次数:" + count;
    }
	
    /// <summary>
    /// 测试路径
    /// 0=北, 1=南, 2=东, 3=西
    /// </summary>
    /// <param name="cmd"></param>
    public double TestRoute(List<int> cmd) {
        ClearRoute();
        int posX = (int)StartPos.x;
        int posY = (int)StartPos.y;
        double fitNess = 0;
        for (int i = 0; i < cmd.Count; i++) {
            int dir = cmd[i];
            switch (dir) {
                case 0:
                    if (posY - 1 <= 0 || Map[posY - 1, posX] == 1)
                        break;
                    posY -= 1;
                    RenderRoute(posX, posY);
                    break;
                case 1:
                    if (posY + 1 >= mapHeight || Map[posY + 1, posX] == 1)
                        break;
                    posY += 1;
                    RenderRoute(posX, posY);
                    break;
                case 2:
                    if (posX + 1 >= mapWidth || Map[posY, posX + 1] == 1)
                        break;
                    posX += 1;
                    RenderRoute(posX, posY);
                    break;
                case 3:
                    if (posX - 1 <= 0 || Map[posY, posX - 1] == 1)
                        break;
                    posX -= 1;
                    RenderRoute(posX, posY);
                    break;
                default:
                    break;
            }
            TempMap[posY, posX] = 1;
            //根据最后一步距离终点的距离计算适应性
            int diffX = Mathf.Abs(posX - (int)EndPos.x);
            int diffY = Mathf.Abs(posY - (int)EndPos.y);
            double temp = 1 / (double)(diffX + diffY + 1);
            if (temp > fitNess)
                fitNess = temp;
        }
        return fitNess;
    }

    void RenderRoute(int _x, int _y)
    {
        GameObject route = Instantiate(RoutePrefab);
        route.transform.position = new Vector3(_y, 0, _x);
        route.transform.parent = transform;
        RouteList.Add(route);
    }

    void ClearRoute() {
        if (RouteList.Count != 0)
        {
            foreach (GameObject go in RouteList)
            {
                DestroyImmediate(go);
            }
            RouteList.Clear();
        }
        for (int i = 0; i < TempMap.GetLength(0); i++)
        {
            for (int j = 0; j < TempMap.GetLength(1); j++)
            {
                TempMap[i, j] = 0;
            }
        }
    }
}
