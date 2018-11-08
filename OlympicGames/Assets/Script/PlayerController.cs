using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;


public class PlayerController : MonoBehaviour
{

    Rigidbody2D rigid;
    [SerializeField]
    float movePower = 1.0f;
    [SerializeField]
    float boostPower = 2.0f;
    [SerializeField]
    float AttackPower = 5.0f;
    [SerializeField]
    private Vector2 moveVelocity = Vector2.zero;
    [SerializeField]
    private float velocityClamp;
    [SerializeField]
    float hoverMovePower = 1.0f;
    [SerializeField]
    private float hoverClamp;
    [SerializeField]
    private float angulerVelocity = 0.0f;
    [SerializeField]
    float torquePower = 1.0f;
    [SerializeField]
    float torqueClamp;
    [SerializeField]
    float brakeDump = 0.1f;
    [SerializeField]
    float brakeAngulerDump = 0.1f;
    [SerializeField]
    GamepadInput.GamePad.Index playerNo;
    [SerializeField]
    public uint playerStock = 0;
    Vector2 deadPos = Vector2.zero;
    Vector3 deadRotation = Vector3.zero;
    [SerializeField]
    bool isDead = false;
    [SerializeField]
    bool isDown = false;
    [SerializeField]
    bool isInvincible = false;
    [SerializeField]
    bool isCollided = false;

    [SerializeField]
    Vector2 respownPos;
    [SerializeField]
    float respownRotate;
    [SerializeField]
    float downTime = 3.0f;
    float downtimer = 0.0f;
    [SerializeField]
    float invincibleTime = 3.0f;
    float invincibleTimer = 0.0f;
    [SerializeField]
    float blinkDownTime = 0.1f;
    [SerializeField]
    float blinkInvincibleTime = 0.4f;
    float blinkTimer = 0.0f;
    [SerializeField]
    float attackTime = 0.5f;
    float attackTimer = 0.0f;
    [SerializeField]
    float attackTime2 = 1.0f;
    float attackTimer2 = 0.0f;
    [SerializeField]
    float respawnTime = 1.0f;
    float respawnTimer = 0.0f;
    [SerializeField]
    private GamepadInput.GamepadState gamepadState;
    [SerializeField]
    private GamepadInput.GamepadState gamepadStateOld;
    [SerializeField]
    AudioSource deadSE;
    [SerializeField]
    AudioSource boostSE;
    [SerializeField]
    AudioSource moveSE;
    [SerializeField]
    AudioSource downSE;
    [SerializeField]
    AudioSource slashSE;

    SpriteRenderer spriteRenderer = null;

    System.Action inputUpdater;
    System.Action moveUpdater;
    System.Action stateUpdater;
    System.Action attackSeq;

    [SerializeField]
    CapsuleCollider2D slasherCollider = null;

