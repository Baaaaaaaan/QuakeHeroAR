
using UnityEngine;

public static class RoomProgressManager
{
    private const string UnlockedPrefix = "QuakeHero_RoomUnlocked_";
    private const string CompletedPrefix = "QuakeHero_RoomCompleted_";
    private const string BestScorePrefix = "QuakeHero_RoomBestScore_";

    // These are the first available rooms in each environment.
    public static void UnlockStartingRooms()
    {
        UnlockRoom("Bedroom");
        UnlockRoom("Playground");
        UnlockRoom("Classroom");
    }

    public static void UnlockRoom(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        PlayerPrefs.SetInt(UnlockedPrefix + sceneName, 1);
        PlayerPrefs.Save();
    }

    public static bool IsRoomUnlocked(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return false;

        // Starting rooms are always available.
        if (sceneName == "Bedroom" ||
            sceneName == "Playground" ||
            sceneName == "Classroom")
        {
            return true;
        }

        return PlayerPrefs.GetInt(
            UnlockedPrefix + sceneName, 0
        ) == 1;
    }

    public static void RecordRoomResult(
        string roomSceneName,
        int score,
        string nextSceneName,
        bool passed)
    {
        if (string.IsNullOrWhiteSpace(roomSceneName))
            return;

        // Save the player's highest quiz score for this room.
        int previousBest = PlayerPrefs.GetInt(
            BestScorePrefix + roomSceneName, 0
        );

        if (score > previousBest)
        {
            PlayerPrefs.SetInt(
                BestScorePrefix + roomSceneName, score
            );
        }

        // Only a passing result completes the room.
        if (passed)
        {
            PlayerPrefs.SetInt(
                CompletedPrefix + roomSceneName, 1
            );

            // Unlock the next room configured in RoomData.
            if (!string.IsNullOrWhiteSpace(nextSceneName))
            {
                UnlockRoom(nextSceneName);

                Debug.Log(
                    "Room unlocked: " + nextSceneName
                );
            }
        }

        PlayerPrefs.Save();

        Debug.Log(
            "Room progress saved: " + roomSceneName +
            " | Score: " + score +
            " | Passed: " + passed
        );
    }

    public static bool IsRoomCompleted(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return false;

        return PlayerPrefs.GetInt(
            CompletedPrefix + sceneName, 0
        ) == 1;
    }

    public static int GetBestScore(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return 0;

        return PlayerPrefs.GetInt(
            BestScorePrefix + sceneName, 0
        );
    }
}
