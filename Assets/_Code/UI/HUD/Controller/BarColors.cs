namespace UI.HUD.Controller;

public static class BarColors {
	public static Color fireColor = new Color(0.780f, 0.19f, 0.27f, 1);
	public static Color waterColor = new Color(0, 0, 1, 1);
	public static Color electricColor = new Color(1, 1, 0, 1);
	public static Color windColor = new Color(0, 1, 0, 1);
	public static Color noneColor = new Color(1, 1, 1, 1);

	public static Dictionary<ElementType, Color> ElementColor = new(){
		{ElementType.electric, electricColor},
		{ElementType.fire, fireColor},
		{ElementType.water, waterColor},
		{ElementType.wind, windColor},
		{ElementType.None, noneColor}
	};
}
