using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public string googleIdToken = "";
    public string googleAccessToken = "";
    private void Start()
    {
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
}
