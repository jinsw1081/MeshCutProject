using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

/// <summary>
/// Playfab 로그인을 하는 클래스 
/// Playfab은 로그인이 기본이며 로그인을 하지않으면 실행할수없습니다
/// 여기서 하는 로그인은 게임 내부 로그인이라서 Playfab 상관이 없습니다 
/// 나머지는 playfab 설명서에 적혀있습니다. 
/// </summary>

public class PlayFabLogin : MonoBehaviour
{
    private string userEmail="jin152@naver.com";
    private string userPassword = "9876543";
    private string userName = "Noob";

    public void Awake()
    {
        //강제로 타이틀 아디와 개발자 시크릿키를 할당해준 부분 에디터에서 할당해줄수 있지만 
        //제대로 할당이 되지않아서 오류가 여러번 발생했으므로 아래처럼 강제로 할당해주었습니다.
        PlayFabSettings.TitleId = "AE39A";
        PlayFabSettings.DeveloperSecretKey = "QINJDI3H15ER5U13HNJI9TGYPKYHREIIDFNKFQFHAWPUYP5O9G";
        OnClickLogin();
    }
    void OnRegistSucees(RegisterPlayFabUserResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    }

    //아이디가 없을경우 새롭게 가입해주어야합니다
    void RegistNow()
    {
        RegisterPlayFabUserRequest registRequest = new RegisterPlayFabUserRequest
        {
            Email = userEmail,
            Password = userPassword,
            Username = userName
        };
        PlayFabClientAPI.RegisterPlayFabUser(registRequest, OnRegistSucees, OnRegistFailed);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    void OnRegistFailed(PlayFabError result)
    {
        Debug.LogError(result.GenerateErrorReport());
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("LoginSucces");
    }
    
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    
    //로그인 함수
    public void OnClickLogin()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

}