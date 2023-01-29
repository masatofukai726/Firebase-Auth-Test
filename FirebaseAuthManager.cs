using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

public class FirebaseAuthManager : MonoBehaviour
{
    // Firebase Variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    // Login Variables
    [Space]
    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public InputField nameRegisterFiled;
    public InputField emailRegisterFiled;
    public InputField passwordRegisterFiled;
    public InputField confirmPasswordRegisterFiled;

    private void Start()
    {
        StartCoroutine(CheckAndFixDependenciesAsync());
    }

    private IEnumerator CheckAndFixDependenciesAsync()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        dependencyStatus = dependencyTask.Result;

        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
        }
        else
        {
            Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
        }
    }

    void InitializeFirebase()
    {
        //
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckForAutoLogin()
    {
        if(user != null)
        {
            var reloadUserTask = user.ReloadAsync();

            yield return new WaitUntil(() => reloadUserTask.IsCompleted);

            AutoLogin();
        }
        else
        {
            UIManager.Instance.OpenLoginPanel();
        }
    }

    private void AutoLogin()
    {
        if(user != null)
        {
            if(user.IsEmailVerified)
            {
                References.userName = user.DisplayName;
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
            }
            else
            {
                SendEmailVerification();
            }
        }
        else
        {
            UIManager.Instance.OpenLoginPanel();
        }
    }

    // 

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Logout();
                Debug.Log("Signed out " + user.UserId);
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
                UIManager.Instance.OpenLoginPanel();
                ClearLoginInputFieldText();
            }
            
            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Sigend in " + user.UserId);
            }
        }
    }

    private void ClearLoginInputFieldText()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void Logout()
    {
        if(auth != null && user != null)
        {
            auth.SignOut();
        }
    }

    public void Login()
    {
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failedMessage = "Login Failde! Because ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Wrong Password";
                    break;
                default:
                    failedMessage = "Login Failed";
                    break;
            }

            Debug.Log(failedMessage);
        }
        else 
        {
            user = loginTask.Result;

            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);

            if(user.IsEmailVerified)
            {
                References.userName = user.DisplayName;
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
            }
            else
            {
                SendEmailVerification();
            }
        }
    }

    public void Register()
    {
        StartCoroutine(RegisterAsync(nameRegisterFiled.text, emailRegisterFiled.text, passwordRegisterFiled.text, confirmPasswordRegisterFiled.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (name == "")
        {
            Debug.LogError("User Name is empty");
        }
        else if (email == "")
        {
            Debug.LogError("email field is empty");
        }
        else if (passwordRegisterFiled.text != confirmPasswordRegisterFiled.text)
        {
            Debug.LogError("Password does not match");
        }
        else 
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed! Because ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        break;
                    default:
                        failedMessage = "Registration Failed";
                        break;  
                }

                Debug.Log(failedMessage);
            }
            else
            {
               //
               user = registerTask.Result;

               UserProfile userProfile = new UserProfile { DisplayName = name } ;

               var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

               yield return new WaitUntil(() => updateProfileTask.IsCompleted);

               if (updateProfileTask.Exception != null)
               {
                    //
                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;


                    string failedMessage = "Profile update Failed! Because ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Wrong Password";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            break;
                        default:
                            failedMessage = "Registration Failed";
                            break;  
                    }

                    Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
                    if(user.IsEmailVerified)
                    {
                        UIManager.Instance.OpenLoginPanel();
                    }
                    else
                    {
                        SendEmailVerification();
                    }
                }
            }
        }
    }

    public void SendEmailVerification()
    {
        StartCoroutine(SendEmailVerificationAsync());
    }

    private IEnumerator SendEmailVerificationAsync()
    {
        if(user != null)
        {
            var sendEmailTask = user.SendEmailVerificationAsync();

            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            if(sendEmailTask.Exception != null)
            {
                FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError error = (AuthError)firebaseException.ErrorCode;

                string errorMessage = "Unknown Error : Please try again later";

                switch (error)
                {
                    case AuthError.Cancelled:
                    errorMessage = "Email Verification Was Cancelled";
                        break;
                    case AuthError.TooManyRequests:
                    errorMessage = "Too Many Request";
                        break;
                    case AuthError.InvalidRecipientEmail:
                    errorMessage = "The Email You Entered Is Invalid";
                        break;
                }

                UIManager.Instance.ShowVerificationResponse(false, user.Email, errorMessage);
            }
            else
            {
                Debug.Log("Email has successfully sent");
                UIManager.Instance.ShowVerificationResponse(true, user.Email, null);
            }
        }
    }
}
