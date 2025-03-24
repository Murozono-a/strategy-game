using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI result;
    public TextMeshProUGUI turn;
    public TextMeshProUGUI point;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowResult(bool winner, int turnCount) {
        if (winner)
        {
            result.text = "victory";
        }
        else
        {
            result.text = "failure";
        }

        turn.text = turnCount.ToString();
    }

    public void RestartGame() {
        SceneManager.LoadScene(0);
    }
}
