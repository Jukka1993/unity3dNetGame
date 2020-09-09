using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterView : MonoBehaviour {
    private InputField nameInputField;
    //private InputField nameInputField1;
    private InputField passwordInputField;
    private InputField passwordInputField1;

    private Button registerButton;
    private Button closeButton;

    public void Awake()
    {
        nameInputField = transform.Find("NameInputField").GetComponent<InputField>();
        passwordInputField = transform.Find("PasswordInputField").GetComponent<InputField>();
        passwordInputField1 = transform.Find("PasswordInputField1").GetComponent<InputField>();

        registerButton = transform.Find("RegisterButton").GetComponent<Button>();
        closeButton = transform.Find("CloseButton").GetComponent<Button>();

        registerButton.onClick.AddListener(OnRegisterClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }
    void OnRegisterClicked()
    {
        Debug.Log("RegisterView onregister button clicked " + nameInputField.text + " " + passwordInputField.text + " " + passwordInputField1.text);
    }
    void OnCloseClicked()
    {
        Debug.Log("RegisterView onClose button clicked " + nameInputField.text + " " + passwordInputField.text + " " + passwordInputField1.text);
    }
}
