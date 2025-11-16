using UnityEngine;

namespace Cur.UI {
	public static class ColorUtil {
		// intesity should be between 0 and 1
		public static Color Darken(this Color color, float intesity = 0.2f) {
			return new Color(
				Mathf.Lerp(color.r, 0, intesity),
				Mathf.Lerp(color.g, 0, intesity),
				Mathf.Lerp(color.b, 0, intesity)
			);
		}

	}
}