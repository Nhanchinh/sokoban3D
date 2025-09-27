// using System.Collections;
// using UnityEngine;

// public class PlayerController : MonoBehaviour
// {
//     public LayerMask blockingLayer;   // Layer cản (tường, hộp)
//     public float moveSpeed = 5f;
//     private bool isMoving = false;    // Trạng thái đang di chuyển
//     private Animator animator;
//     void Awake()
//     {
//         animator = GetComponent<Animator>();
//     }
//     void Update()
//     {
//         if (isMoving) return;
// animator.SetBool("Walk",false);
//         Vector3 direction = Vector3.zero;

//         if (Input.GetKey(KeyCode.W)) direction = Vector3.forward;
//         if (Input.GetKey(KeyCode.S)) direction = Vector3.back;
//         if (Input.GetKey(KeyCode.A)) direction = Vector3.left;
//         if (Input.GetKey(KeyCode.D)) direction = Vector3.right;

//         if (direction != Vector3.zero)
//         {
//             transform.LookAt(transform.position + direction);
//             animator.SetBool("Walk",true);
//             TryToMove(direction);
//         }
//     }

//     private void TryToMove(Vector3 direction)
//     {
//         Vector3 targetPosition = transform.position + direction;

//         // Kiểm tra phía trước
//         if (!Physics.Raycast(transform.position, direction, out RaycastHit hit, 1f, blockingLayer))
//         {
//             // Không có gì chặn -> đi tới
//             StartCoroutine(MoveToPosition(targetPosition));
//         }
//         else if (hit.collider.CompareTag("Box"))
//         {
//             // Nếu gặp hộp -> thử đẩy hộp
//             Vector3 boxTargetPos = hit.collider.transform.position + direction;

//             if (!Physics.Raycast(hit.collider.transform.position, direction, 1f, blockingLayer))
//             {
//                 // Đẩy hộp
//                 StartCoroutine(MoveBox(hit.collider.gameObject, boxTargetPos));
//                 StartCoroutine(MoveToPosition(targetPosition));
//             }
//         }
//     }

//     private IEnumerator MoveToPosition(Vector3 target)
//     {
//         isMoving = true;
//         while (Vector3.Distance(transform.position, target) > 0.01f)
//         {
//             transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
//             yield return null;
//         }
//         transform.position = target;
//         isMoving = false;
//     }

//     private IEnumerator MoveBox(GameObject box, Vector3 target)
//     {
//         while (Vector3.Distance(box.transform.position, target) > 0.01f)
//         {
//             box.transform.position = Vector3.MoveTowards(box.transform.position, target, moveSpeed * Time.deltaTime);
//             yield return null;
//         }
//         box.transform.position = target;
//     }
// }

using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask blockingLayer;   // Layer cản (tường, hộp)
    public float moveSpeed = 5f;
    private bool isMoving = false;    // Trạng thái đang di chuyển
    private Animator animator;

    // cờ giữ nút từ mobile UI
    public bool upHeld   = false;
    private bool downHeld = false;
    private bool leftHeld = false;
    private bool rightHeld = false;

    // setter để MobileDpad gọi
    public void SetUpHeld(bool v)    { upHeld = v; }
    public void SetDownHeld(bool v)  { downHeld = v; }
    public void SetLeftHeld(bool v)  { leftHeld = v; }
    public void SetRightHeld(bool v) { rightHeld = v; }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (isMoving) return;
        animator.SetBool("Walk",false);
        Vector3 direction = Vector3.zero;

        // Hợp nhất: Bàn phím OR Button UI
        if (Input.GetKey(KeyCode.W) || upHeld)    direction = Vector3.forward;
        if (Input.GetKey(KeyCode.S) || downHeld)  direction = Vector3.back;
        if (Input.GetKey(KeyCode.A) || leftHeld)  direction = Vector3.left;
        if (Input.GetKey(KeyCode.D) || rightHeld) direction = Vector3.right;
        if (direction != Vector3.zero)
        {
            transform.LookAt(transform.position + direction);
            animator.SetBool("Walk",true);
            TryToMove(direction);
        }
    }

    public void ResetInput()
    {
        upHeld = false;
        downHeld = false;
        leftHeld = false;
        rightHeld = false;
    }
    private void TryToMove(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + direction;

        // Kiểm tra phía trước
        if (!Physics.Raycast(transform.position, direction, out RaycastHit hit, 1f, blockingLayer))
        {
            // Không có gì chặn -> đi tới
            StartCoroutine(MoveToPosition(targetPosition));
        }
        else if (hit.collider.CompareTag("Box"))
        {
            // Nếu gặp hộp -> thử đẩy hộp
            Vector3 boxTargetPos = hit.collider.transform.position + direction;

            if (!Physics.Raycast(hit.collider.transform.position, direction, 1f, blockingLayer))
            {
                // Đẩy hộp
                StartCoroutine(MoveBox(hit.collider.gameObject, boxTargetPos));
                StartCoroutine(MoveToPosition(targetPosition));
                // SFX đẩy hộp (gọi 1 lần khi bắt đầu đẩy)
                if (AudioManager.Instance != null) 
               {
                AudioManager.Instance.PlayPush();
                Debug.Log("Push SFX");
               }
            }
        }
        
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        isMoving = false;
    }

    private IEnumerator MoveBox(GameObject box, Vector3 target)
    {
        while (Vector3.Distance(box.transform.position, target) > 0.01f)
        {
            box.transform.position = Vector3.MoveTowards(box.transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        box.transform.position = target;
    }
}