using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreateButtonController : MonoBehaviour
{
    public GameManegerScript gameManegerScript;

    public void SetInfantryCreatePoints()
    {
        gameManegerScript.PlayerCastle.setCreatePoints(gameManegerScript.soldiers.infantry_player);
    }

    public void SetCavalryCreatePoints()
    {
        gameManegerScript.PlayerCastle.setCreatePoints(gameManegerScript.soldiers.Cavalry_player);
    }

    public void SetArtilleryCreatePoints()
    {
        gameManegerScript.PlayerCastle.setCreatePoints(gameManegerScript.soldiers.artillery_player);
    }
}
