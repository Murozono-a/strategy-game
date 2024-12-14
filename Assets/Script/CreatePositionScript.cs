using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePositionScript : MonoBehaviour
{
    public GameManegerScript gameManegerScript;
    public GameObject soldier;
    public GameManegerScript.Space thisSpace;
    public Castle castle;
    public int createPoint;
    
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
        gameManegerScript.setSoldier(soldier, ref thisSpace);
        castle.point -= createPoint;
    }
}
