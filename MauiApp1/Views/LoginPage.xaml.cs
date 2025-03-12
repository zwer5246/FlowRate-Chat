using WebSocketSharp;
using MauiApp1.Classes;
namespace MauiApp1.Views;

public partial class LoginPage : ContentPage
{
    ChatPage chatPage;

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        AuthStarted();

        chatPage = new ChatPage(LoginEntry.Text, PasswordEntry.Text);

        await Navigation.PushAsync(chatPage);

        AuthEnded();

        LoginCancelButton.IsEnabled = true;
        LoginButton.IsEnabled = false;
        LoginEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = false;
        RememberMeSwitch.IsEnabled = false;
    }

    private void AuthStarted()
    {
        LoginEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = false;
        LoginButton.IsEnabled = false;
        ServerProperitesButton.IsEnabled = false;
        LoginCancelButton.IsEnabled = false;
        GoogleOAuthButton.IsEnabled = false;
        VKOAuthButton.IsEnabled = false;
        RememberMeSwitch.IsEnabled = false;
    }

    private void AuthEnded()
    {
        LoginEntry.IsEnabled = true;
        PasswordEntry.IsEnabled = true;
        LoginButton.IsEnabled = true;
        ServerProperitesButton.IsEnabled = true;
        LoginCancelButton.IsEnabled = true;
        GoogleOAuthButton.IsEnabled = true;
        VKOAuthButton.IsEnabled = true;
        RememberMeSwitch.IsEnabled = true;
    }

    public void LoginCancelButton_Clicked(object sender, EventArgs e)
    {
        LoginCancelButton.IsEnabled = false;
        LoginButton.IsEnabled = true;
        LoginEntry.IsEnabled = true;
        PasswordEntry.IsEnabled = true;
        RememberMeSwitch.IsEnabled = true;
        chatPage.Disconnect();
        Navigation.RemovePage(chatPage);
        chatPage = null;
    }
}