    public void Initialized()
    {
        isCollided = false;
        moveVelocity = Vector2.zero;
        angulerVelocity = 0.0f;
        stateUpdater = Normal;
        inputUpdater = GamePadStateUpdate;
        gamepadStateOld = gamepadState = GamepadInput.GamePad.GetState(playerNo);
        angulerVelocity = 0.0f;
    }

    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        rigid = this.GetComponent<Rigidbody2D>();
        //slasherCollider = this.GetComponentInChildren<CapsuleCollider2D>();
        slasherCollider.enabled = false;
        Initialized();
    }

    // Update is called once per frame
    void Update()
    {
        inputUpdater();
        stateUpdater();
        ClampVelocity();
        ClapmAngluerVelocity();
    }

    private void LateUpdate()
    {

    }

    private void FixedUpdate()
    {

    }

    public bool IsColl()
    {
        return isCollided;
    }

    public void CollReset()
    {
        isCollided = false;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool IsDown()
    {
        return isDown;
    }

    public void Kill()
    {
        isDead = true;
    }

    public void SetPlayerNO(int index)
    {
        playerNo = (GamepadInput.GamePad.Index)index;
    }

    public uint GetPlayerNO()
    {
        return (uint)playerNo;
    }

    public uint GetPlayerStock()
    {
        return (uint)playerStock;
    }

    public void SetRespownParamater(Vector2 pos, float rotate)
    {

        this.respownPos = pos;
        this.respownRotate = rotate;
    }

    public void SetUpdaterToWait()
    {
        stateUpdater = Wait;
    }

    public void SetUpdaterToNormal()
    {
        stateUpdater = Normal;
    }

    void ClampVelocity()
    {
        moveVelocity = rigid.velocity;
        moveVelocity.x = Mathf.Clamp(moveVelocity.x, -velocityClamp, velocityClamp);
        moveVelocity.y = Mathf.Clamp(moveVelocity.y, -velocityClamp, velocityClamp);
        rigid.velocity = moveVelocity;
    }

    void ClapmAngluerVelocity()
    {
        rigid.angularVelocity = Mathf.Clamp(rigid.angularVelocity, -torqueClamp, torqueClamp);
        this.angulerVelocity = rigid.angularVelocity;

    }

    void Brake()
    {
        DumpBrake();

        if (!gamepadState.B)
        {
            stateUpdater = Normal;
            return;
        }

        Vector2 tempVelo = Vector2.zero;

        if (gamepadState.Left)
        {
            tempVelo += Vector2.left;
        }
        if (gamepadState.Up)
        {
            tempVelo += Vector2.up;
        }
        if (gamepadState.Right)
        {
            tempVelo += Vector2.right;
        }
        if (gamepadState.Down)
        {
            tempVelo += Vector2.down;
        }

        tempVelo.Normalize();
        tempVelo = tempVelo * hoverMovePower * Time.deltaTime;
        tempVelo.x = Mathf.Clamp(tempVelo.x, -hoverClamp, hoverClamp);
        tempVelo.y = Mathf.Clamp(tempVelo.y, -hoverClamp, hoverClamp);

        rigid.AddRelativeForce((tempVelo), ForceMode2D.Force);
        RotatePlayer();
    }

    void Move()
    {
        Vector2 tempVelo = (Vector2.up * movePower);
        rigid.AddRelativeForce(tempVelo, ForceMode2D.Impulse);
        //Instantiate(Resources.Load("boostSmoke"),this.transform.position,this.transform.rotation);
        moveVelocity = rigid.velocity;
        stateUpdater = Normal;
    }

    void DumpBrake(float amount = 1.0f)
    {
        if (!(rigid.velocity.SqrMagnitude().Equals(0.0f)))
        {
            Vector2 dump = rigid.velocity * brakeDump * amount * Time.deltaTime;
            rigid.velocity -= dump;
        }
        if (rigid.velocity.SqrMagnitude() < 0.00001f)
        {
            rigid.velocity = Vector2.zero;
        }
    }

    void DumpAngleBrake(float amount = 1.0f)
    {
        if (!(rigid.angularVelocity.Equals(0.0f)))
        {
            float dump = rigid.angularVelocity * brakeAngulerDump * amount * Time.deltaTime;
            rigid.angularVelocity -= dump;
        }
        if (rigid.angularVelocity < 0.0001f)
        {
            rigid.angularVelocity = 0.0f;
        }
    }

    void Attack()
    {
        Vector2 boostVelo = (Vector2.up * boostPower);

        rigid.AddRelativeForce(boostVelo, ForceMode2D.Impulse);

        // StartCoroutine(AttakerUpdate(1.0f));
        stateUpdater = AttackAction1;
        RotatePlayer();
    }

    //急速前進減衰あり
    void AttackAction1()
    {
        DumpBrake(2.0f);
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackTime)
        {
            rigid.angularVelocity = 0.0f;


            slasherCollider.enabled = true;
            attackTimer = 0.0f;
            stateUpdater = AttackAction2;
            return;
        }
    }

    //切りつけ回転
    void AttackAction2()
    {
        DumpBrake(2.0f);
        attackTimer += Time.deltaTime;
        transform.Rotate(new Vector3(0.0f, 0.0f, 12.0f));

        if (attackTimer >= attackTime2)
        {
            slasherCollider.enabled = false;
            attackTimer = 0.0f;
            stateUpdater = Normal;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDown || !isDead || !isInvincible)
        {
            //他プレイヤーとの衝突時
            if (collision.gameObject.tag == "Player")
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();


                if (player.moveVelocity.SqrMagnitude() >= moveVelocity.SqrMagnitude())
                {

                    rigid.velocity += player.rigid.velocity / 2.0f;
                    player.rigid.velocity /= 2.0f;
                }


            }
            //他プレイヤー攻撃との衝突時
            if (collision.gameObject.tag == "AttackColl")
            {
                PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();



                var vec = -(player.transform.position - this.transform.position).normalized;
                this.rigid.velocity = vec * AttackPower;

                slasherCollider.enabled = false;
                attackTime = 0.0f;
            }


            if (collision.gameObject.tag == "Wall") //壁との衝突時
            {
                slasherCollider.enabled = false;

                isCollided = true;
                --playerStock;
                deadPos = transform.position;
                deadRotation = transform.rotation.eulerAngles;
                stateUpdater = Dead;

            }
        }
    }

    void GameOver()
    {

    }

    public void DeadPlayer()
    {
        this.enabled = false;
        spriteRenderer.enabled = false;
        GameResultManager.GetPlayerRank().Add((int)this.playerNo);
        stateUpdater = GameOver;
        isDead = true;
    }

    void Dead()
    {
        if (playerStock <= 0)
        {
            stateUpdater = GameOver;
            isDead = true;
            GameResultManager.GetPlayerRank().Add((int)this.playerNo);
           
            //Destroy(this);
        }
        else
        {
            this.rigid.velocity = Vector2.zero;

            stateUpdater = Respawn;
            isInvincible = true;
            StartCoroutine(Invincible());
        }

    }

    void Respawn()
    {
        respawnTimer += Time.deltaTime;
        if (respawnTimer >= 1.0f)
        {
            respawnTimer= 1.0f;
            this.transform.position = Vector3.Lerp(deadPos, respownPos, respawnTimer);
            this.transform.eulerAngles = Vector3.Slerp(deadRotation, new Vector3(0.0f,0.0f,respownRotate), respawnTimer);
            respawnTimer = 0.0f;
            stateUpdater = Normal;
            return;
        }
        this.transform.position = Vector3.Lerp(deadPos, respownPos, respawnTimer);
        this.transform.eulerAngles = Vector3.Slerp(deadRotation, new Vector3(0.0f, 0.0f, respownRotate), respawnTimer);

    }

    void Wait()
    {

    }

    void Normal()
    {
        if (gamepadState.A && !gamepadStateOld.A)
        {
            stateUpdater = Move;
            return;
        }
        else if (gamepadState.Y && !gamepadStateOld.Y)
        {
            stateUpdater = Attack;
            return;
        }
        else if (gamepadState.B && !gamepadStateOld.B)
        {
            stateUpdater = Brake;
            return;
        }
        RotatePlayer();
    }

    void RotatePlayer()
    {
        //mode1
        if (gamepadState.LeftShoulder)
        {
            //rigid.AddTorque(torquePower * Time.deltaTime, ForceMode2D.Impulse);
            this.transform.Rotate(Vector3.forward, torquePower * Time.deltaTime);

        }
        if (gamepadState.RightShoulder)
        {
            //rigid.AddTorque(-torquePower * Time.deltaTime, ForceMode2D.Impulse);
            this.transform.Rotate(Vector3.forward, -torquePower * Time.deltaTime);
        }
        //mode2
        //{
        //    Vector2 rotateVector = gamepadState.LeftStickAxis;
        //    if (!rotateVector.SqrMagnitude().Equals(0.0f))
        //    {
        //        float angle = Mathf.Rad2Deg * Mathf.Atan2(rotateVector.y, rotateVector.x) - 90.0f;
        //        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        //    }
        //}

    }
    
    private IEnumerator Invincible(float duration = 0.0f)
    {

        var elapsed = 0.0f;

        while (elapsed < invincibleTime)
        {
            elapsed += Time.deltaTime;
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInvincibleTime)
            {
                blinkTimer = 0.0f;
                bool flg = spriteRenderer.enabled;
                spriteRenderer.enabled = !flg;
            }
            yield return null;
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
        blinkTimer = 0.0f;
    }

    void GamePadStateUpdate()
    {
        gamepadStateOld = gamepadState;

        gamepadState = GamepadInput.GamePad.GetState(playerNo, true);

    }
}
