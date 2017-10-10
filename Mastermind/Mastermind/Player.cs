using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;

namespace Mastermind {
    class Player {
        private string name;
        private string playerId;
        private string colorValue;

        public string Name {
            get { return name; }
            set { name = value; }
        }
        public string PlayerId {
            get { return playerId; }
            set { playerId = value; }
        }
        public string ColorValue {
            get {
                //To use the values as a string simply
                //string test = thePlayer.ColorValue.ToString().ToLower();
                return colorValue;
            }

            set {
                //Setting the new chosen color, using a PythonScript
                try {
                    ScriptEngine engine = Python.CreateEngine();
                    dynamic getFile = engine.ExecuteFile(@"PlayerHandler.py");

                    dynamic getClass = getFile.PlayerHandler();
                    ConsoleColor tmpColor = getClass.ChangeColor(value);
                    colorValue = tmpColor.ToString().ToLower();
                }
                catch (Exception e) //Need to have this or fatal error in python will make C# Fatal Crash
                {

                }
            }
        }
        //Getting the color as a ConsoleColor using a PythonScript
        public ConsoleColor GetConsoleColor(string color) {
            try {
                ScriptEngine engine = Python.CreateEngine();
                dynamic getFile = engine.ExecuteFile(@"PlayerHandler.py");

                dynamic getClass = getFile.PlayerHandler();
                return getClass.ChangeColor(color);
            }
            catch (Exception e) //Need to have this or fatal error in python will make C# Fatal Crash
            {

            }
            return ConsoleColor.White; //If Failed!
        }

        public Player() {
        }
    }
}