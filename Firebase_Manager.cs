using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class Firebase_Manager : MonoBehaviour{
    //Firebase variables
    [Header("Firebase")]
    public static DependencyStatus dependencyStatus;
    public static FirebaseAuth auth;    
    public static FirebaseUser User;
    public static DatabaseReference DBreference;
    public static DataSnapshot snapshot;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header("Register")]
    public TMP_InputField TeacherUsernameRegisterField;
    public TMP_InputField TeacherEmailRegisterField;
    public TMP_InputField TeacherPasswordRegisterField;
    public TMP_InputField TeacherPasswordRegisterVerifyField;
    public TMP_Text warningTeacherRegisterText;

    private void Awake(){
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available){
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else{
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase(){
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Setting up DBref");
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("finish Setting up DBref");
    }

    public void LoginButton(){
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton(){
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void RegisterAsTeacherButton(){
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(RegisterAsTeacher(TeacherEmailRegisterField.text, TeacherPasswordRegisterField.text, TeacherUsernameRegisterField.text));
    }

    private IEnumerator Login(string _email, string _password){
        //Call the Firebase auth signin function passing the email and password
        var LoginPage = "";
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null){
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode){
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
            yield return new WaitForSeconds(2);
            warningLoginText.text = "";
        }else{
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            
            var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();
            
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
             Debug.Log("DBTask:"+DBTask.Result);

            if (DBTask.Exception != null) {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else if (DBTask.Result.Value == null){
            //No data exists yet 
            
            }else{
            
            //Data has been retrieved
            snapshot = DBTask.Result;
            if(snapshot.Child("UserType").Value.ToString().Equals("student")){
                LoginPage = "MainMenuScene";
            };
            if(snapshot.Child("UserType").Value.ToString().Equals("teacher")){
                LoginPage = "TeacherMenuScene";
            };
           
        }
           
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";

            ClearLoginFields();
            ClearRegisterFields();
            SceneManager.LoadScene(LoginPage);

            yield return new WaitForSeconds(2);

            confirmLoginText.text = "";

        }
    }

    public void ClearLoginFields(){
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFields(){
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

     private IEnumerator Register(string _email, string _password, string _username){
        if (_username == ""){
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
            yield return new WaitForSeconds(2);
            warningRegisterText.text = "";
            

        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text) {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
            yield return new WaitForSeconds(2);
            warningRegisterText.text = "";
        }else {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null){
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode){
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
                yield return new WaitForSeconds(2);
                warningRegisterText.text = "";
            }else{
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;



                if (User != null){
                    
                    StartCoroutine(writeNewUser(User.UserId, _username));
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _username};

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
                    
                    if (ProfileTask.Exception != null){
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                        yield return new WaitForSeconds(2);
                        warningRegisterText.text = "";
                    }else{
                        //Username is now set
                        //Now return to login screen
                        UI_Manager.instance.LoginScreenON();
                        warningRegisterText.text = "";
                        
                    }


                }
            }
        }
    }

    private IEnumerator RegisterAsTeacher(string _email, string _password, string _username){
        if (_username == ""){
            //If the username field is blank show a warning
            warningTeacherRegisterText.text = "Missing Username";
            yield return new WaitForSeconds(2);
            warningTeacherRegisterText.text = "";
            

        }
        else if(TeacherPasswordRegisterField.text != TeacherPasswordRegisterVerifyField.text) {
            //If the password does not match show a warning
            warningTeacherRegisterText.text = "Password Does Not Match!";
            yield return new WaitForSeconds(2);
            warningTeacherRegisterText.text = "";
        }else {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null){
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode){
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
                warningTeacherRegisterText.text = message;
                yield return new WaitForSeconds(2);
                warningTeacherRegisterText.text = "";
            }else{
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;



                if (User != null){
                    
                    StartCoroutine(writeNewTeacher(User.UserId, _username));
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _username};

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
                    
                    if (ProfileTask.Exception != null){
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningTeacherRegisterText.text = "Username Set Failed!";
                        yield return new WaitForSeconds(2);
                        warningTeacherRegisterText.text = "";
                    }else{
                        //Username is now set
                        //Now return to login screen
                        UI_Manager.instance.LoginScreenON();
                        warningTeacherRegisterText.text = "";
                        
                    }


                }
            }
        }
    }

     private IEnumerator UpdateUsernameAuth(string _username){
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }        
    }

    private IEnumerator UpdateUsernameDatabase(string _username){
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator writeNewUser(string userId, string name) {
        UserInitalData user = new UserInitalData(name,"student");
        string json = JsonUtility.ToJson(user);
       

        var DBTask = DBreference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
        

    }

     private IEnumerator writeNewTeacher(string userId, string name) {
        TeacherInitalData teacher = new TeacherInitalData(name,"teacher");
        string json = JsonUtility.ToJson(teacher);
       

        var DBTask = DBreference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
        

    }



}