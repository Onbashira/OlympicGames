using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameResultManager
{

    [SerializeField]
    private static List<int> playerRankList;

    private static List<int> GetPlayerRank()
    {
        return playerRankList;
    }

}
