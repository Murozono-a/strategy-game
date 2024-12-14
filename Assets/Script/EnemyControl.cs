using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]private GameManegerScript gameManegerScript;
    public List<Soldier> enemySoldiers;
    public List<Castle> enemyCastles;
    private List<Soldier> availableSoldiers;  // まだ行動していない兵士
    private bool thisTurn;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(thisTurn)
        {
            if(availableSoldiers.Count > 0)
            {
                Soldier soldier = availableSoldiers[Random.Range(0,availableSoldiers.Count)];
                GameManegerScript.Space pastSpace = soldier.nowSpace;

                //兵士の移動
                GameManegerScript.Space[] moveSpaces = soldier.getNextSpaces();
                if(moveSpaces.Length > 0)
                {
                    GameManegerScript.Space moveSpace_decided = moveSpaces[0];
                    foreach (GameManegerScript.Space moveSpace in moveSpaces)
                    {
                        if(moveSpace.priority > moveSpace_decided.priority)
                        {
                            moveSpace_decided = moveSpace;
                        }
                    }
                    pastSpace.soldier = null;
                    soldier.nowSpace = moveSpace_decided;
                    moveSpace_decided.soldier = soldier;
                }
                //兵士の攻撃
                Soldier[] attackSoldiers = soldier.getAttackSoldier();
                if(attackSoldiers.Length > 0)
                {
                    Soldier attackSoldier_decided = attackSoldiers[0];
                    foreach (Soldier attackSoldier in attackSoldiers)
                    {
                        if(attackSoldier.hp < attackSoldier_decided.hp)
                        {
                            attackSoldier_decided = attackSoldier;
                        }
                    }
                    soldier.attack(attackSoldier_decided);
                }

                availableSoldiers.Remove(soldier);
            }
            else
            {
                gameManegerScript.finishEnemyTurn();
                thisTurn = false;
            }
        }
        else if(!thisTurn && gameManegerScript.enemyTurn)
        {
            for(int i = 0; i < enemySoldiers.Count; i++)
            {
                if(enemySoldiers[i] == null)
                {
                    enemySoldiers.RemoveAt(i);
                    i--;
                }
            }
            availableSoldiers = new List<Soldier>(enemySoldiers);
            thisTurn = true;

            for(int i = 0; i < enemyCastles.Count; i++)
            {
                if(enemyCastles[i] == null)
                {
                    enemyCastles.RemoveAt(i);
                    i--;
                }
            }

            GameObject[] enemySoldiersKind = gameManegerScript.soldiers.getAllEnemySoldier();
            for(int i = 0; i < enemyCastles.Count; i++)
            {
                while(true)
                {
                    GameObject produceSoldier = enemySoldiersKind[Random.Range(0,enemySoldiersKind.Length)];
                    int createPoint = produceSoldier.GetComponent<Soldier>().createPoint;
                    if(enemyCastles[i] != null && enemyCastles[i].point >= createPoint)
                    {
                        GameManegerScript.Space[] createSpaces = enemySoldiers[i].GetComponent<Castle>().getCreateSpaces();
                        GameManegerScript.Space moveSpace = findCreateSpace(createSpaces);
                        if (moveSpace != null)
                        {
                            GameObject producedSoldier = gameManegerScript.setSoldier(produceSoldier, ref moveSpace);
                            enemyCastles[i].point -= createPoint;
                            enemySoldiers.Add(producedSoldier.GetComponent<Soldier>());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
    GameManegerScript.Space findCreateSpace(GameManegerScript.Space[] spaces)
    {
        GameManegerScript.Space result = null;
        foreach (GameManegerScript.Space space in spaces)
        {
            if(space.soldier == null && (result == null || result.priority < space.priority))
            {
                result = space;
            }
        }
        return result;
    }
}
