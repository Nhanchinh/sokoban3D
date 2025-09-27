// using UnityEngine;

// [RequireComponent(typeof(Collider), typeof(Renderer))]
// public class BoxController : MonoBehaviour
// {
//     private Renderer rend;
//     [Tooltip("Màu ban đầu sẽ lấy từ material nếu bạn không set trong Inspector")]
//     public Color normalColor = Color.gray;
//     public Color onGoalColor = Color.green;

//     // trạng thái public để inspector/manager có thể đọc
//     public bool onGoal = false;

//     void Awake()
//     {
//         rend = GetComponent<Renderer>();
//         if (rend != null)
//         {
//             // lưu màu từ material nếu user không set normalColor trong inspector
//             normalColor = rend.material.color;
//             rend.material.color = onGoal ? onGoalColor : normalColor;
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Goal"))
//         {
//             SetOnGoal(true);
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Goal"))
//         {
//             SetOnGoal(false);
//         }
//     }

//     private void SetOnGoal(bool value)
//     {
//         onGoal = value;
//         if (rend != null)
//             rend.material.color = onGoal ? onGoalColor : normalColor;

//         Debug.Log($"{gameObject.name} OnGoal = {onGoal}");
//         // Nếu cần, có thể gọi GameManager để cập nhật tiến trình level ở đây
//         // GameManager.Instance?.BoxStateChanged(this, onGoal);
//     }

//     public bool IsOnGoal()
//     {
//         return onGoal;
//     }
// }


// using UnityEngine;

// [RequireComponent(typeof(Collider), typeof(Renderer))]
// public class BoxController : MonoBehaviour
// {
// 	private Renderer rend;
// 	private Collider col;

// 	[Tooltip("Màu ban đầu sẽ lấy từ material nếu bạn không set trong Inspector")]
// 	public Color normalColor = Color.gray;
// 	public Color onGoalColor = Color.green;

// 	// Trạng thái public để inspector/manager có thể đọc
// 	public bool onGoal = false;

// 	[Header("Goal Alignment Settings")]
// 	[Tooltip("Ngưỡng tương đối theo kích thước hộp (XZ). Ví dụ 0.2 = 20% cạnh nhỏ hơn của hộp.")]
// 	public float centerToleranceFactor = 0.5f;
// 	[Tooltip("Ngưỡng tuyệt đối tối thiểu (m). Dùng nếu hộp quá nhỏ hoặc muốn cố định.")]
// 	public float minAbsoluteTolerance = 0.02f;

// 	void Awake()
// 	{
//         rend = GetComponent<Renderer>();
// 		col = GetComponent<Collider>();
// 		if (rend != null)
// 		{
// 			// lưu màu từ material nếu user không set normalColor trong inspector
// 			normalColor = rend.material.color;
// 			rend.material.color = onGoal ? onGoalColor : normalColor;
// 		}
// 	}

// 	// Không set true ở Enter để tránh đếm sớm khi mới chạm mép
// 	private void OnTriggerEnter(Collider other)
// 	{
// 		if (other.CompareTag("Goal"))
// 		{
// 			// do nothing, chờ OnTriggerStay để xác nhận căn giữa
// 		}
// 	}

// 	private void OnTriggerStay(Collider other)
// 	{
// 		if (!other.CompareTag("Goal"))
// 			return;

// 		// Căn giữa theo mặt phẳng XZ: khoảng cách giữa tâm hộp và tâm goal
// 		Vector3 boxCenter = col != null ? col.bounds.center : transform.position;
// 		Vector3 goalCenter = other.bounds.center;

// 		Vector2 boxXZ = new Vector2(boxCenter.x, boxCenter.z);
// 		Vector2 goalXZ = new Vector2(goalCenter.x, goalCenter.z);
// 		float dxz = Vector2.Distance(boxXZ, goalXZ);

// 		// Tolerance dựa trên kích thước hộp (cạnh nhỏ hơn trên XZ) + ngưỡng tối thiểu
// 		float minExtentXZ = 0.0f;
// 		if (col != null)
// 			minExtentXZ = Mathf.Min(col.bounds.extents.x, col.bounds.extents.z);

// 		float tol = Mathf.Max(minAbsoluteTolerance, centerToleranceFactor * Mathf.Max(0.0001f, minExtentXZ));

