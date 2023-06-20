namespace HendricksAustin.Lab6
{
    // The Game Events used across the Game.
    // Anytime there is a need for a new event, it should be added here.

    public static class Events
    {
        public static IncrementScoreEvent IncrementScoreEvent = new();
        public static AddLifeEvent AddLifeEvent = new();
        public static AddAmmoEvent AddAmmoEvent = new();
        public static GameOverEvent GameOverEvent = new();

        public static OptionsMenuEvent OptionsMenuEvent = new();
        public static MovementSpeedUpdateEvent MovementSpeedUpdateEvent = new();
    }

    // Game Events.
    public class IncrementScoreEvent : GameEvent
    {
        public int Score;
    }

    public class GameOverEvent : GameEvent
    {
        public bool Win;
    }

    // UI Events.
    public class OptionsMenuEvent : GameEvent
    {
        public bool Active;
    }

    public class MovementSpeedUpdateEvent : GameEvent
    {
        public float Value;
    }

    public class AddLifeEvent : GameEvent { }

    public class AddAmmoEvent : GameEvent { }

}
