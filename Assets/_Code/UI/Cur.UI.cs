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

		public static Color Lighten(this Color color, float intesity = 0.2f) {
			return new Color(
				Mathf.Lerp(color.r, 1, intesity),
				Mathf.Lerp(color.g, 1, intesity),
				Mathf.Lerp(color.b, 1, intesity)
			);
		}

		public static Color Lerp(Color color, Color targetColor, float intesity) {
			return new Color(
				Mathf.Lerp(color.r, targetColor.r, intesity),
				Mathf.Lerp(color.g, targetColor.g, intesity),
				Mathf.Lerp(color.b, targetColor.b, intesity)
			);
		}

	}
}