using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentDataBase : MonoBehaviour
{
    private static PersistentDataBase pDB = null;

    public PlayerData p1Data;
    public PlayerData p2Data;

    private GameManager gm;

    private void Awake( )
    {
        if ( pDB != null && pDB != this )
        {
            Destroy( gameObject );
        }
        else
        {
            
            p1Data = new PlayerData( );
            p2Data = new PlayerData( );

            pDB = this;

            DontDestroyOnLoad( gameObject );
        }
    }

    // Use this for initialization
    void Start ()
    {


        gm = FindObjectOfType<GameManager>( );

        if ( gm == null )
        {
            return;
        }

        gm.pDB = this;

        if ( p1Data.money > -1 )
        {
            gm.playerMoney[0].moneyAmnt = p1Data.money;
            gm.playerMoney[0].moneyTxt.text = "$" + p1Data.money.ToString( );
        }

        if ( p2Data.money > -1 )
        {
            gm.playerMoney[1].moneyAmnt = p2Data.money;
            gm.playerMoney[1].moneyTxt.text = "$" + p2Data.money.ToString( );
        }

        if ( p1Data.Speed > -1 )
        {
            gm.playerController[0].speedMultiplier = p1Data.Speed;
        }

        if ( p2Data.Speed > -1 )
        {
            gm.playerController[1].speedMultiplier = p2Data.Speed;
        }

        if ( p1Data.RotSpeed > -1 )
        {
            gm.playerController[0].rotateMultiplier = p1Data.RotSpeed;
        }

        if ( p2Data.RotSpeed > -1 )
        {
            gm.playerController[1].rotateMultiplier = p2Data.RotSpeed;
        }
    }

    private void OnEnable( )
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable( )
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading( Scene scene, LoadSceneMode mode )
    {
        
        gm = FindObjectOfType<GameManager>( );

        if ( gm == null )
        {
            return;
        }

        gm.pDB = this;

        if ( p1Data.money > -1 )
        {
            gm.playerMoney[0].moneyAmnt = p1Data.money;
            gm.playerMoney[0].moneyTxt.text = "$" + p1Data.money.ToString( );
        }

        if ( p2Data.money > -1 )
        {
            gm.playerMoney[1].moneyAmnt = p2Data.money;
            gm.playerMoney[1].moneyTxt.text = "$" + p2Data.money.ToString( );
        }

        if ( p1Data.Speed > -1 )
        {
            gm.playerController[0].speedMultiplier = p1Data.Speed;
        }

        if ( p2Data.Speed > -1 )
        {
            gm.playerController[1].speedMultiplier = p2Data.Speed;
        }

        if ( p1Data.RotSpeed > -1 )
        {
            gm.playerController[0].rotateMultiplier = p1Data.RotSpeed;
        }

        if ( p2Data.RotSpeed > -1 )
        {
            gm.playerController[1].rotateMultiplier = p2Data.RotSpeed;
        }
    }
}

public class PlayerData
{
    public int money = -1;
    public float RotSpeed = -1;
    public float Speed = -1;
}
