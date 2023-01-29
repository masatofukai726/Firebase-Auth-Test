using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject loginPanel;
    public Text warningLoginText;
    public Text confirmLoginText;

    [SerializeField]
    private GameObject registrationPanel;

    [SerializeField]
    private GameObject emailVerificationPanel;
    public Text emailVerificationText;

    private void Awake()
    {
        CreateInstance();
    }

    public void CreateInstance()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        registrationPanel.SetActive(false);
        emailVerificationPanel.SetActive(false);
    }

    public void OpenRegistraionPanel()
    {
        loginPanel.SetActive(false);
        registrationPanel.SetActive(true);
        emailVerificationPanel.SetActive(false);
    }

    public void ShowDebugLogLogin(bool isWarningLogin, string warningMessage, string confirmMessage)
    {
        if(isWarningLogin)
        {
            warningLoginText.text = $"{warningMessage}";
        }
        else
        {
            confirmLoginText.text = $"{confirmMessage}";
        }

    }

    public void ShowVerificationResponse(bool isEmailSent, string emailId, string errorMessage)
    {
        loginPanel.SetActive(false);
        registrationPanel.SetActive(false);
        emailVerificationPanel.SetActive(true);

        if(isEmailSent)
        {
            emailVerificationText.text = $"Please verify your email address \n Verification email has been sent to {emailId}";
        }
        else
        {
            emailVerificationText.text = $"Couldn't sent email : {errorMessage}";

        }
    }
}
