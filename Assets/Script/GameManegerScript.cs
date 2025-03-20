using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManegerScript : MonoBehaviour
{
    public Soldiers soldiers;
    [SerializeField]private EnemyControl enemyControl;
    public GameObject nextPosition;
    public TextMeshProUGUI turnInfo;
    public TextMeshProUGUI defaultInfo;
    public TextMeshProUGUI actionInfo;
    public TextMeshProUGUI pointInfo;
    private Castle playerCastle;
    public Space[,] map;
    private bool explanation = false;
    public bool enemyTurn;  // false -> プレイヤーのターン
    public bool gameFinished = false;
    public Castle PlayerCastle {get => playerCastle;}

    // 優先度設定
    Soldier playerCastleSoldier;
    Soldier enemyCastleSoldier;
    List<Soldier> playerSoldiers = new List<Soldier>();

    // 結果表示用
    bool winner;
    int turnCount = 0;
    bool resultShowed = false;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 30;
        Space first = new Space();
        first.x = -3.820724f;
        first.y = 3.488765f;
        Space rightBottom = new Space();
        rightBottom.x = -3.43889f;
        rightBottom.y = 2.792343f;
        map = makeMap(11, 11, first, rightBottom);

        GameObject playerCastle = setSoldier(soldiers.castle_player, ref map[8,2]);
        playerCastle.GetComponent<Castle>().gameManegerScript = this;
        this.playerCastle = playerCastle.GetComponent<Castle>();
        playerCastleSoldier = playerCastle.GetComponent<Soldier>();
        GameObject enemyCastle = setSoldier(soldiers.castle_enemy, ref map[2,8]);
        enemyCastle.GetComponent<Castle>().gameManegerScript = this;
        enemyCastleSoldier = enemyCastle.GetComponent<Soldier>();
        enemyControl.enemyCastles.Add(enemyCastle.GetComponent<Castle>());
        enemyControl.enemySoldiers.Add(enemyCastle.GetComponent<Soldier>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();

        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (!explanation)
            {
                SceneManager.LoadScene(1, LoadSceneMode.Additive);
                explanation = true;
            }
            else
            {
                SceneManager.UnloadSceneAsync(1);
                explanation = false;
            }
        }

        if(Input.GetKeyUp(KeyCode.Space)) finishPlayerTurn();
        if(!playerCastleSoldier && !gameFinished)
        {
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
            winner = false;
            gameFinished = true;
        }
        else if(!enemyControl.enemyCastles[0] && !gameFinished)
        {
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
            winner = true;
            gameFinished = true;
        }
        else if (gameFinished && !resultShowed)
        {
            FinishGame(winner);
        }

        pointInfo.text = "pt : " +  playerCastle.point.ToString();
    }

    public void finishPlayerTurn()
    {
        if(!enemyTurn)
        {
            turnInfo.text = "Enemy Turn";
            createPriority();
            enemyTurn = true;
            turnCount++;
        }
    }
    public void finishEnemyTurn()
    {
        if(enemyTurn)
        {
            enemyTurn = false;
            turnInfo.text = "Player Turn";
        }
    }
    void FinishGame(bool winner)
    {
        resultShowed = true;
        ResultManager resultManager = GameObject.Find("resultSceneManager").GetComponent<ResultManager>();
        resultManager.ShowResult(winner, turnCount);
    }

    void createPriority()
    {
        bool[] visitedSpace = new bool[map.Length];
        List<GameManegerScript.Space> stack = new List<GameManegerScript.Space>();
        if (playerSoldiers.Count + 3 < enemyControl.enemySoldiers.Count)
        {
            stack.Add(playerCastleSoldier.nowSpace);  // 城の位置からの優先度設定
            visitedSpace[playerCastleSoldier.nowSpace.number] = true;
        }
        else
        {
            stack.Add(enemyCastleSoldier.nowSpace);  // 城の位置からの優先度設定
            visitedSpace[enemyCastleSoldier.nowSpace.number] = true;
        }
        // playerSoldierの調整
        for(int i = 0; i < playerSoldiers.Count; i++)
        {
            if(playerSoldiers[i] == null)
            {
                playerSoldiers.RemoveAt(i);
                i--;
            }
        }
        foreach(Soldier playerSoldier in playerSoldiers)
        {
            stack.Add(playerSoldier.nowSpace);  // プレイヤー側の兵士からの優先度設定
            visitedSpace[playerSoldier.nowSpace.number] = true;
        }
        playerCastleSoldier.nowSpace.priority = map.Length;
        void search()
        {
            GameManegerScript.Space nowSpace = stack[0];
            List<GameManegerScript.Space> surrround = nowSpace.getSurround();
            foreach (GameManegerScript.Space next in surrround)
            {
                if (!visitedSpace[next.number])
                {
                    next.priority = nowSpace.priority - 1;
                    stack.Add(next);
                    visitedSpace[next.number] = true;
                }
            }
            stack.Remove(nowSpace);
        }

        while(stack.Count > 0)
        {
            search();
        }

    }

    
    public GameObject setSoldier(GameObject soldier, ref Space place)
    {
        GameObject gameObject = Instantiate(soldier, place.getPosition(), Quaternion.identity);
        Soldier gameObjectSoldier = gameObject.GetComponent<Soldier>();
        gameObjectSoldier.gameManegerScript = this;
        gameObjectSoldier.nowSpace = place;
        place.soldier = gameObjectSoldier;
        if(gameObjectSoldier.player)
        {
            playerSoldiers.Add(gameObjectSoldier);
        }
        return gameObject;
    }
    public void showStatus(int hp, int mobility, int damage, int attackRange, int protect, int attackStrong, bool player, bool move, bool attack)
    {
        defaultInfo.text = hp.ToString() + "\r\n" + damage.ToString() + "\r\n" + attackRange.ToString() + "\r\n" + mobility.ToString() + "\r\n" + protect.ToString() + "\r\n" + attackStrong.ToString();
        if(player)
        {
            string info = "move : ";
            info += move ? "finished" : "not finished";
            info += "\r\nattack : ";
            info += attack ? "finished" : "not finished";
            actionInfo.text = info;
        }
        else
        {
            actionInfo.text = "";
        }
    }

    Space[,] makeMap(int maxWidth, int height, Space first, Space rightBottom)
    {
        Space[,] result = new Space[height,maxWidth];

        float distance_x = rightBottom.x - first.x;
        float distance_y = first.y - rightBottom.y;

        int nextNumber = 0;
        
        result[0,0] = first;
        for (int i = 0 ; i < result.GetLength(0) ; i++)
        {
            for (int j = 0 ; j < result.GetLength(1) ; j++)
            {
                if (j == 0 && i < result.GetLength(0) - 1)
                {
                    result[i+1,j] = new Space();
                    if ((i+1) % 2 < 1)
                    {
                        result[i+1,j].x = result[i,j].x - distance_x;
                        result[i+1,j].y = result[i,j].y - distance_y;
                        result[i+1,j].rightTop = result[i,j];

                        result[i,j].leftBottom = result[i+1,j];
                        result[i,j].rightTop = result[i-1,j+1];
                        result[i,j].leftTop = result[i-1,j];

                        result[i-1,j+1].leftBottom = result[i,j];
                    }
                    else
                    {
                        result[i+1,j].x = result[i,j].x + distance_x;
                        result[i+1,j].y = result[i,j].y - distance_y;
                        result[i+1,j].leftTop = result[i,j];

                        result[i,j].rightBottom = result[i+1,j];
                    }
                }
                else if(j == 0 && i >= result.GetLength(0) - 1)
                {
                    ;
                }
                else
                {
                    result[i,j] = new Space();
                    result[i,j].x = result[i,j-1].x + (2*distance_x);
                    result[i,j].y = result[i,j-1].y;
                    if ((i+1) % 2 < 1)
                    {
                        if (j < maxWidth - 1) {
                            result[i,j].left = result[i,j-1];
                            result[i,j].leftTop = result[i-1,j];
                            result[i,j].rightTop = result[i-1,j+1];

                            result[i,j-1].right = result[i,j];
                            result[i-1,j].rightBottom = result[i,j];
                            result[i-1,j+1].leftBottom = result[i,j];
                        }
                        else
                        {
                            result[i,j] = null;
                        }
                    }
                    else
                    {
                        result[i,j].left = result[i,j-1];
                        if (i != 0) {
                            result[i,j].leftTop = result[i-1,j-1];
                            result[i,j].rightTop = result[i-1,j];
                        }

                        result[i,j-1].right = result[i,j];
                        if (i != 0) {
                            result[i-1,j-1].rightBottom = result[i,j];
                            if (j < maxWidth - 1) {
                                result[i-1,j].leftBottom = result[i,j];
                            }
                        }
                    }
                }
                if (result[i,j] != null)
                {
                    result[i,j].number = nextNumber++;
                }
            }
        }

        return result;
    }

    public class Space
    {
        public float x;
        public float y;

        public int number;

        public int movility_use = 1;
        public int priority = 0;

        public Soldier soldier = null;  // このマスに兵士がいるかどうか

        public Space rightTop = null;
        public Space right = null;
        public Space rightBottom = null;
        public Space leftTop = null;
        public Space left = null;
        public Space leftBottom = null;

        public Vector2 getPosition()
        {
            return new Vector2(x, y);
        }

        public List<Space> getSurround()
        {
            List<Space> resultList = new List<Space>();
            if (rightTop != null)
            {
                resultList.Add(rightTop);
            }
            if (right != null)
            {
                resultList.Add(right);
            }
            if (rightBottom != null)
            {
                resultList.Add(rightBottom);
            }
            if (leftTop != null)
            {
                resultList.Add(leftTop);
            }
            if (left != null)
            {
                resultList.Add(left);
            }
            if (leftBottom != null)
            {
                resultList.Add(leftBottom);
            }
            return resultList;
        }
    }
    
    [System.Serializable]
    public class Soldiers
    {
        public GameObject castle_player;
        public GameObject infantry_player;
        public GameObject Cavalry_player;
        public GameObject artillery_player;
        public GameObject castle_enemy;
        public GameObject infantry_enemy;
        public GameObject Cavalry_enemy;
        public GameObject artillery_enemy;

        public GameObject[] getAllEnemySoldier()
        {
            return new GameObject[]{infantry_enemy,Cavalry_enemy,artillery_enemy};
        }
    }
}
