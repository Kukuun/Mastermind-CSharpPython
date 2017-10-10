using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mastermind {
    class GameManager {
        #region Fields

        private int[] slaveMind = new int[4];
        private int[] masterMind = new int[4];
        private int[] lastTry = new int[4] { 10, 10, 10, 10 };
        private List<HistoryItem> previousTries = new List<HistoryItem>();
        private int lives = 10;
        private int selectedColumn = 0;
        private bool win = false;
        private bool lost = false;
        private bool isTrying = false;
        private ScriptEngine se = Python.CreateEngine();

        #endregion
        #region Properties

        public int[] SlaveMind {
            get { return slaveMind; }
            set { slaveMind = value; }
        }
        public int[] MasterMind {
            get { return masterMind; }
            set { masterMind = value; }
        }
        public bool Win {
            get { return win; }
            set { win = value; }
        }
        public int Lives {
            get { return lives; }
            set { lives = value; }
        }
        public bool Lost {
            get { return lost; }
            set { lost = value; }
        }
        public int SelectedColumn {
            get { return selectedColumn; }
            set { selectedColumn = value; }
        }
        public int[] LastTry {
            get { return lastTry; }
            set { lastTry = value; }
        }
        public bool IsTrying {
            get { return isTrying; }
            set { isTrying = value; }
        }
        public bool OnePlayer { get; set; }
        #endregion

        Player player1 = new Player();
        Player player2 = new Player();
        Random rnd = new Random();
        DatabaseStuff dbStuff = new DatabaseStuff();
        //#region Methods

        static ConsoleKey GetKey() {
            return Console.ReadKey().Key;
        }

        /// <summary>
        /// The function that runs the game.
        /// </summary>
        public void Game() {
            Console.ForegroundColor = ConsoleColor.White;
            #region Start

            Console.Clear();
            WriteLine("Welcome to Mastermind");
            bool keepLooping = true; // A bool used to run through the pregame settings.

            // Loops through the pregame settings such as selecting the amount of players, a name and a color.
            while (keepLooping) {
                WriteLine("how many players? (max 2)");

                switch (Console.ReadLine()) {
                    case "0":
                        WriteLine("Zero Players selected, exiting program...");
                        Console.Write("OK");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                    case "1":
                        while (true) {
                            WriteLine("One player selected");
                            OnePlayer = true;
                            keepLooping = false;

                            WriteLine("Enter a username: ");
                            player1.Name = Console.ReadLine(); // Stores the input in the name variable in the Player class.

                            // Checks if the entered name already exists in the database.
                            if (dbStuff.GetNameFromDB(player1.Name) == player1.Name && dbStuff.GetNameFromDB(player1.Name) != "42YouCanHaveThisName42" && player1.Name != "") {
                                WriteLine("Welcome back " + player1.Name + "\\n");
                                break;
                            }
                            else if (player1.Name != "") {
                                // If the entered name doesn't exist in the database.
                                WriteLine("Enter a color (white, red, green, blue, yellow): ");
                                player1.ColorValue = Console.ReadLine().ToLower();
                                dbStuff.InsertUserInDB(player1.Name, player1.ColorValue); // Inserts the name and color stored in their respective variables into the database.
                                WriteLine("Welcome " + player1.Name + "\\n"); // Prints a welcome message for the player.
                                break;
                            }
                        }
                        break;
                    case "2":
                        while (true) {
                            WriteLine("Two player selected");
                            OnePlayer = false;
                            keepLooping = false;

                            WriteLine("Enter a username for player 1: ");
                            player1.Name = Console.ReadLine();

                            if (dbStuff.GetNameFromDB(player1.Name) == player1.Name && dbStuff.GetNameFromDB(player1.Name) != "42YouCanHaveThisName42" && player1.Name != "") {
                                WriteLine("Welcome back " + player1.Name + "\\n");
                                break;
                            }
                            else if (player1.Name != "") {
                                WriteLine("Enter a color (white, red, green, blue, yellow): ");
                                player1.ColorValue = Console.ReadLine().ToLower();
                                dbStuff.InsertUserInDB(player1.Name, player1.ColorValue);
                                WriteLine("Welcome " + player1.Name + "\\n");
                                break;
                            }
                        }

                        WriteLine("Press 'enter' to enter the next players name.");
                        Console.ReadLine();
                        Console.Clear();

                        while (true) {
                            WriteLine("Enter a username for player 2: ");
                            player2.Name = Console.ReadLine();

                            // If statement that runs if player 1 didn't enter the same name as player 2.
                            if (player1.Name != player2.Name) {
                                if (dbStuff.GetNameFromDB(player2.Name) == player2.Name && dbStuff.GetNameFromDB(player2.Name) != "42YouCanHaveThisName42" && player2.Name != "") {
                                    WriteLine("Welcome back " + player2.Name + "\\n");
                                    break;
                                }
                                else if (player2.Name != "") {
                                    WriteLine("Enter a color (white, red, green, blue, yellow): ");
                                    player2.ColorValue = Console.ReadLine().ToLower();
                                    dbStuff.InsertUserInDB(player2.Name, player2.ColorValue);
                                    WriteLine("Welcome " + player2.Name + "\\n");
                                    break;
                                }
                            }
                            else {
                                WriteLine("You can't have the same username as player 1.\\n"); // Prints this message out if the name of player 2 is the same as player 1.
                            }
                        }
                        WriteLine("\\nPress 'enter' to continue.");
                        Console.ReadLine();
                        break;
                    default:
                        Console.Clear();
                        WriteLine("Invalid input. Please choose 1 or 2"); // Prints this message if the entered number of players is not eligible.
                        break;
                }
            }
            #endregion
            #region masterStart
            bool running = true;

            // If statement that's run if number of players selected is 1.
            if (OnePlayer) {
                UpdateMastermind1Player(); // Function used to generate a random color combination to guess.

                if (running) {
                    CheckBordersMastermind(); // Function to make sure we're within the limits of the selectable column in a row.
                }
            }
            // Statement that's run if the number of players selected is 2.
            else {
                while (running) {
                    CheckBordersMastermind();
                    UpdateMastermind2Players(); // Function used by player 1 to make a color combination.

                    // Switch case used on keyboard input which allows player 1 to choose colors and placement.
                    switch (GetKey()) {
                        case ConsoleKey.UpArrow:
                            MasterMind[SelectedColumn]++;
                            break;
                        case ConsoleKey.DownArrow:
                            MasterMind[SelectedColumn]--;
                            break;
                        case ConsoleKey.LeftArrow:
                            SelectedColumn--;
                            break;
                        case ConsoleKey.RightArrow:
                            SelectedColumn++;
                            break;
                        case ConsoleKey.Enter:
                            running = false;
                            break;
                    }
                }

                WriteLine("\\nYou chose the following color combination: ");

                for (int i = 0; i < 4; i++) {
                    switch (MasterMind[i]) {
                        case 0:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case 1:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                    }

                    // Prints out the color combination made by player 1.
                    Console.Write("██");
                    Console.ForegroundColor = ConsoleColor.White;

                    if (i != 3) {
                        Console.Write(" - ");
                    }
                }
            }
            WriteLine("Press 'enter' to continue.");
            Console.ReadLine();
            Console.Clear();
            #endregion
            #region slaveStart
            UpdateSlavemind(); // Function to clear the screen and set the game up for player 2.

            // Loop that keeps running as long as one has not won or lost.
            while (Win == false && Lost == false) {
                switch (GetKey()) {
                    case ConsoleKey.UpArrow:
                        IsTrying = false;
                        SlaveMind[SelectedColumn]++; // Updates the indicator of where the player is.
                        CheckBordersSlavemind(); // Function to make sure we're within the limits of the selectable column in a row.
                        UpdateSlavemind();
                        break;
                    case ConsoleKey.DownArrow:
                        IsTrying = false;
                        SlaveMind[SelectedColumn]--;
                        CheckBordersSlavemind();
                        UpdateSlavemind();
                        break;
                    case ConsoleKey.LeftArrow:
                        IsTrying = false;
                        SelectedColumn--;
                        CheckBordersSlavemind();
                        UpdateSlavemind();
                        break;
                    case ConsoleKey.RightArrow:
                        IsTrying = false;
                        SelectedColumn++;
                        CheckBordersSlavemind();
                        UpdateSlavemind();
                        break;
                    case ConsoleKey.Enter:
                        if (IsTrying == false) {
                            for (int i = 0; i < 4; i++) {
                                LastTry[i] = SlaveMind[i]; // Compaires the color of each index of the player and the colors to guess.
                            }

                            IsTrying = true; // IsTrying set to true so it doesn't keep looping within this statement.
                            UpdateSlavemind();
                            CheckWin(); // Function to check wether the player have won or lost and prints out the 10 players with the highest score.
                        }
                        break;
                    default:
                        break;
                }
            }
            WriteLine("\\nPress 'enter' to continue.");
            Console.ReadLine();
            return;

            // TODO: Player name, player id, wins, losses, win/loss ratio(debateable) to database.
            #endregion
        }

        public void CheckWin() {
            byte correctAmount = 0; // Used to check how many colors are correct.

            for (int i = 0; i < 4; i++) {
                if (slaveMind[i] == masterMind[i]) {
                    correctAmount++;
                }
            }

            // Statement to check if the correctAmount variable is above 4.
            // If it is, the player who solves the color combination wins and the one to make
            // the combination loses.
            if (correctAmount >= 4) {
                if (OnePlayer) {
                    WriteLine("\\nYou've won");
                    win = true;
                    dbStuff.EditScoreInDB(player1.Name, lives); // Edits score in the database according to performance via. the name of the player.
                    WriteLine("\\nPress 'enter' to continue.");
                    Console.ReadLine();
                    Console.Clear();
                    WriteLine("Top 10 highscore of players:\\n");
                    string scoreString = dbStuff.GetTop10FromDB(); // Function to print out players with a score among the 10 highest.
                    string[] scoreList = scoreString.Split(';');
                    foreach (string item in scoreList) {
                        if (item != "") {
                            string name = item;
                            int index = name.IndexOf(":");
                            if (index > 0)
                                name = name.Substring(0, index);
                            Console.ForegroundColor = player1.GetConsoleColor(dbStuff.GetColorFromDB(name));
                            WriteLine(item);
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else {
                    WriteLine($"\\n { player2.Name} won");
                    win = true;
                    dbStuff.EditScoreInDB(player1.Name, -5); // Subtracts score from player 1, who chose a color combination, via. the name of the player.
                    dbStuff.EditScoreInDB(player2.Name, lives); // Adds score to player 2, who solves a color combination, via. the name of the player.
                    WriteLine("\\nPress 'enter' to continue.");
                    Console.ReadLine();
                    Console.Clear();
                    WriteLine("Top 10 highscore of players:\\n");
                    string scoreString = dbStuff.GetTop10FromDB();
                    string[] scoreList = scoreString.Split(';');
                    foreach (string item in scoreList) {
                        if (item != "") {
                            string name = item;
                            int index = name.IndexOf(":");
                            if (index > 0)
                                name = name.Substring(0, index);

                            Console.ForegroundColor = player1.GetConsoleColor(dbStuff.GetColorFromDB(name));
                            WriteLine(item);
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            // If the correctAmount variable is below 4 the player, to solve the color combination, loses and 
            // the one to make the combination wins.
            else {
                WriteLine("\\nTry again!");
                lives--;

                if (lives <= 0) {
                    if (OnePlayer) {
                        Console.Clear();
                        WriteLine("You've lost the game");
                        lost = true;
                        dbStuff.EditScoreInDB(player1.Name, -5);
                        WriteLine("\\nPress 'enter' to continue.");
                        Console.ReadLine();
                        Console.Clear();
                        WriteLine("Top 10 highscore of players:\\n");
                        string scoreString = dbStuff.GetTop10FromDB();
                        string[] scoreList = scoreString.Split(';');
                        foreach (string item in scoreList) {
                            if (item != "") {
                                string name = item;
                                int index = name.IndexOf(":");
                                if (index > 0)
                                    name = name.Substring(0, index);

                                Console.ForegroundColor = player1.GetConsoleColor(dbStuff.GetColorFromDB(name));
                                WriteLine(item);
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else {
                        WriteLine($"\\n {player1.Name} won");
                        win = true;
                        dbStuff.EditScoreInDB(player1.Name, +5);
                        dbStuff.EditScoreInDB(player2.Name, -5);
                        WriteLine("\\nPress 'enter' to continue.");
                        Console.ReadLine();
                        Console.Clear();
                        WriteLine("Top 10 highscore of players:\\n");
                        string scoreString = dbStuff.GetTop10FromDB();
                        string[] scoreList = scoreString.Split(';');
                        foreach (string item in scoreList) {
                            if (item != "") {
                                string name = item;
                                int index = name.IndexOf(":");
                                if (index > 0)
                                    name = name.Substring(0, index);

                                Console.ForegroundColor = player1.GetConsoleColor(dbStuff.GetColorFromDB(name));
                                WriteLine(item);
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }

        /// <summary>
        /// Function to randomly generate a color combination.
        /// </summary>
        private void UpdateMastermind1Player() {
            Random rnd = new Random();

            for (int i = 0; i < 4; i++) {
                masterMind[i] = rnd.Next(0, 4); // generate a random combination

                switch (masterMind[i]) {
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    default:
                        break;
                }

                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = ConsoleColor.White;
            WriteLine("Color combination has been generated"); // Tell the player that a color combination has been generated.
        }

        /// <summary>
        /// Function with logic to choose a color combination yourself.
        /// </summary>
        public void UpdateMastermind2Players() {
            Console.Clear();
            WriteLine("Choose a combination among the colors.");
            WriteLine("\\nUse the Up, Down, Left and Right arrow keys to navigate\\nbetween columns and colors.");
            Console.Write("\n|");

            for (int i = 0; i < 4; i++) {
                switch (masterMind[i]) { // Case to print the representation of the color boxes.
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("██");
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("██");
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("██");
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("██");
                        break;
                }

                Console.ForegroundColor = ConsoleColor.White;

                if (i != 3) {
                    Console.Write(" - ");
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("|\n");

            // Case that displays which box is currently selected.
            switch (selectedColumn) {
                case 0:
                    WriteLine("|** -    -    -   |");
                    break;
                case 1:
                    WriteLine("|   - ** -    -   |");
                    break;
                case 2:
                    WriteLine("|   -    - ** -   |");
                    break;
                case 3:
                    WriteLine("|   -    -    - **|");
                    break;
                default:
                    break;
            }

            Console.Write("|\n");
            WriteLine("Press enter to confirm color pattern");
        }

        /// <summary>
        /// Function with logic for player 1, 2 if playing vs another, to solve the color combination.
        /// </summary>
        public void UpdateSlavemind() {
            Console.Clear();
            WriteLine("Welcome to Mastermind");
            PrintHistory();
            WriteLine("\\nYour opponent have chosen four color combinations for you to guess");
            WriteLine("\\nUse the Up, Down, Left and Right arrow keys to navigate\\nbetween columns and colors.");
            Console.Write("\n|");
            for (int i = 0; i < 4; i++) {
                switch (slaveMind[i]) { // Case to print the representation of the color boxes.
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("██");
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("██");
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("██");
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("██");
                        break;
                }
                Console.ForegroundColor = ConsoleColor.White;

                if (i != 3) {
                    Console.Write(" - ");
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("| ");

            #region correct color and place stuff

            int placedCorrect = 0;
            int colorCorrect = 0;
            int[] masterColors = CountColors(masterMind);

            for (int i = 0; i < 4; i++) {
                if (lastTry[i] == masterMind[i]) {
                    placedCorrect++;
                }
            }

            // Runs through the color combinations of the last try and 
            // sets the color and placement variables accordingly.
            foreach (int item in lastTry) {
                switch (item) {
                    case 0:
                        if (masterColors[0] > 0) { colorCorrect++; masterColors[0]--; }
                        break;
                    case 1:
                        if (masterColors[1] > 0) { colorCorrect++; masterColors[1]--; }
                        break;
                    case 2:
                        if (masterColors[2] > 0) { colorCorrect++; masterColors[2]--; }
                        break;
                    case 3:
                        if (masterColors[3] > 0) { colorCorrect++; masterColors[3]--; }
                        break;
                    default:
                        break;
                }
            }

            if (placedCorrect == 1) // If statement ment for proper grammar.
            {
                WriteLine($"[{placedCorrect}] is correct | [{colorCorrect}] has correct color");
            }
            else {
                WriteLine($"[{placedCorrect}] are correct | [{colorCorrect}] has correct color");
            }

            #endregion

            // When isTrying is true, which happens when the Enter button is pressed, it adds the last try to the previousTries variable.
            if (isTrying) {
                int[] tempint = lastTry.ToArray();
                previousTries.Add(new HistoryItem(tempint, placedCorrect, colorCorrect));
            }

            Console.Write("\n");

            // Case that displays which box is currently selected.
            switch (selectedColumn) {
                case 0:
                    WriteLine("|** -    -    -   |");
                    break;
                case 1:
                    WriteLine("|   - ** -    -   |");
                    break;
                case 2:
                    WriteLine("|   -    - ** -   |");
                    break;
                case 3:
                    WriteLine("|   -    -    - **|");
                    break;
                default:
                    break;
            }

            WriteLine("\\nLives remaining: " + lives);
        }

        /// <summary>
        /// Function to make sure we stay within the limits of selectable columns.
        /// </summary>
        public void CheckBordersSlavemind() {
            if (selectedColumn > 3) {
                selectedColumn = 3;
            }
            else if (selectedColumn < 0) {
                selectedColumn = 0;
            }

            if (slaveMind[selectedColumn] > 3) {
                slaveMind[selectedColumn] = 3;
            }
            else if (slaveMind[selectedColumn] < 0) {
                slaveMind[selectedColumn] = 0;
            }
        }

        /// <summary>
        /// Function to make sure we stay within the limits of the amount of boxes that are defined.
        /// </summary>
        public void CheckBordersMastermind() {
            if (selectedColumn > 3) {
                selectedColumn = 3;
            }
            else if (selectedColumn < 0) {
                selectedColumn = 0;
            }

            if (masterMind[selectedColumn] > 3) {
                masterMind[selectedColumn] = 3;
            }
            else if (masterMind[selectedColumn] < 0) {
                masterMind[selectedColumn] = 0;
            }
        }

        public void WriteLine(string str) {
            str = str.Replace("'", "\\'");
            string theScript = "print ('" + str + "')";
            se.Execute(theScript);
        }

        /// <summary>
        /// Function to print previous color combination the player have guessed.
        /// </summary>
        private void PrintHistory() {
            foreach (HistoryItem item in previousTries) { // Goes through every try that've been made.
                for (int i = 0; i < 4; i++) {
                    switch (item.Combination[i]) {
                        case 0:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("██");
                            break;
                        case 1:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("██");
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("██");
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("██");
                            break;
                    }

                    Console.ForegroundColor = ConsoleColor.White;

                    if (i != 3) {
                        Console.Write(" - ");
                    }
                }

                Console.Write($"[{item.PlacedCorrect}] placed correct | [{item.ColoredCorrect}] has correct color");
                Console.Write("\n");
            }
        }

        /// <summary>
        /// Function to used to help count colors.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private int[] CountColors(int[] array) {
            int[] result = new int[4];

            for (int ii = 0; ii < 4; ii++) {
                switch (array[ii]) {
                    case 0:
                        result[0]++;
                        break;
                    case 1:
                        result[1]++;
                        break;
                    case 2:
                        result[2]++;
                        break;
                    case 3:
                        result[3]++;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }
}