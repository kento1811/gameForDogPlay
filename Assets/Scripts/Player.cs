using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private InputAction moveAction;
    private InputAction attackAction;
    private Animator animator;

    private float speedMov = 5f;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
    }

    private void handleMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
         if (moveInput != Vector2.zero)
        {
            animator.SetBool("isWalking",true);
            Move(moveInput);
        }else
        {
            animator.SetBool("isWalking",false);
        }
    }

    private void Move(Vector2 direction)
    {
        switch (direction)
        {
            case Vector2 left when left == Vector2.left:
                transform.Translate(Vector2.left * Time.deltaTime * speedMov);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                animator.SetFloat("Direction",0);
                break;
            case Vector2 right when right == Vector2.right:
                transform.Translate(Vector2.right * Time.deltaTime * speedMov);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                animator.SetFloat("Direction",0);
                break;
            case Vector2 up when up == Vector2.up:
                transform.Translate(Vector2.up * Time.deltaTime * speedMov);
                animator.SetFloat("Direction",1);
                break;
            case Vector2 down when down == Vector2.down:
                transform.Translate(Vector2.down * Time.deltaTime * speedMov);
                animator.SetFloat("Direction",-1);
                break;
            case Vector2 up_side when up_side.y > 0:
                transform.Translate(up_side * Time.deltaTime * speedMov);
                if(up_side.x > 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                } else
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                animator.SetFloat("Direction",0.5f);
                break;

            case Vector2 down_side when down_side.y < 0:
                transform.Translate( down_side * Time.deltaTime * speedMov);
                if(down_side.x > 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                } else
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                animator.SetFloat("Direction",-0.5f);
                break;
            default : 
            animator.SetBool("isWalking",false);
            break;
        }
    }
}
