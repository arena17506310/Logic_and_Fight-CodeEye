using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 3f;

    private bool canDash = true;
    private bool isDashing = false;

    void Update()
    {
        Vector2 move = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) move.y += 1;
        if (Keyboard.current.sKey.isPressed) move.y -= 1;
        if (Keyboard.current.aKey.isPressed) move.x -= 1;
        if (Keyboard.current.dKey.isPressed) move.x += 1;

        Vector3 dir = new Vector3(move.x, 0, move.y);

        if (dir.magnitude > 0)
        {
            dir.Normalize();

            if (!isDashing)
            {
                transform.position += dir * moveSpeed * Time.deltaTime;
            }

            transform.forward = dir;
        }

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float timer = 0f;

        while (timer < dashDuration)
        {
            transform.position += transform.forward * dashSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
}