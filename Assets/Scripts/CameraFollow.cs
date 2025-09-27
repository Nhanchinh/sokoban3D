using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target; // Player transform (có thể để trống để tự động tìm)
    public Vector3 offset = new Vector3(0, 8, -10); // Điều chỉnh để player ở 3/5 từ dưới lên
    public float followSpeed = 5f; // Tốc độ theo dõi
    
    [Header("Look Settings")]
    public bool lookAtTarget = false; // TẮT việc nhìn theo player để tránh chóng mặt
    public Vector3 lookOffset = new Vector3(0, -3, 0); // Offset khi nhìn (nhìn xuống dưới player nhiều hơn)
    
    [Header("Smooth Settings")]
    public bool smoothFollow = true; // Theo dõi mượt
    public float smoothTime = 0.2f; // Thời gian mượt
    
    [Header("Auto Find Settings")]
    public bool autoFindPlayer = true; // Tự động tìm player
    public float searchInterval = 0.5f; // Khoảng thời gian tìm lại player (giây)
    
    [Header("Fixed Camera Settings")]
    public bool useFixedRotation = true; // Sử dụng góc nhìn cố định
    public Vector3 fixedRotation = new Vector3(60, 0, 0); // Góc nhìn 60 độ từ trên xuống
    
    private Vector3 velocity = Vector3.zero;
    private MobileDpad mobileDpad;
    private float lastSearchTime;
    
    void Start()
    {
        // Tìm MobileDpad để lấy reference đến player
        mobileDpad = FindObjectOfType<MobileDpad>();
        
        if (mobileDpad != null && mobileDpad.player != null)
        {
            target = mobileDpad.player.transform;
            Debug.Log("CameraFollow: Found player through MobileDpad");
        }
        else if (autoFindPlayer)
        {
            // Fallback: tìm player bằng tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                Debug.Log("CameraFollow: Found player by tag");
            }
        }
        
        // Set góc nhìn cố định khi start
        if (useFixedRotation)
        {
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
        
        lastSearchTime = Time.time;
    }
    
    void LateUpdate()
    {
        // Tự động tìm lại player nếu cần
        if (autoFindPlayer && Time.time - lastSearchTime > searchInterval)
        {
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                FindPlayer();
            }
            lastSearchTime = Time.time;
        }
        
        if (target == null) return;
        
        // Vị trí đích của camera
        Vector3 targetPosition = target.position + offset;
        
        if (smoothFollow)
        {
            // Di chuyển mượt đến vị trí đích
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            // Di chuyển trực tiếp
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        
        // Chỉ thay đổi góc nhìn nếu lookAtTarget = true
        if (lookAtTarget)
        {
            Vector3 lookTarget = target.position + lookOffset;
            transform.LookAt(lookTarget);
        }
        // Nếu không, giữ nguyên góc nhìn cố định
        else if (useFixedRotation)
        {
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
    }
    
    private void FindPlayer()
    {
        // Ưu tiên tìm qua MobileDpad
        if (mobileDpad != null && mobileDpad.player != null)
        {
            target = mobileDpad.player.transform;
            Debug.Log("CameraFollow: Reconnected to player through MobileDpad");
            return;
        }
        
        // Fallback: tìm bằng tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            Debug.Log("CameraFollow: Reconnected to player by tag");
        }
    }
    
    // Hàm để set target mới (dùng khi player spawn lại)
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("CameraFollow: Target manually set");
    }
    
    // Hàm để force tìm lại player
    public void ForceFindPlayer()
    {
        FindPlayer();
        lastSearchTime = Time.time;
    }
}
