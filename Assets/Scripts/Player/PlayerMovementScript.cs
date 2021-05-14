using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public SurfaceSensorScript groundSensor;
    public SurfaceSensorScript wallSensor;
    public float jumpForce = 5f;
    public float speed = 5f;

    private Rigidbody2D m_body;
    private Animator m_animator;
    private float m_direction = 1;
    private State m_state = State.DEFAULT;
    // Start is called before the first frame update
    void Start()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        groundSensor?.AttachListener(OnGroundEnter, true);
        groundSensor?.AttachListener(OnGroundExit, false);
        wallSensor?.AttachListener(OnWallEnter, true);
        wallSensor?.AttachListener(OnWallExit, false);
    }

    // Update is called once per frame
    void Update()
    {
        var input = Input.GetAxis("Horizontal");

        if((input > 0 && m_direction < 0) || (input < 0 && m_direction > 0))
            Flip();

        if(Input.GetButtonDown("Jump"))
            Jump();

        if(Input.GetButtonDown("Dash") && m_state != State.DASH)
            Dash();

        switch(m_state){
            case State.DEFAULT:
                Move(input);
                break;
            case State.MOVE_ON_WALL:
                MoveOnWall(input);
                break;
            case State.CLIMB_TO_WALL:
                ClimbToWall(input);
                break;
        }

        UpdateAnimator();
    }

    private void Move(float input){
        if(wallSensor.State() && input != 0)
            StartCoroutine(StartWallClimbing());
        else
            m_body.velocity = new Vector2(input * speed, m_body.velocity.y);
    }


    private bool m_isAbleToWallWalk = false;
    private float m_moveOnWallMultiplier = 0f;
    private float m_wallMoveDuration = 2f;
    private IEnumerator StartWallWalking(){
        m_moveOnWallMultiplier = 0f;
        m_state = State.MOVE_ON_WALL;
        for(float t = 0f; t < m_wallMoveDuration; t += Time.deltaTime){
            m_moveOnWallMultiplier = Mathf.Lerp(0, 360, t / m_wallMoveDuration);
            yield return null;
        }
        m_moveOnWallMultiplier = 360;
        StopWallMoving();
    }
    private void MoveOnWall(float input){
        m_body.velocity = new Vector2(input * speed, speed * Mathf.Sin(m_moveOnWallMultiplier * Mathf.Deg2Rad) / 1.5f);
    }
    private void StopWallMoving(){
        StopCoroutine(StartWallWalking());
        m_state = State.DEFAULT;
    }

    private float m_climbForceMultiplier = 1f;
    private float m_climbDirection = -2f;
    private IEnumerator StartWallClimbing(){
        if(m_climbDirection == m_direction)
            yield break;
        m_climbDirection = m_direction;
        m_climbForceMultiplier = 1f;
        m_state = State.CLIMB_TO_WALL;
        while(m_climbForceMultiplier > 0){
            m_climbForceMultiplier -= Time.deltaTime / 2;
            yield return null;
        }
        StopWallClimbing();
    }
    private void ClimbToWall(float input){
        m_body.velocity = new Vector2(input * speed, speed * m_climbForceMultiplier * Mathf.Abs(input));
    }
    private void StopWallClimbing(){
        Debug.Log("Stop wall climbing");
        StopCoroutine(StartWallClimbing());
        m_state = State.DEFAULT;
    }

    private void Jump(){
        if(groundSensor.State()){
            SimpleJump();
        } else if(wallSensor.State()){
            StartCoroutine(WallJump());
        } else if(m_isAbleToWallWalk){
            StartCoroutine(StartWallWalking());
        }
    }

    private void SimpleJump(){
        m_animator.SetTrigger("Jump");
        m_body.velocity = new Vector2(m_body.velocity.x, jumpForce);
        groundSensor.SetDelay(0.2f);
    }

    private IEnumerator WallJump(){
        m_state = State.WALL_JUMP;
        m_body.velocity = new Vector2((Mathf.Abs(m_body.velocity.x) + speed) * m_direction * -1, jumpForce);
        yield return new WaitForSeconds(0.5f);
        m_state = State.DEFAULT;
    }

    private void Dash(){
        m_animator.SetTrigger("Dash");
        m_state = State.DASH;
        m_body.velocity = new Vector2((Mathf.Abs(m_body.velocity.x) + speed) * m_direction, m_body.velocity.y);
        StartCoroutine(EndDash(0.5f));
    }

    private IEnumerator EndDash(float timer){
        yield return new WaitForSeconds(timer);
        m_state = State.DEFAULT;
    }

    private void Flip(){
        m_direction *= -1;
        transform.localScale = new Vector3(m_direction, 1, 1);
    }

    private void UpdateAnimator(){
        m_animator.SetBool("IsGrounded", groundSensor.State());
        if(m_state == State.DEFAULT && m_body.velocity.x != 0)
            m_animator.SetInteger("AnimState", 1);
        else if(m_state == State.DEFAULT)
            m_animator.SetInteger("AnimState", 0);
    }

    private void OnGroundExit(Collider2D col){

    }

    private void OnGroundEnter(Collider2D col){
        m_climbDirection = -2f;
    }

    private void OnWallExit(Collider2D col){
        Debug.Log("Wall exit");
        if(col.gameObject.layer == LayerMask.NameToLayer("Ground")){
            if(m_direction == m_climbDirection && m_body.velocity.y > 0)
                SimpleJump();
            StopWallClimbing();
        }
        else if(col.gameObject.layer == LayerMask.NameToLayer("Background"))
            StopWallMoving();
    }

    private void OnWallEnter(Collider2D col){

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Background"))
            m_isAbleToWallWalk = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Background"))
            m_isAbleToWallWalk = false;
    }

    enum State{
        DEFAULT,
        MOVE_ON_WALL,
        CLIMB_TO_WALL,
        WALL_JUMP,
        DASH
    }
}
