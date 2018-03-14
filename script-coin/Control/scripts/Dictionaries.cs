using System;
using System.Collections.Generic;

namespace scriptcoin
{
    public class Util
    {
        /*
        public static readonly Dictionary<string, string> Commands = new Dictionary<string, string>()
        {
            {"new", "Creates and displays information about new wallet"},
            {"send", "Send Script-Coin to a destination"},
            {"wallet", "Displays information about current wallet"},
            {"code", "Opens programming window"},
            {"mine", "Begins mining Script-Coin"},
            {"joke", "Displays a random joke"},
            {"help", "Displays help menu"},
        };
        */
        public static readonly List<Command> Commands = new List<Command>()
        {
            new Wallet(),
            new New(),
            new Mine(),
            new Send(),
            new Code(),
            new Joke(),
            new Clear(),
            new Help(),
            new Quit()
        };

        public static void WriteColor(string input, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(input);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLineColor(string input, ConsoleColor color) =>
            WriteColor(input + "\n", color);

        public static void PrintError(string message) =>
            WriteLineColor("Error: " + message, ConsoleColor.Red);

        public static void PrintWarning(string message) =>
            WriteLineColor("Warning: " + message, ConsoleColor.Yellow);

        public static void PrintInfo(string message) =>
            WriteLineColor("Info: " + message, ConsoleColor.Gray);

        public static string ReadLineColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
            string input = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            return input;
        }

        public static readonly List<string> Jokes = new List<string>()
        {
            {"Whats the only shop thats 35+? A printer store!!" },
            {"When does a dad joke become a dad joke? When it's fully groan!!" },
            {"Why did the chicken cross the road? To get to the other side!!" },
            {"Did you hear about the restaurant on the moon? Great food, no atmosphere!!" },
            {"What do you call a fake noodle? An Impasta!!" },
            {"How many apples grow on a tree? All of them!!" },
            {"Want to hear a joke about paper? Nevermind it's tearable!!" },
            {"I just watched a program about beavers. It was the best dam program I've ever seen." },
            {"Why did the coffee file a police report? It got mugged." },
            {"How does a penguin build it's house? Igloos it together." },
            {"Dad, did you get a haircut? No I got them all cut." },
            {"What do you call a Mexican who has lost his car? Carlos." },
            {"Dad, can you put my shoes on? No, I don't think they'll fit me." },
            {"Why did the scarecrow win an award? Because he was outstanding in his field." },
            {"Why don't skeletons ever go trick or treating? Because they have no body to go with." },
            {"Ill call you later. Don't call me later, call me Dad." },
            {"What do you call an elephant that doesn't matter? An irrelephant" },
            {"Want to hear a joke about construction? I'm still working on it." },
            {"What do you call cheese that isn't yours? Nacho Cheese." },
            {"Why couldn't the bicycle stand up by itself? It was two tired." },
            {"What did the grape do when he got stepped on? He let out a little wine." },
            {"I wouldn't buy anything with velcro. It's a total rip-off." },
            {"The shovel was a ground-breaking invention." },
            {"Dad, can you put the cat out? I didn't know it was on fire." },
            {"This graveyard looks overcrowded. People must be dying to get in there." },
            {"Whenever the cashier at the grocery store asks my dad if he would like the milk in a bag he replies, 'No, just leave it in the carton!'" },
            {"5/4 of people admit that they’re bad with fractions." },
            {"Two goldfish are in a tank. One says to the other, 'do you know how to drive this thing?'" },
            {"What do you call a man with a rubber toe? Roberto." },
            {"What do you call a fat psychic? A four-chin teller." },
            {"I would avoid the sushi if I was you. It’s a little fishy." },
            {"To the man in the wheelchair that stole my camouflage jacket... You can hide but you can't run." },
            {"The rotation of earth really makes my day." },
            {"I thought about going on an all-almond diet. But that's just nuts" },
            {"What's brown and sticky? A stick." },
            {"I’ve never gone to a gun range before. I decided to give it a shot!" },
            {"Why do you never see elephants hiding in trees? Because they're so good at it." },
            {"Did you hear about the kidnapping at school? It's fine, he woke up." },
            {"A furniture store keeps calling me. All I wanted was one night stand." },
            {"I used to work in a shoe recycling shop. It was sole destroying." },
            {"Did I tell you the time I fell in love during a backflip? I was heels over head." },
            {"I don’t play soccer because I enjoy the sport. I’m just doing it for kicks." },
            {"People don’t like having to bend over to get their drinks. We really need to raise the bar." },
            {"Why was the snowman looking through a bag of carrots? He was picking his nose." },
            {"What's invisible and smells like carrots? Rabbit farts." },
            {"Did you hear the joke about the king? It ruled." },
            {"What did the elevator say to the other elevator? 'I think I'm coming down with something'" },
            {"How many apples grow on a tree? All of them." },
            {"What did the astronaut's fiancée say when he proposed in space? `I can't breathe!`" },
            {"Why did the tomato blush? Because it saw the salad dressing." },
            {"I witnessed an attempted murder earlier, luckily only one crow showed up." },
            {"What's Whitney Houston's favorite type of coordingation? HAAAAAAAAAAAAAAAAAND EEEEEEEEEEEEEEEEEYEEEEEEEEE🎶" },
            {"What's at the bottom of the ocean and shivers? A nervous wreck." },
            {"Why did Cinderella get kicked off the soccer team? Because she kept running from the ball." },
            {"How do crazy people get through the forest? They take the psycho-path" },
            {"What do you give a sick bird? Tweetment." },
            {"Why do scuba divers fall backwards off the boat? If they fell forwards they'd still be on the boat." },
            {"What do you call a fake noodle? An impasta." },
            {"I went ot buy some camouflage trousers the other day but I couldn't find any." },
            {"What do you call a fish with no eye? Fsh." },
            {"Why did the orange stop rolling around? It ran out of juice" },
            {"Two fish swim into a concrete wall. One says to the other `dam!`" },
            {"Archeaologist: a person whose career lies in ruins." },
            {"A ship with red paint hit a ship with blue paint, both we marooned." },
            {"She criticized my apartment, so I knocked her flat." },
            {"What's the difference between a hippo and a Zippo? A hippo is really heavy and a Zippo is a little lighter." },
            {"I really like deep sea monster jokes, I keep Kraken up." },
            {"I only like 25 letters of the alphabet, I don't really know Y" },
            {"What sound does a sleeping T-Rex make? A dino-snore." },
            {"A man sued an airline company after he lost his luggage. Sadly, he lost his case." },
            {"An atom loses an electron, it says, 'Man I really gotta keep an ion them`" },
            {"6:30 is the best time on a clock, hands down." },
            {"I hate how early funerals are, I'm not really a mourning person." },
            {"I lost my job at the bank, a woman told me to check her balance so I pushed her over." },
            {"Did you hear about the two silk worms in a race? It ended in a tie" }
            {"Last time I stole a calendar, I got 12 months" },
            {"" }
        };
    }
}

