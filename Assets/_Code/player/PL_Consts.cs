using Player.Movement;

namespace Player {
	public static class Consts {
		public static class Movement {
			const float MID_AIR_SPEED_REDUCTION = 0.714f;
			public static float AirSpeedChange(bool grounded) => !grounded ? MID_AIR_SPEED_REDUCTION : 1f;
		}

		public static class Multipliers {
			public const float JUMP_MULTIPLIER = 100f;
			public const float SLAM_MULTIPLIER = 10_000f;
			public const float SLIDE_MULTIPLIER = 10_000f;
			public const float DASH_MULTIPLIER = 2_000f;
		}

		public static class Player{
			public static PL_Controller pc;
		}
	}
}