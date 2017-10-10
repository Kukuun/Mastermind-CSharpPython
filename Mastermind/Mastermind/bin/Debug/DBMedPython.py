import sqlite3 as lite
import sys

class DBManager:
    # Making some global variables.
    global con
    global cur

    con = lite.connect('dbHighscore.db') # Creating the file, if it doesn't exist.
    cur = con.cursor() # Creates a cursor.
    # Then the table is created, and adds 4 different columns.
    cur.execute("CREATE TABLE IF NOT EXISTS tblHighscore(fldID INTEGER PRIMARY KEY, fldName TEXT, fldScore INT, fldColor TEXT)")

    def SelectTop10FromDB(self):
        try:    
            with con:
                cur.execute("SELECT * FROM tblHighscore ORDER BY fldScore DESC LIMIT 10") # Gets the top 10 from the highscore table, and orders by the score.
                rows = cur.fetchall() # The result is saved in the variable rows.

                # Makes a string, that is being filled in with all the names and their score in the for loop.
                scoreList = ""
                for row in rows:
                    scoreList += row[1] + ": " + str(row[2]) + ";"
                return scoreList
        # Throws an exception if the code above fails.
        except Exception, e:
            print "FATAL ERROR %s: " % e.args[0]
            sys.exit(1)

    def SelectNameFromDB(self, name):
        try:    
            with con:
                cur.execute("SELECT fldName FROM tblHighscore WHERE fldName='" + name + "'") # Gets all the names from the table, where fldName is equal to the name variable. So actually only the player.
                rows = cur.fetchone() # The result is saved in the variable rows, but only the first result. But there should only be one anyways.
                # If the user entered a new name, that wasn't already in the database. Then we return a custom string, to avoid errors.
                if rows is None:
                    return "42YouCanHaveThisName42"
                # If the user entered an already stored username, return that username.
                else:
                    nameFound = rows[0]
                    return nameFound
        except Exception, e:
            print "FATAL ERROR %s: " % e.args[0]
            sys.exit(1)

    def SelectColorFromDB(self, name):
        try:    
            with con:
                cur.execute("SELECT fldColor FROM tblHighscore WHERE fldName='" + name + "'") # Gets the color for the individual username.
                rows = cur.fetchone() # The result is saved in the rows variable.
                color = rows[0]
                # Returns the color found.
                return color
        except Exception, e:
            print "FATAL ERROR %s: " % e.args[0]
            sys.exit(1)

    def InsertIntoDB(self, name, color):
        try:    
            with con:
                cur.execute("INSERT INTO tblHighscore VALUES(NULL,'" + name + "',0,'" + color + "')") # Inserts a new user into the highscore table, with the name they wrote, and color they chose.
        except Exception, e:
            print "FATAL ERROR %s: " % e.args[0]
            sys.exit(1)

    def UpdateScoreInDB(self, name, score):
        try:
            with con:
                cur.execute("UPDATE tblHighscore SET fldScore = fldScore + " + score + " WHERE fldName = '" + name + "'") # Updates the players highscore, from the name the player logged in with.
        except Exception, e:
            print "FATAL ERROR %s: " % e.args[0]
            sys.exit(1)