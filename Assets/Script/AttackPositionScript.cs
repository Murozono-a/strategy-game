using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPositionScript : MonoBehaviour
{
    public Soldier attackSoldier;
    public Soldier attackedSoldier;
    
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
        attackSoldier.attack(attackedSoldier);
    }
}
