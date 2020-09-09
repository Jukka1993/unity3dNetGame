using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour {
    InputField nameInputField;
    InputField passwordInputField;
    Button loginButton;
    Button registerButton;
    void Start()
    {
        nameInputField = transform.Find("NameInputField").GetComponent<InputField>();
        passwordInputField = transform.Find("PasswordInputField").GetComponent<InputField>();
        loginButton = transform.Find("LoginButton").GetComponent<Button>();
        registerButton = transform.Find("RegisterButton").GetComponent<Button>();
        loginButton.onClick.AddListener(OnLoginClick);
        registerButton.onClick.AddListener(OnRegisterButton);
    }
    public void OnLoginClick()
    {
        Debug.Log("login button clicked, name = "+nameInputField.text + ", passwordInputField = "+passwordInputField.text);
    }
    public void OnRegisterButton()
    {
        Debug.Log("register button clicked =  " + nameInputField.text + ", passwordInputField = " + passwordInputField.text);
    }
}
