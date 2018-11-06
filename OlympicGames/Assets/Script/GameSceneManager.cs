using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{

    [SerializeField, Tooltip("GameUpdatePreUpdateで要する時間を秒単位で指定")]
    private float maxPreUpdateTime = 1.0f; //一秒
    [SerializeField, Tooltip("GameUpdateStartGameで要する時間を秒単位で指定")]
    private float maxStartGameTime = 1.0f; //一秒
    [SerializeField, Tooltip("GameUpdateGameSetで要する時間を秒単位で指定")]
    private float maxGameSetTime = 1.0f; //一秒
    [SerializeField, Tooltip("GameUpdateGameEndで要する時間を秒単位で指定")]
    private float maxGameEndTime = 1.0f; //一秒

    [SerializeField, Tooltip("ゲームが進行した時間")]
    private float gameStepTime = 0.0f; //一秒
    [SerializeField, Tooltip("ゲームの開始フラグ")]
    private bool isPreUpdatePassed = false; //一秒
    [SerializeField, Tooltip("ゲームの開始フラグ")]
    private bool isGameStartPassed = false; //一秒
    [SerializeField, Tooltip("ゲームの開始フラグ")]
    private bool isGameSetPassed = false; //一秒
    [SerializeField, Tooltip("ゲームの開始フラグ")]
    private bool isGameEndPassed = false; //一秒

    System.Action gameUpdater;
    Camera mainCamera;
    List<PlayerController> players;
    CanvasGroup UIcanvas;


    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        gameUpdater();

    }

    void Initialize()
    {
        gameUpdater = GameUpdatePreUpdate;
        isPreUpdatePassed = false; //一秒
        isGameStartPassed = false; //一秒
        isGameSetPassed = false; //一秒
        isGameEndPassed = false; //一秒
        //接続されているコントローラーからプレイヤーを生成する
    }


    //ゲームが始まる前までの処理
    void GameUpdatePreUpdate()
    {
        gameStepTime += Time.deltaTime;
        if (gameStepTime >= maxPreUpdateTime)
        {
            gameStepTime = 0.0f;
            isPreUpdatePassed = true;
            gameUpdater = GameUpdateStartGame;
        }
    }
    //ゲームのスタートを告げる（カットインとか）処理
    void GameUpdateStartGame()
    {
        gameStepTime += Time.deltaTime;
        if (gameStepTime >= maxStartGameTime)
        {
            gameStepTime = 0.0f;
            isGameStartPassed = true;

            gameUpdater = GameUpdateNormal;
        }
    }
    //ゲームの通常アップデート　もしもこのアップデート中にプレイヤーの残機がゼロになるなどでゲームセットフェーズに移行
    void GameUpdateNormal()
    {
        
    }

    //ゲームの終了を告げるカットイン
    void GameUpdateGameSet()
    {
        gameStepTime += Time.deltaTime;
        if (gameStepTime >= maxGameSetTime)
        {
            gameStepTime = 0.0f;
            isGameSetPassed = true;

            gameUpdater = GameUpdateNormal;
        }
    }

    //全ての入力を切ってフェードフラグをたたせる
    void GameUpdateGameEnd()
    {
        gameStepTime += Time.deltaTime;
        if (gameStepTime >= maxGameEndTime)
        {
            gameStepTime = 0.0f;
            isGameEndPassed = true;

            gameUpdater = GameUpdateNormal;
        }
    }

    void GameUpdate()
    {

    }
}