// 		// Optional: đảm bảo có chồng lấp theo phương dọc (nếu cần)
// 		bool verticalOverlap = true; // thường 2 cái nằm trên mặt đất, có thể bỏ qua
// 		// Nếu muốn check kỹ:
// 		// Bounds b = col.bounds;
// 		// Bounds g = other.bounds;
// 		// verticalOverlap = (b.min.y <= g.max.y) && (g.min.y <= b.max.y);

// 		SetOnGoal(dxz <= tol && verticalOverlap);
// 	}

// 	private void OnTriggerExit(Collider other)
// 	{
// 		if (other.CompareTag("Goal"))
// 		{
// 			SetOnGoal(false);
// 		}
// 	}

// 	private void SetOnGoal(bool value)
// 	{
// 		onGoal = value;
// 		if (rend != null)
// 			rend.material.color = onGoal ? onGoalColor : normalColor;

// 		Debug.Log($"{gameObject.name} OnGoal = {onGoal}");
// 		// Nếu cần, có thể gọi GameManager để cập nhật tiến trình level ở đây
// 		// GameManager.Instance?.BoxStateChanged(this, onGoal);
// 	}

// 	public bool IsOnGoal()
// 	{
// 		return onGoal;
// 	}
// }

using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class BoxController : MonoBehaviour
{
	private Renderer rend;
	private Collider col;

	[Tooltip("Màu ban đầu sẽ lấy từ material nếu bạn không set trong Inspector")]
	public Color normalColor = Color.gray;
	public Color onGoalColor = Color.green;

	// Trạng thái để GameManager đọc
	public bool onGoal = false;

	// Sự kiện bắn mỗi khi trạng thái onGoal của bất kỳ Box nào thay đổi
	public static System.Action OnAnyBoxGoalStateChanged;

	[Header("Goal Alignment Settings")]
	[Tooltip("Tỉ lệ dung sai theo kích thước hộp (XZ). 0.5 = 50% cạnh nhỏ hơn của hộp.")]
	public float centerToleranceFactor = 0.5f;
	[Tooltip("Dung sai tuyệt đối tối thiểu (m).")]
	public float minAbsoluteTolerance = 0.2f;

	void Awake()
	{
		rend = GetComponent<Renderer>();
		col = GetComponent<Collider>();
		if (rend != null)
		{
			normalColor = rend.material.color;
			rend.material.color = onGoal ? onGoalColor : normalColor;
		}
	}

	private bool IsGoalCollider(Collider other)
	{
		return other != null && (other.CompareTag("Goal") || other.GetComponent<GoalController>() != null);
	}

	private void OnTriggerEnter(Collider other)
	{
		// Chờ OnTriggerStay để xác nhận căn giữa
		if (IsGoalCollider(other)) { }
	}

	private void OnTriggerStay(Collider other)
	{
		if (!IsGoalCollider(other)) return;

		// Tính khoảng cách tâm trên mặt phẳng XZ
		Vector3 boxCenter = col != null ? col.bounds.center : transform.position;
		Vector3 goalCenter = other.bounds.center;

		Vector2 boxXZ = new Vector2(boxCenter.x, boxCenter.z);
		Vector2 goalXZ = new Vector2(goalCenter.x, goalCenter.z);
		float dxz = Vector2.Distance(boxXZ, goalXZ);

		// Tolerance dựa trên kích thước hộp (cạnh nhỏ hơn trên XZ) + ngưỡng tối thiểu
		float minExtentXZ = 0.0f;
		if (col != null)
			minExtentXZ = Mathf.Min(col.bounds.extents.x, col.bounds.extents.z);

		float tol = Mathf.Max(minAbsoluteTolerance, centerToleranceFactor * Mathf.Max(0.0001f, minExtentXZ));

		// Có thể thêm kiểm tra chồng lấp theo Y nếu cần
		bool verticalOverlap = true;

		SetOnGoal(dxz <= tol && verticalOverlap);
	}

	private void OnTriggerExit(Collider other)
	{
		if (IsGoalCollider(other))
		{
			SetOnGoal(false);
		}
	}

	private void SetOnGoal(bool value)
	{
		if (onGoal == value) return;

		onGoal = value;
		if (rend != null)
			rend.material.color = onGoal ? onGoalColor : normalColor;

		// Thông báo cho GameManager
		OnAnyBoxGoalStateChanged?.Invoke();
	}

	public bool IsOnGoal()
	{
		return onGoal;
	}
}