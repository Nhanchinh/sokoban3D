using UnityEngine;

public class MobileDpad : MonoBehaviour
{
	public PlayerController player;
	public HoldButton upBtn;
	public HoldButton downBtn;
	public HoldButton leftBtn;
	public HoldButton rightBtn;

	      void Update()
	{
		if (!player) return;
		bool up =  (upBtn    != null) && upBtn.IsHeld;
		bool dn =  (downBtn  != null) && downBtn.IsHeld;
		bool lf =  (leftBtn  != null) && leftBtn.IsHeld;
		bool rt =  (rightBtn != null) && rightBtn.IsHeld;

		player.upHeld = up;
		player.SetDownHeld(dn);
		player.SetLeftHeld(lf);
		player.SetRightHeld(rt);

		//Debug.Log($"Held U/D/L/R = {up}/{dn}/{lf}/{rt}");
	}

	// Gọi để nhả tất cả input ngay lập tức
	public void ReleaseAll()
	{
		if (upBtn) upBtn.ForceRelease();
		if (downBtn) downBtn.ForceRelease();
		if (leftBtn) leftBtn.ForceRelease();
		if (rightBtn) rightBtn.ForceRelease();

		if (player)
		{
			player.SetUpHeld(false);
			player.SetDownHeld(false);
			player.SetLeftHeld(false);
			player.SetRightHeld(false);
			player.ResetInput();
		}
	}
}