using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLance : IEnemy
{

    private enum State
    {
        MoveIntoScreen,
        SpinRandom,   //Spin like clock and end spin with player targeted
        SpinToPlayer, //Rotate To player
        MoveBack, //do a little step back
        FlyToPlayer //fly to player fast
    }

    private State state;
    [SerializeField] private float rotationSpeedEulerAngles = 10f;
    [SerializeField] private float rotationSpeed = 1f;
    Player player;

    private Vector2 screenBounds;
    private Vector2 target;
    private Vector3 direction;

    [SerializeField] private float spinDuration = 3f;
    private float spinTimer;

    private float finalRotateToPlayer = 0.3f;
    private float finalRotateTimer;

    private float stepBackDuration = 0.15f;
    private float stepBackTimer;

    private float x;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        state = State.MoveIntoScreen;
        if(Player.Instance != null)
        {
            player = Player.Instance;
        }
        else
        {
            Debug.Log("Player instance is null");
        }
        spinTimer = spinDuration;
        finalRotateTimer = finalRotateToPlayer;
        stepBackTimer = stepBackDuration;
    }
    
    void Update()
    {
        switch (state)
        {
            case State.MoveIntoScreen:
                if(transform.position.x < -screenBounds.x)
                {
                    x = -screenBounds.x + 2;
                    direction = new Vector2(1, 0);
                }else if (transform.position.x > screenBounds.x)
                {
                    x = screenBounds.x - 2;
                    direction = new Vector2(-1, 0);
                }

                transform.up = direction;
                target = new Vector2(x,transform.position.y);
                MoveTo(target);
                if (ReachedTarget(target))
                {
                    state = State.SpinRandom;
                    SoundManager.PlaySound(SoundManager.Sound.LanceRotateSound);
                }
                break;
            case State.SpinRandom:
                if(spinTimer > 0f)
                {
                    //spin
                    transform.Rotate(0, 0, rotationSpeedEulerAngles * Time.deltaTime);
                    spinTimer -= Time.deltaTime;
                }
                else
                {                 
                    state = State.SpinToPlayer;
                    spinTimer = spinDuration;
                }
                
                break;
            case State.SpinToPlayer:
                //target player
                if (player != null)
                {
                    direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, 0);
                }

                if(finalRotateTimer > 0f)
                {
                    transform.up = Vector3.RotateTowards(transform.up, direction, rotationSpeed * Time.deltaTime, 0.0f);
                    finalRotateTimer -= Time.deltaTime;
                }
                else
                {
                    transform.up = direction;
                    finalRotateTimer = finalRotateToPlayer;
                    state = State.MoveBack;
                }
                break;
            case State.MoveBack:    
                if(stepBackTimer > 0f)
                {
                    target = new Vector2(transform.position.x - direction.x, transform.position.y - direction.y);
                    MoveTo(target);
                    stepBackTimer -= Time.deltaTime;
                }
                else
                {
                    if(player != null)
                    {
                        target = player.transform.position;
                    }
                    state = State.FlyToPlayer;
                    stepBackTimer = stepBackDuration;
                }
                break;
            case State.FlyToPlayer:
                MoveTo(target);

                if (ReachedTarget(target))
                {
                    state = State.SpinRandom;
                    SoundManager.PlaySound(SoundManager.Sound.LanceRotateSound);
                }
                break;
        }
    }
}
