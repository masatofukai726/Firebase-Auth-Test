using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{

    [SerializeField] GameObject SettingPanel;

    public void ManageSettingPanel()
    {
        if (SettingPanel.activeInHierarchy == true)
        {
            SettingPanel.SetActive(false);
        }
        else
        {
            SettingPanel.SetActive(true);
        }
    }

    public void CloseSettingPanel()
    {
        SettingPanel.SetActive(false);
    }

}
