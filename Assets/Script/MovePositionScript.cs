using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePositionScript : MonoBehaviour
{
    public Soldier soldier;
    public GameManegerScript.Space thisSpace;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown) {
            Destroy(gameObject, 0.2f);
        }
    }

    public void OnClick()
    {
        soldier.nowSpace.soldier = null;
        soldier.nowSpace = thisSpace;
        thisSpace.soldier = soldier;
        soldier.move_finished = true;
    }
}
