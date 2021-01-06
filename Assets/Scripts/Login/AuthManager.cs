using System.Collections;
using Firebase;
using Firebase.Auth;
using Login.HandleScene;
using Server;
using ServerSQL;
using SQL;
using SQL.Model;
using TMPro;
using UnityEngine;

namespace Login {
    public class AuthManager : MonoBehaviour {
        [Header("Firebase")] public DependencyStatus dependencyStatus;
        private FirebaseAuth m_auth;
        private FirebaseUser m_user;
        [Header("Login")] public TMP_InputField emailLoginField;
        public TMP_InputField passwordLoginField;
        public TMP_Text warningLoginText;
        public TMP_Text confirmLoginText;
        [Header("Register")] public TMP_InputField userNameRegisterField;
        public TMP_InputField emailRegisterField;
        public TMP_InputField passwordRegisterField;
        public TMP_InputField passwordRegisterVerifyField;
        public TMP_Text warningRegisterText;
        public TMP_Text confirmRegistrationText;

        void Awake() {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available) {
                    InitializeFirebase();
                } else {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        private void InitializeFirebase() {
            Debug.Log("Setting up Firebase Auth");
            m_auth = FirebaseAuth.DefaultInstance;
        }
        public void LoginButton() {
            // StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
            StartCoroutine(Login("test@test.com", "123456"));
        }
        public void RegisterButton() { 
            StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, userNameRegisterField.text));
        }
        private IEnumerator Login(string email, string password) {
            var loginTask = m_auth.SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(predicate: () => loginTask.IsCompleted);
            if (loginTask.Exception != null) {
                Debug.LogWarning(message: $"Failed to register task with {loginTask.Exception}");
                FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Login Failed!";
                switch (errorCode) {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WrongPassword:
                        message = "Wrong Password";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email";
                        break;
                    case AuthError.UserNotFound:
                        message = "Account does not exist";
                        break;
                }
                warningLoginText.text = message;
            } else {
                m_user = loginTask.Result;
                GetLocalUserData();
                DatabaseId databaseID = GetComponent<DatabaseId>();
                databaseID.Id = m_user.UserId;
                LoginSceneHandler.SwitchToCharSelectScene();
                Debug.LogFormat("User signed in successfully: {0} ({1})", m_user.DisplayName, m_user.Email);
                warningLoginText.text = "";
                confirmLoginText.text = "Logged In";
            }
        }
        private void GetLocalUserData() {
            UserAccount userAccount = new UserAccount();
            userAccount.Id = m_user.UserId;
            
            DataService dataService = new DataService("MMORPG");
            // dataService.CreateDB();
            var result = dataService.LoginOrCreate(userAccount);
            print(result);
        }
        private IEnumerator Register(string email, string password, string username) {
            print("Getter here");
            if (username == "") {
                warningRegisterText.text = "Missing Username";
            } 
            else if (passwordRegisterField.text != passwordRegisterVerifyField.text) {
                warningRegisterText.text = "Password Does Not Match!";
            } else {
                var registerTask = m_auth.CreateUserWithEmailAndPasswordAsync(email, password);
                yield return new WaitUntil(predicate: () => registerTask.IsCompleted);
                if (registerTask.Exception != null) {
                    Debug.LogWarning(message: $"Failed to register task with {registerTask.Exception}");
                    FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError) firebaseEx.ErrorCode;
                    string message = "Register Failed!";
                    switch (errorCode) {
                        case AuthError.MissingEmail:
                            message = "Missing Email";
                            break;
                        case AuthError.MissingPassword:
                            message = "Missing Password";
                            break;
                        case AuthError.WeakPassword:
                            message = "Weak Password";
                            break;
                        case AuthError.EmailAlreadyInUse:
                            message = "Email Already In Use";
                            break;
                    }
                    warningRegisterText.text = message;
                } else {
                    m_user = registerTask.Result;
                    if (m_user == null) yield break;
                    UserProfile profile = new UserProfile {DisplayName = username};
                    var profileTask = m_user.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => profileTask.IsCompleted);
                    if (profileTask.Exception != null) {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {profileTask.Exception}");
                        FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError) firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    } else {
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }
}