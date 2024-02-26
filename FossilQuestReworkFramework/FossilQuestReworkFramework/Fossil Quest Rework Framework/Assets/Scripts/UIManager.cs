using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public UIInstance[] uiInstances;

    void Start()
    {
        uiInstances = new UIInstance[GameManager.Instance.gameInstances.Length];
        
        for (int i = 0; i < GameManager.Instance.gameInstances.Length; i++)
        {
            uiInstances[i] = GameManager.Instance.gameInstances[i].uiInstance;
        }
    }

    void Update()
    {
        InputCheats();
    }
    
    void InputCheats()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            for (int i = 0; i < uiInstances.Length; i++)
            {
                if (uiInstances[i].brushToggle != null)
                    uiInstances[i].brushToggle.isOn = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            for (int i = 0; i < uiInstances.Length; i++)
            {
                if (uiInstances[i].chiselToggle != null)
                    uiInstances[i].chiselToggle.isOn = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            for (int i = 0; i < uiInstances.Length; i++)
            {
                if (uiInstances[i].fishUI != null)
                    uiInstances[i].fishUI.Show(true);

                if (uiInstances[i].toolTipUI != null)
                    uiInstances[i].toolTipUI.gameObject.SetActive(false);
            }
        }
    }
}
