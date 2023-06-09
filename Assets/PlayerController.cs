using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    Vector2 movementInput;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;



    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate() {
        if(canMove)
        {
            // If movement input is not 0, try to move
            if(movementInput !=  Vector2.zero) {
                bool res = TryMove(movementInput);

                if(!res) {
                    res = TryMove(new Vector2(movementInput.x, 0));
                }

                if(!res) {
                    res = TryMove(new Vector2(0, movementInput.y));
                }

                animator.SetBool("isMoving", res);
            
            } else {
                animator.SetBool("isMoving", false);
            }

            // Set direction of sprite to movement direction
            if(movementInput.x < 0) {
                spriteRenderer.flipX = true;
            } else if (movementInput.x > 0) {
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if(direction != Vector2.zero) {
            // check for collisions
            int count = rb.Cast(
                direction,                                        // X and Y values betweeen -1 and 1 that represent the direction from the body to look for collisions
                movementFilter,                                       // The settings that determine where a collision can occur on
                castCollisions,                                       // List of collisions to store into after cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset     //  amount to cast equal to the movement plus offset
            );

            if(count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack() {
        LockMovement();

        if(spriteRenderer.flipX == true) {
            swordAttack.AttackLeft();
        } else {
            swordAttack.AttackRight();
        }
    }

    public void EndSwordAttack() {
        UnlockMovement();
        swordAttack.StopAttack();
    }

    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }
}
