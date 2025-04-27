using UnityEngine;
using System.Collections;
using PolyNav;


/*
* This is an example script of how you could make stuff happen when the agent is changing direction.
* In this example 4 directions are supported + idle.
* Furthermore, if doFlip is checked, the gameobject will flip on it's x axis, usefull for 2D sprites moving left/right like for example
* in an adventure game.
* Once again, this is an example to see how it can be done, for you to take and customize to your needs :)
*/
public class DirectionChecker : MonoBehaviour
{
    Animator animator;
    public bool doFlip = true;

    private Vector2 lastDir;
    private float originalScaleX;
    

    private PolyNavAgent _agent;
    private PolyNavAgent agent {
        get { return _agent != null ? _agent : _agent = GetComponent<PolyNavAgent>(); }
    }

    void Awake() {
        originalScaleX = transform.localScale.x;
        animator = GetComponent<Animator>();
    }
    public Vector2 UP= new Vector2(0,1);
    public Vector2 DOWN = new Vector2(0, -1);
    public Vector2 LEFT = new Vector2(1, 0);
    public Vector2 RIGHT = new Vector2(-1, 0);
    void Update() {

        var dir = agent.movingDirection;
        var x = Mathf.Round(dir.x);
        var y = Mathf.Round(dir.y);

        //eliminate diagonals favoring x over y
        y = Mathf.Abs(y) == Mathf.Abs(x) ? 0 : y;

        dir = new Vector2(x, y);

        if ( dir != lastDir ) {

            if ( dir == Vector2.zero ) {
                Debug.Log("IDLE");
                animator.SetBool("Walking", false);

            }

            if ( dir.x == -1 ) {
                Debug.Log("RIGHT");
                    SetAnimator(RIGHT);
                
            }

            if ( dir.x == 1 ) {
                Debug.Log("LEFT");
                if ( doFlip ) {
                    SetAnimator(LEFT);
                }
            }

            if ( dir.y == -1 ) {
                Debug.Log("UP");
                SetAnimator(DOWN);
            }

            if ( dir.y == 1 ) {
                Debug.Log("DOWN");
                SetAnimator(UP);
            }

            lastDir = dir;
        }
    }
    void SetAnimator( Vector2 vector)
    {
        animator.SetFloat("DirX", vector.x);
        animator.SetFloat("DirY", vector.y);
        animator.SetBool("Walking", true);
    }
}
