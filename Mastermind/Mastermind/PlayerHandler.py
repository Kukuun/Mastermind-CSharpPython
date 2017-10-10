from System import Console, ConsoleColor
import sys

class PlayerHandler:

    #Method of changing the color which player chose
    def ChangeColor(self, myChoice):
        #Try to change the color based on the color chosen
        try:
            #A Dictionary // as we know from C# a Switch case, to choose the correct color
            return {
            'white': ConsoleColor.White,
            'red': ConsoleColor.Red,
            'green': ConsoleColor.Green,
            'blue': ConsoleColor.Blue,
            'yellow': ConsoleColor.Yellow,
        }.get(myChoice, 'white') #x = the choice 'white' = default if something else was written

        except Exception, e:
            print "FATAL ERROR %s: " % e.args[0]
            sys.exit(1)
    