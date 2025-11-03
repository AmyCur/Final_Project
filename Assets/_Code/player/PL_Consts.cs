namespace Player {
    public static class Consts {
        public static class Movement {
            const float MID_AIR_SPEED_REDUCTION = 0.714f;
            
            public static float AirSpeedChange (bool grounded) => !grounded ? MID_AIR_SPEED_REDUCTION : 1f;  
        }
    }
}