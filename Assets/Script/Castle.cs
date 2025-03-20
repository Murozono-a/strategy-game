using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public bool player;
    public int point;
    [SerializeField]private int addedPointPerTurn;
    [SerializeField]private GameObject createSoldierObject;
    public GameManegerScript gameManegerScript;
    public GameManegerScript.Space castlePosition;

    private GameManegerScript.Space[] createPositions;

    private bool nowTurn;
    
    // Start is called before the first frame update
    void Start()
    {
        createPositions = GetComponent<Soldier>().nowSpace.getSurround().ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        if(player)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)) {
                setCreatePoints(gameManegerScript.soldiers.infantry_player);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)) {
                setCreatePoints(gameManegerScript.soldiers.Cavalry_player);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)) {
                setCreatePoints(gameManegerScript.soldiers.artillery_player);
            }
        }
        if(nowTurn != gameManegerScript.enemyTurn)
        {
            nowTurn = gameManegerScript.enemyTurn;
            if(nowTurn != player)
            {
                point += addedPointPerTurn;
            }
        }
    }

    public void setCreatePoints(GameObject soldier)
    {
        int createPoint = soldier.GetComponent<Soldier>().createPoint;
        if(createPoint > 0 && point >= createPoint)
        {
            for(int i = 0; i < createPositions.Length; i++)
            {
                if(createPositions[i].soldier == null)
                {
                    GameObject gameObject = Instantiate(createSoldierObject, createPositions[i].getPosition(), Quaternion.identity);
                    CreatePositionScript script = gameObject.GetComponent<CreatePositionScript>();
                    script.gameManegerScript = gameManegerScript;
                    script.soldier = soldier;
                    script.castle = this;
                    script.createPoint = createPoint;
                    script.thisSpace = createPositions[i];
                }
            }
        }
    }

    public GameManegerScript.Space[] getCreateSpaces() {
        return createPositions;
    }
}
