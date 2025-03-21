using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public bool player;
    public int hp;  // HP
    public int mobility;  // Mobility
    public int damage;  // Damage
    public int attackRange;  // Range
    public int protect;  // Protection (防御力)
    public int attackStrong;  // Breakthrough (防御突破力/貫通力)
    public int createPoint;  // 生産にかかるポイント　0で生産不可
    private int fullHp;


    [SerializeField] GameObject selectedPosition;
    [SerializeField] GameObject movePosition;
    [SerializeField] GameObject attackPosition;
    
    public GameManegerScript gameManegerScript;
    [SerializeField]private Transform thisTransform;
    public GameManegerScript.Space nowSpace;

    // for moving soldier
    [SerializeField]private float moveSpeed;  // grid/sec
    private float nowPosition_x;
    private float nowPosition_y;


    public bool move_finished = true;
    public bool attack_finished = true;

    private bool pastTurn;
    
    // Start is called before the first frame update
    void Start()
    {
        fullHp = hp;
        pastTurn = gameManegerScript.enemyTurn;
    }

    // Update is called once per frame
    void Update()
    {
        nowSpace.soldier = this;
        if(nowSpace.x != nowPosition_x || nowSpace.y != nowPosition_y)
        {
            /*Vector2 moveDirection = new Vector2(nowSpace.x - nowPosition_x, nowSpace.y - nowPosition_y);
            moveDirection.Normalize();
            Vector2 move = moveDirection * moveSpeed * Time.deltaTime;
            if(Mathf.Abs(nowSpace.x - nowPosition_x) < move.x || Mathf.Abs(nowSpace.y - nowPosition_y) < move.y)
            {
                thisTransform.position = nowSpace.getPosition();
            }
            else{
                thisTransform.position += (Vector3)move;
            }*/thisTransform.position = nowSpace.getPosition();
            nowPosition_x = thisTransform.position.x;
            nowPosition_y = thisTransform.position.y;
        }
        if(pastTurn != gameManegerScript.enemyTurn)
        {
            pastTurn = gameManegerScript.enemyTurn;
            if(pastTurn != player)
            {
                move_finished = false;
                attack_finished = false;
            }
        }
    }

    public void OnClick()
    {
        gameManegerScript.showStatus(hp,mobility,damage,attackRange,protect,attackStrong,player,move_finished,attack_finished);
        setSelectedPosition();
        if(!gameManegerScript.enemyTurn && player)
        {
            if(!move_finished)
            {
                setNextSpaces();
            }
            if(!attack_finished)
            {
                setAttackSoldier();
            }
        }
    }

    public void attack(Soldier target)
    {
        int giveDamage = Mathf.FloorToInt(((float)damage - ((float)target.protect / (float)attackStrong)) * ((float)hp / (float)fullHp));
        if (giveDamage == 0) giveDamage = 1;
        if(giveDamage > 0)
        {
            target.hp -= giveDamage;
            if(target.hp <= 0)
            {
                target.nowSpace.soldier = null;
                Destroy(target.transform.gameObject);
            }
        }
        attack_finished = true;
    }
    
    void setSelectedPosition() {
        Instantiate(selectedPosition, nowSpace.getPosition(), Quaternion.identity);
    }
    void setNextSpaces()
    {
        GameManegerScript.Space[] nextSpaces = getNextSpaces();
        for(int i = 0; i < nextSpaces.Length; i++)
        {
            GameObject gameObject = Instantiate(movePosition, nextSpaces[i].getPosition(), Quaternion.identity);
            MovePositionScript script = gameObject.GetComponent<MovePositionScript>();
            script.soldier = this;
            script.thisSpace = nextSpaces[i];
        }
    }
    public GameManegerScript.Space[] getNextSpaces()
    {
        List<GameManegerScript.Space> result = new List<GameManegerScript.Space>();
        List<GameManegerScript.Space> stack = new List<GameManegerScript.Space>();
        stack.Add(nowSpace);
        bool[] finishSpace = new bool[gameManegerScript.map.Length];
        finishSpace[nowSpace.number] = true;
        bool[,] searchedPlace = new bool[gameManegerScript.map.Length,6];
        int nowMobility = mobility;

        void search()
        {
            List<GameManegerScript.Space> nextPositions = stack[stack.Count - 1].getSurround();
            GameManegerScript.Space next = null;
            for (int i = 0; i < nextPositions.Count; i++)
            {
                if(!searchedPlace[stack[stack.Count - 1].number,i] && nextPositions[i].soldier == null && (stack.Count <= 1 || nextPositions[i].number != stack[stack.Count - 2].number))
                {
                    next = nextPositions[i];
                    searchedPlace[stack[stack.Count - 1].number,i] = true;
                    break;
                }
            }
            if(next != null)
            {
                stack.Add(next);
                if(!finishSpace[next.number]) result.Add(next);
                finishSpace[next.number] = true;
                nowMobility -= next.movility_use;
                if(nowMobility == 0)
                {
                    stack.RemoveAt(stack.Count - 1);
                    nowMobility += next.movility_use;
                }
                else if(nowMobility < 0)
                {
                    stack.RemoveAt(stack.Count - 1);
                    result.RemoveAt(result.Count - 1);
                    finishSpace[next.number] = false;
                    nowMobility += next.movility_use;
                }
            }
            else
            {
                int lastStackNumber = stack.Count - 1;
                searchedPlace[stack[lastStackNumber].number,0] = false;
                searchedPlace[stack[lastStackNumber].number,1] = false;
                searchedPlace[stack[lastStackNumber].number,2] = false;
                searchedPlace[stack[lastStackNumber].number,3] = false;
                searchedPlace[stack[lastStackNumber].number,4] = false;
                searchedPlace[stack[lastStackNumber].number,5] = false;
                nowMobility += stack[lastStackNumber].movility_use;
                stack.RemoveAt(lastStackNumber);
            }
        }

        while(stack.Count > 0)
        {
            search();
        }

        return result.ToArray();
    }
    void setAttackSoldier()
    {
        Soldier[] attackSoldier = getAttackSoldier();
        for(int i = 0; i < attackSoldier.Length; i++)
        {
            GameObject gameObject = Instantiate(attackPosition, attackSoldier[i].nowSpace.getPosition(), Quaternion.identity);
            AttackPositionScript script = gameObject.GetComponent<AttackPositionScript>();
            script.attackSoldier = this;
            script.attackedSoldier = attackSoldier[i];
        }
    }
    public Soldier[] getAttackSoldier()
    {
        List<Soldier> result = new List<Soldier>();
        List<GameManegerScript.Space> stack = new List<GameManegerScript.Space>();
        stack.Add(nowSpace);
        bool[] finishSpace = new bool[gameManegerScript.map.Length];
        finishSpace[nowSpace.number] = true;
        bool[,] searchedPlace = new bool[gameManegerScript.map.Length,6];
        int nowRange = attackRange;

        void search()
        {
            List<GameManegerScript.Space> nextPositions = stack[stack.Count - 1].getSurround();
            GameManegerScript.Space next = null;
            for (int i = 0; i < nextPositions.Count; i++)
            {
                if(!searchedPlace[stack[stack.Count - 1].number,i] && (stack.Count <= 1 || nextPositions[i].number != stack[stack.Count - 2].number))
                {
                    next = nextPositions[i];
                    searchedPlace[stack[stack.Count - 1].number,i] = true;
                    break;
                }
            }
            if(next != null)
            {
                stack.Add(next);
                if(next.soldier != null && next.soldier.player != player && !finishSpace[next.number]) result.Add(next.soldier);
                finishSpace[next.number] = true;
                nowRange -= 1;
                if(nowRange == 0)
                {
                    stack.RemoveAt(stack.Count - 1);
                    nowRange += 1;
                }
                else if(nowRange < 0)
                {
                    stack.RemoveAt(stack.Count - 1);
                    result.RemoveAt(result.Count - 1);
                    finishSpace[next.number] = false;
                    nowRange += 1;
                }
            }
            else
            {
                int lastStackNumber = stack.Count - 1;
                searchedPlace[stack[lastStackNumber].number,0] = false;
                searchedPlace[stack[lastStackNumber].number,1] = false;
                searchedPlace[stack[lastStackNumber].number,2] = false;
                searchedPlace[stack[lastStackNumber].number,3] = false;
                searchedPlace[stack[lastStackNumber].number,4] = false;
                searchedPlace[stack[lastStackNumber].number,5] = false;
                nowRange += 1;
                stack.RemoveAt(lastStackNumber);
            }
        }

        while(stack.Count > 0)
        {
            search();
        }

        return result.ToArray();
    }
}
