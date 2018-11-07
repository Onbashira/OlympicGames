using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameResultManager
{

    //Rank順に格納されたリスト
    [SerializeField]
    private static List<int> playerRankList;

    public static List<int> GetPlayerRank()
    {
        return playerRankList;
    }
    public static int GetPlayerRank(int rank)
    {
        if (rank > 0 && rank < ControllerFetcher.GetMaxConectedController())
        {
            return playerRankList[rank];
        }
        return -1;
    }

    public static void Initialize()
    {
        playerRankList.Clear();
    }

}
