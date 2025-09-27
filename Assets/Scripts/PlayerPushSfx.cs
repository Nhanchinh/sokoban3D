using UnityEngine;

public class PlayerPushSfx : MonoBehaviour
{
	[Header("Filter mục tiêu đẩy")]
	public LayerMask pushableLayers;     // Đặt Layer cho hộp (ví dụ "Box"); để None nếu dùng Tag
	public string pushableTag = "";      // Hoặc điền Tag (ví dụ "Pushable"); để rỗng nếu dùng Layer

	[Header("Ngưỡng phát SFX")]
	public float minMoveSpeed = 0.12f;   // Player phải di chuyển tối thiểu
	public float interval = 0.25f;       // Khoảng thời gian tối thiểu giữa 2 lần phát

	Vector3 _lastPos;
	float _lastPlay;

	void Start()
	{
		_lastPos = transform.position;
	}

	void FixedUpdate()
	{
		_lastPos = transform.position;
	}

	void OnCollisionStay(Collision c)
	{
		if (AudioManager.Instance == null) return;
		if (!IsPushable(c.collider)) return;

		// Ước lượng tốc độ dịch chuyển của player (không cần Rigidbody)
		float speed = (transform.position - _lastPos).magnitude / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
		if (speed < minMoveSpeed) return;

		if (Time.time - _lastPlay >= interval)
		{
			AudioManager.Instance.PlayPush();
			_lastPlay = Time.time;
		}
	}

	bool IsPushable(Collider col)
	{
		// Ưu tiên Layer nếu có set
		if (pushableLayers.value != 0)
		{
			if (((1 << col.gameObject.layer) & pushableLayers) != 0) return true;
		}
		// Hoặc dùng Tag nếu có điền
		if (!string.IsNullOrEmpty(pushableTag))
		{
			if (col.CompareTag(pushableTag)) return true;
		}
		return false;
	}
}