using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;

namespace Mastermind {
    class DatabaseStuff {
        public string GetTop10FromDB() {
            try {
                // Creates the ScriptEngine variable, and gets the python file.
                ScriptEngine engine = Python.CreateEngine();
                dynamic getFile = engine.ExecuteFile(@"DBMedPython.py");

                // Gets the class DBManager in the file, and returns the result from the SelectTop10FromDB method.
                dynamic getClass = getFile.DBManager();
                return getClass.SelectTop10FromDB();
            }
            // Throws an exception if the code in the try fails.
            catch (Exception e) {

            }
            // Gives a default return value, it shouldn't get to.
            return "Failed!";
        }

        public string GetNameFromDB(string name) {
            try {
                ScriptEngine engine = Python.CreateEngine();
                dynamic getFile = engine.ExecuteFile(@"DBMedPython.py");

                dynamic getClass = getFile.DBManager();
                return getClass.SelectNameFromDB(name);
            }
            catch (Exception e) {

            }

            return "Failed!";
        }

        public string GetColorFromDB(string name) {
            try {
                ScriptEngine engine = Python.CreateEngine();
                dynamic getFile = engine.ExecuteFile(@"DBMedPython.py");

                dynamic getClass = getFile.DBManager();
                return getClass.SelectColorFromDB(name);
            }
            catch (Exception e) {

            }

            return "Failed!";
        }

        public void InsertUserInDB(string name, string color) {
            try {
                ScriptEngine engine = Python.CreateEngine();
                dynamic getFile = engine.ExecuteFile(@"DBMedPython.py");

                dynamic getClass = getFile.DBManager();
                getClass.InsertIntoDB(name, color);
            }
            catch (Exception e) {

            }
        }

        public void EditScoreInDB(string name, int score) {
            try {
                ScriptEngine engine = Python.CreateEngine();
                dynamic getFile = engine.ExecuteFile(@"DBMedPython.py");

                dynamic getClass = getFile.DBManager();
                getClass.UpdateScoreInDB(name, score.ToString());
            }
            catch (Exception e) {

            }
        }
    }
}