
using UnityEngine;

public static class UsernameManager
{
    private const string UsernameKey = "PlayerUsername";

    public static void SaveUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return;

        PlayerPrefs.SetString(UsernameKey, username.Trim());
        PlayerPrefs.Save();
    }

    public static string GetUsername()
    {
        return PlayerPrefs.GetString(UsernameKey, "Player");
    }

    public static bool HasUsername()
    {
        return PlayerPrefs.HasKey(UsernameKey) &&
               !string.IsNullOrWhiteSpace(
                   PlayerPrefs.GetString(UsernameKey, "")
               );
    }
}
