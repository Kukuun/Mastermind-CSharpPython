using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using IrrKlang;


namespace Mastermind {
    public enum colors : int { Red = 0, Blue = 1, Green = 2, Yellow = 3, Error = 10000 }

    class Program {
        static GameManager gm = new GameManager();
        static ScriptEngine se = Python.CreateEngine();

        static void Main(string[] args) {
            ISoundEngine engine = new ISoundEngine();


            // To play a sound, we only to call play2D(). The second parameter
            // tells the engine to play it looped.
            engine.SoundVolume = 0.02f;
            engine.Play2D("snds/FcKahuna - Hayling.mp3", true);
            gm.Game();
        }

        static void DatabaseStuff() {

        }
    }
}