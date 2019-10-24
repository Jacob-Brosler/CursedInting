using System;

namespace CursedInting
{
    class Program
    {
        public static uint state;

        static void Main(string[] args)
        {
            Console.WriteLine("What is your starting state?");
            string input = Console.ReadLine();
            state = uint.TryParse(input, out state) ? state : 0;
            Console.WriteLine("Now running on state: " + Convert.ToString(state, 2));
            Run();
        }

        static void Run()
        {
            //If just started, ask for name
            if (state == 0)
            {
                Console.WriteLine("What is your name brave adventurer? (Valid characters: a-z, +, -, =, _, ?)");
                string input = Console.ReadLine().ToLower();

                //5 is the max number of supported characters
                for (int i = 0; i < 5; i++)
                {
                    state <<= 5;
                    if (i < input.Length)
                    {
                        //Question mark is the default for an unsupported character
                        uint result = 31;
                        if (input[i] >= 'a' && input[i] <= 'z')
                            result = (uint)(input[i] - 96);
                        else if (input[i] == '+')
                            result = 27;
                        else if (input[i] == '-')
                            result = 28;
                        else if (input[i] == '=')
                            result = 29;
                        else if (input[i] == '_')
                            result = 30;

                        state += result;
                    }
                }
                state <<= 7;
                //Console.WriteLine(Convert.ToString(state, 2) + " " + GetName());
                Console.WriteLine("Welcome to the spooky scary mansion " + GetName() + ".");
                Console.WriteLine("You walk into the entryway of the mansion. The door creaks as it slowly shuts behind you. The room itself is somewhat in a " +
                    "state of disrepair as dust and spiders have taken over most of the available space. A lonely, broken chandelier hangs from the ceiling high above you. " +
                    "The exit lays closed behind you and the way to the dining hall lies ahead.");
            }
            //If there is state but no name, go away
            else if (state < 128)
            {
                Console.WriteLine("You try to remember who you are, but you only scream for you cannot know.\nTry a valid state next time headass.");
                return;
            }
            //Main game loop
            else
            {
                Console.WriteLine("What would you like to do, " + GetName() + "?");
                string input = Console.ReadLine().ToLower();
                string[] splitInput = input.IndexOf(" ") != -1 ? new string[] { input.Substring(0, input.IndexOf(" ")), input.Substring(input.IndexOf(" ") + 1) }
                    : new string[] { input };
                if (splitInput.Length >= 1)
                {
                    switch (splitInput[0])
                    {
                        case "help":
                            Console.WriteLine("Available commands: \nHelp: Prints out available commands with small descriptions.\n" +
                                "Move <Room>: Moves to the specified room if possible.\nInspect: Prints a description of the room you are in, noting any important items\n" +
                                "Inspect <Object>: Prints a description of a specific item in the room if it exists.\nTake <Object>: Takes a valid item and puts it in your inventory.\n" +
                                "Use <Item>: Uses a given item if it can be used on anything in the room you are in.");
                            break;
                        case "move":
                            if (splitInput.Length < 2)
                            {
                                Console.WriteLine("Invalid move command: No target given.");
                                break;
                            }
                            switch (GetCurrentRoom())
                            {
                                case "entryway":
                                    switch (splitInput[1])
                                    {
                                        case "exit":
                                            if ((state & 1) == 1)
                                            {
                                                Console.WriteLine("You leave the mansion, Rusty loot in tow. You turn back for only a single second as your farewell to this great adversary escape your lips. \"Bet.\"\n\n" +
                                                    "Congratulations, you win Cursed Inting!");
                                                return;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Having found nothing of real value you leave the mansion dejectedly. Coward.");
                                                return;
                                            }
                                        case "dining hall":
                                            Console.WriteLine("As you move into the dining hall you notice a large ornate wooden table filling the center of the room. There are a handful of chairs in scattered disrepair around it, and it is completely clear of items. There is a path back to the entryway behind you and a path to the kitchen to your right.");
                                            OverwriteEntry(5, 2, 1);
                                            break;
                                        default:
                                            Console.WriteLine("Invalid move command: Invalid target " + splitInput[1] + " given.");
                                            break;
                                    }
                                    break;
                                case "dining hall":
                                    switch (splitInput[1])
                                    {
                                        case "entryway":
                                            Console.WriteLine("The entryway is just as dusty as you remember, however most of the spiders have been scared off by your movements. Everything else is just as you remember. The exit is in front of you and the way to the dining hall is behind you.");
                                            OverwriteEntry(5, 2, 0);
                                            break;
                                        case "kitchen":
                                            Console.WriteLine("The kitchen is somewhat barren, with dust and spiders just as bad as the entryway. You smell something you're pretty sure is the trash can but you definitely don't want to go over and find out. There is a propane fridge and an ice box to your left. Behind you is the way back to the dining hall, and to your right there is another door that you think leads back to the entryway.");
                                            OverwriteEntry(5, 2, 2);
                                            break;
                                        default:
                                            Console.WriteLine("Invalid move command: Invalid target " + splitInput[1] + " given.");
                                            break;
                                    }
                                    break;
                                case "kitchen":
                                    switch (splitInput[1])
                                    {
                                        case "entryway":
                                            Console.WriteLine("As the door shuts behind you you notice that it is now flush with the wall. It is probably meant to only be one way. The entryway is just as dusty as you remember, however most of the spiders have been scared off by your movements. Everything else is just as you remember. The exit is to your left and the way to the dining hall is to your right.");
                                            OverwriteEntry(5, 2, 0);
                                            break;
                                        case "dining hall":
                                            Console.WriteLine("As you move into the dining hall you notice a large ornate wooden table filling the center of the room. There are a handful of chairs in scattered disrepair around it, and it is completely clear of items. There is a path back to the kitchen behind you and a path to the entryway to your left.");
                                            OverwriteEntry(5, 2, 1);
                                            break;
                                        case "vault":
                                        case "mysterious vault":
                                            //If they've discovered the vault
                                            if (GetValue(1, 1) == 1)
                                            {
                                                //If they re-closed the door
                                                if (GetValue(2, 2) == 2)
                                                {
                                                    Console.WriteLine("The passageway you found is no longer there. You think flipping the lever might have closed it back up.");
                                                }
                                                //If the door is open
                                                else if (GetValue(2, 2) == 3)
                                                {
                                                    Console.WriteLine("Moving through the passageway and down a flight of stone steps leads you to a small room deep in the mansion. In it you find gold, jewels, and other useless baubles. Then you finally find it, what you came for: Rust! This place is so old and damp it has all the Rust you could ever want!");
                                                    OverwriteEntry(5, 2, 3);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Invalid state reached: Play through the game normally you lazy bum.");
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid move command: Invalid target " + splitInput[1] + " given.");
                                            }
                                            break;
                                        default:
                                            Console.WriteLine("Invalid move command: Invalid target " + splitInput[1] + " given.");
                                            break;
                                    }
                                    break;
                                case "vault":
                                    switch (splitInput[1])
                                    {
                                        case "kitchen":
                                            Console.WriteLine("The kitchen appears just as you left it. You still smell something you're pretty sure is the trash can but you still definitely don't want to go over and find out. There is a propane fridge and an ice box to your left. Behind you is the way back to the vault, to your right is the door to the dining hall, and ahead of you there is another door that you think leads back to the entryway.");
                                            OverwriteEntry(5, 2, 2);
                                            break;
                                        default:
                                            Console.WriteLine("Invalid move command: Invalid target " + splitInput[1] + " given.");
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case "inspect":
                            bool globalInspectUsed = false;
                            if (splitInput.Length >= 2)
                            {
                                switch (splitInput[1])
                                {
                                    case "ceiling":
                                        Console.WriteLine("The ceiling is pretty high up and hard to see, but it seems to be made mostly out of wood. It still looks pretty structurally sound but you don't know how much longer that will last.");
                                        globalInspectUsed = true;
                                        break;
                                    case "floor":
                                        Console.WriteLine("The floor is made of very worn wood. It seems very highly trafficked but it looks safe.");
                                        globalInspectUsed = true;
                                        break;
                                    case "wall":
                                    case "walls":
                                    case "wallpaper":
                                        Console.WriteLine("The wallpaper is peeling and extremely faded but you think you can make out a faint flower pattern.");
                                        globalInspectUsed = true;
                                        break;
                                    case "dust":
                                        Console.WriteLine("The dust is extremely dusty. A very nice brown color.");
                                        globalInspectUsed = true;
                                        break;
                                    case "nail":
                                    case "nails":
                                        Console.WriteLine("The nails are also nailed down. Whoever did this was very thorough or very paranoid!");
                                        globalInspectUsed = true;
                                        break;
                                    case "spider":
                                    case "spiders":
                                        Console.WriteLine("You try to get closer to inspect them better, but as you do it jumps up and bites you. You die.");
                                        return;
                                    case "key":
                                        if (GetValue(4, 1) == 1)
                                        {
                                            Console.WriteLine("The key is very simple and unassuming in nature. There are no markings to suggest what it might be used for.");
                                            globalInspectUsed = true;
                                        }
                                        break;
                                }
                            }
                            if (!globalInspectUsed)
                            {
                                switch (GetCurrentRoom())
                                {
                                    case "entryway":
                                        if (splitInput.Length < 2 || splitInput[1] == GetCurrentRoom())
                                        {
                                            Console.WriteLine("The room you're in is in somewhat of a state of disrepair as dust and spiders have taken over most of the available space. " +
                                                "A lonely, broken chandelier hangs from the ceiling high above you. There is also a small locked box attached to the wall near you. " +
                                                "The exit lays closed behind you and the way to the dining hall lies ahead.");
                                        }
                                        else
                                        {
                                            switch (splitInput[1])
                                            {
                                                case "chandelier":
                                                    Console.WriteLine("You can't get close to the chandelier due to its height but from where you are it looks very broken. It's missing " +
                                                        "one of its branches and all of the candles are either melted down or cut at extremely odd angles. You don't know how it hasn't fallen down already.");
                                                    break;
                                                case "box":
                                                case "lock box":
                                                case "locked box":
                                                    Console.Write("The box shows a few signs of rusting on the face but it extremely solidly built. It seems to be firmly attached to the wall. ");
                                                    if (GetValue(2, 2) == 0)
                                                        Console.WriteLine("It has a keyhole in the front of it. " + (GetValue(4, 1) == 1 ? "Maybe your key will fit?" : ""));
                                                    else if (GetValue(2, 2) == 1 || GetValue(2, 2) == 3)
                                                        Console.WriteLine("The lever inside is in what you assume is the 'on' position.");
                                                    else
                                                        Console.WriteLine("The lever inside is in what you assume is the 'off' position.");
                                                    break;
                                                case "door":
                                                case "exit":
                                                    Console.Write("The door has massive gouges all across its inner face but whatever caused them appears to have already left. It is unlocked ");
                                                    if ((state & 1) == 1)
                                                    {
                                                        Console.WriteLine("and you can leave at any time, Rust loot in tow!");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("but you haven't comleted your objective yet.");
                                                    }
                                                    break;
                                                case "lever":
                                                    if (GetValue(2, 2) == 0)
                                                        Console.WriteLine("Invalid inspect command: Invalid target " + splitInput[1] + " given.");
                                                    else if (GetValue(2, 2) == 1 || GetValue(2, 2) == 3)
                                                        Console.WriteLine("The lever is in what you assume is the 'on' position.");
                                                    else
                                                        Console.WriteLine("The lever is in what you assume is the 'off' position.");
                                                    break;
                                                default:
                                                    Console.WriteLine("Invalid inspect command: Invalid target " + splitInput[1] + " given.");
                                                    break;

                                            }
                                        }
                                        break;
                                    case "dining hall":
                                        if (splitInput.Length < 2 || splitInput[1] == GetCurrentRoom())
                                        {
                                            Console.WriteLine("The dining hall you notice a large ornate wooden table filling the center of the room. There are a handful of chairs " +
                                                "in scattered disrepair around it, and it is completely clear of items.  There is a path to the kitchen and a path to the entryway.");
                                        }
                                        else
                                        {
                                            switch (splitInput[1])
                                            {
                                                case "table":
                                                    Console.WriteLine("The table is a beautifully carved wooden table with lion's head shaped feet, a celtic pattern carved around the " +
                                                        "edge and a massive celtic knot in the middle. It looks very expensive and has survived the years surprisingly well, but it seems extremely heavy.");
                                                    if (GetValue(4, 1) == 0)
                                                        Console.WriteLine("After further inspection you notice that there is a key attached to the underside of the table.");
                                                    break;
                                                case "chair":
                                                case "chairs":
                                                    Console.WriteLine("The chairs are all broken, with some missing legs and some having split backs. Interestingly, they all seem to be securely nailed to the floor.");
                                                    break;
                                                case "key":
                                                    Console.WriteLine("The key is very simple and unassuming in nature. There are no markings to suggest what it might be used for.");
                                                    break;
                                                default:
                                                    Console.WriteLine("Invalid inspect command: Invalid target " + splitInput[1] + " given.");
                                                    break;
                                            }
                                        }
                                        break;
                                    case "kitchen":
                                        if (splitInput.Length < 2 || splitInput[1] == GetCurrentRoom())
                                        {
                                            Console.WriteLine("The kitchen is somewhat barren, with dust and spiders everywhere. You smell something you're pretty sure is the " +
                                                "trash can but you definitely don't want to go over and find out. There is a propane fridge and an ice box along one of the walls. " +
                                                "There is also the way back to the dining hall and another door that you think leads back to the entryway.");
                                        }
                                        else
                                        {
                                            switch (splitInput[1])
                                            {
                                                case "trash":
                                                case "trash can":
                                                    Console.WriteLine("The smell just gets worse as you get closer to the trash can. You try to wave away the stench as best as you " +
                                                        "can as you approach but nothing can save you from that god-forsaken scent. It gets worse and worse as you get closer to the " +
                                                        "source. Just as you get close enough to almost see what's causing this sin against your senses the smell causes you to fall " +
                                                        "into a coma and you die of asphyxiation.");
                                                    return;
                                                case "ice box":
                                                    Console.WriteLine("As you open the lid to peer into the ice box it sudden stands up and clamps down on your body, cutting you in half. You are dead.");
                                                    return;
                                                case "fridge":
                                                case "propane fridge":
                                                    Console.Write("The fridge itself is a simple metal box meant to keep food cold through the use of a specially placed pilot light. " +
                                                        "However, it doesn't look like its had propane in it for a very long time. ");
                                                    if (GetValue(2, 2) == 0 || GetValue(2, 2) == 2)
                                                    {
                                                        if (GetValue(1, 1) == 0)
                                                            Console.WriteLine("Opening the fridge reveals... the wall of the room? That's weird.");
                                                        else
                                                            Console.WriteLine("The passage you saw earlier is now covered up by the wall. Maybe something you did closed it?");
                                                    }
                                                    else if (GetValue(2, 2) == 1)
                                                        Console.WriteLine("Opening the fridge reveals... the wall of the room? That's weird. There's a thin break in the wall along " +
                                                            "the right wall of the fridge as a panel appears to be partially moved but you can't seem to get it to move any more.");
                                                    else
                                                    {
                                                        Console.WriteLine("Opening the fridge reveals... a stone staircase spiraling downwards into darkness. Could this be the mysterious vault you've heard rumors of?");
                                                        OverwriteEntry(1, 1, 1);
                                                    }
                                                    break;
                                                default:
                                                    Console.WriteLine("Invalid inspect command: Invalid target " + splitInput[1] + " given.");
                                                    break;
                                            }
                                        }
                                        break;
                                    case "vault":
                                        if (splitInput.Length < 2 || splitInput[1] == GetCurrentRoom())
                                        {
                                            Console.Write("You are currently in a small room deep in the mansion. Most of the floor is taken up by gold, jewels, and other useless baubles. ");
                                            //If you already took the rust
                                            if (GetValue(0, 1) == 1)
                                                Console.WriteLine("However, you've already got all you could ever need from this room.");
                                            else
                                                Console.WriteLine("However, why would you care about that? You've already got all the Rust you could ever need.");
                                        }
                                        else
                                        {
                                            switch (splitInput[1])
                                            {
                                                case "gold":
                                                case "silver":
                                                case "jewel":
                                                case "jewels":
                                                case "bauble":
                                                case "baubles":
                                                case "useless bauble":
                                                case "useless baubles":
                                                    Console.WriteLine("Yea yea yea, they're shiny and all but why would you need them when you have Rust?");
                                                    break;
                                                case "ruby":
                                                case "rubies":
                                                    Console.WriteLine("Rust is just better than Ruby, why would you ever want to use that?");
                                                    break;
                                                case "rwby":
                                                case "rwbies":
                                                    Console.WriteLine("It's also a gun?");
                                                    break;
                                                case "emerald":
                                                case "emeralds":
                                                    Console.WriteLine("Huh, that's weird. The emeralds disappeared as soon as you tried to get a better look. Well who needs them anyways when you have Rust!");
                                                    break;
                                                case "rust":
                                                    Console.WriteLine("It's so plentiful and beautiful! It's everything you could have ever asked for!");
                                                    break;
                                                default:
                                                    Console.WriteLine("Invalid inspect command: Invalid target " + splitInput[1] + " given.");
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                        case "take":
                            if (splitInput.Length < 2)
                            {
                                Console.WriteLine("Invalid move command: No target given.");
                                break;
                            }
                            bool globalTakeUsed = false;
                            switch (splitInput[1])
                            {
                                case "ceiling":
                                case "floor":
                                case "wall":
                                case "walls":
                                    Console.WriteLine("No. Just no.");
                                    globalTakeUsed = true;
                                    break;
                                case "wallpaper":
                                    Console.WriteLine("You try to peel some of the wallpaper off of the wall but the small amount that isn't still firmly adhered just breaks apart to dust as you try to take it.");
                                    globalTakeUsed = true;
                                    break;
                                case "dust":
                                    Console.WriteLine("As you try to pick up the dust it flies out of your hand and disperses into the air.");
                                    globalTakeUsed = true;
                                    break;
                                case "nail":
                                case "nails":
                                    Console.WriteLine("The nails are also nailed down. Whoever did this was very thorough or very paranoid!");
                                    globalTakeUsed = true;
                                    break;
                                case "spider":
                                case "spiders":
                                    Console.WriteLine("You try to get closer to grab them, but as you do it jumps up and bites you. You die.");
                                    return;
                            }

                            if (!globalTakeUsed)
                            {
                                switch (GetCurrentRoom())
                                {
                                    case "entryway":
                                        switch (splitInput[1])
                                        {
                                            case "chandelier":
                                                Console.WriteLine("You can't get close enough to take the chandelier due to how high up it is.");
                                                break;
                                            case "box":
                                            case "lock box":
                                            case "locked box":
                                                Console.Write("The box is solidly nailed to the wall and you can't seem to get a good enough grip on it to rip it off.");
                                                break;
                                            case "door":
                                            case "exit":
                                                Console.Write("As you try to pull the door of its hinges it suddenly swings up from the bottom and you're launched into the ceiling. You splat into a nice red puddle and die.");
                                                return;
                                            case "lever":
                                                if (GetValue(2, 2) == 0)
                                                    Console.WriteLine("Invalid inspect command: Invalid target " + splitInput[1] + " given.");
                                                else
                                                {
                                                    Console.Write("You pull as hard as you can on the lever but can't get it out of the wall. As you struggle with it you accidentally flip it. ");
                                                    if (GetValue(2, 2) == 1 || GetValue(2, 2) == 3)
                                                    {
                                                        Console.WriteLine("The lever is now in what you assume is the 'off' position.");
                                                        OverwriteEntry(2, 2, 2);
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("The lever is now in what you assume is the 'on' position. You hear a faint sound of stone scraping on stone coming from another part of the house");
                                                        OverwriteEntry(2, 2, 3);
                                                    }
                                                }
                                                break;
                                            default:
                                                Console.WriteLine("Invalid take command: Invalid target " + splitInput[1] + " given.");
                                                break;

                                        }
                                        break;
                                    case "dining hall":
                                        switch (splitInput[1])
                                        {
                                            case "table":
                                                Console.WriteLine("The table is solidly nailed down to the floor and you can't seem to move it at all.");
                                                break;
                                            case "chair":
                                            case "chairs":
                                                Console.WriteLine("The chairs are all solidly nailed down to the floor and you can't seem to move them at all.");
                                                break;
                                            case "key":
                                                if (GetValue(4, 1) == 1)
                                                    Console.WriteLine("You already have the key.");
                                                else
                                                {
                                                    Console.WriteLine("You take the small key. This could be useful later.");
                                                    OverwriteEntry(4, 1, 1);
                                                }
                                                break;
                                            default:
                                                Console.WriteLine("Invalid take command: Invalid target " + splitInput[1] + " given.");
                                                break;
                                        }
                                        break;
                                    case "kitchen":
                                        switch (splitInput[1])
                                        {
                                            case "trash":
                                            case "trash can":
                                                Console.WriteLine("The smell just gets worse as you get closer to the trash can. You try to wave away the stench as best as you " +
                                                    "can as you approach but nothing can save you from that god-forsaken scent. It gets worse and worse as you get closer to the " +
                                                    "source. Just as you get close enough to almost see what's causing this sin against your senses the smell causes you to fall " +
                                                    "into a coma and you die of asphyxiation.");
                                                return;
                                            case "ice box":
                                                Console.WriteLine("As you move to grab the ice box it sudden stands up and clamps down on your body, cutting you in half. You are dead.");
                                                return;
                                            case "fridge":
                                            case "propane fridge":
                                                Console.WriteLine("The fridge is very solidly adhered to the wall and floor, but by what you can't tell.");
                                                break;
                                            default:
                                                Console.WriteLine("Invalid take command: Invalid target " + splitInput[1] + " given.");
                                                break;
                                        }
                                        break;
                                    case "vault":
                                        switch (splitInput[1])
                                        {
                                            case "gold":
                                            case "silver":
                                            case "jewel":
                                            case "jewels":
                                            case "bauble":
                                            case "baubles":
                                            case "useless bauble":
                                            case "useless baubles":
                                                Console.WriteLine("Yea yea yea, they're shiny and all but why would you need them when you have Rust?");
                                                break;
                                            case "ruby":
                                            case "rubies":
                                                Console.WriteLine("Rust is just better than Ruby, why would you ever want to take that?");
                                                break;
                                            case "rwby":
                                            case "rwbies":
                                                Console.WriteLine("You get shot by a customizable high impact sniper rifle, punched and shotgunned in the face, and get nailed by title IX all in the span of one breath. You die.");
                                                return;
                                            case "emerald":
                                            case "emeralds":
                                                Console.WriteLine("Huh, that's weird. The emeralds disappeared as soon as you tried to take them. Well who needs them anyways when you have Rust!");
                                                break;
                                            case "rust":
                                                //If you already took the rust
                                                if (GetValue(0, 1) == 1)
                                                    Console.WriteLine("Sadly, you've already scrubbed every inch of this place for all the Rust it had. There's no more left to grab.");
                                                else
                                                {
                                                    Console.WriteLine("It's so plentiful and beautiful! It's everything you could have ever asked for! Just holding it in your hand gives you an overwhelming sense of safety like nothing ever had before.");
                                                    OverwriteEntry(0, 1, 1);
                                                }
                                                break;
                                            default:
                                                Console.WriteLine("Invalid take command: Invalid target " + splitInput[1] + " given.");
                                                break;
                                        }
                                        break;
                                }
                            }
                            break;
                        case "use":
                            if (splitInput.Length < 2)
                            {
                                Console.WriteLine("Invalid use command: No target given.");
                                break;
                            }
                            switch (GetCurrentRoom())
                            {
                                case "entryway":
                                    switch (splitInput[1])
                                    {
                                        case "lever":
                                            if (GetValue(2, 2) == 1 || GetValue(2, 2) == 3)
                                            {
                                                Console.WriteLine("The lever is now in what you assume is the 'off' position.");
                                                OverwriteEntry(2, 2, 2);
                                            }
                                            else if (GetValue(2, 2) == 2)
                                            {
                                                Console.WriteLine("The lever is now in what you assume is the 'on' position. You hear a faint sound of stone scraping on stone coming from another part of the house");
                                                OverwriteEntry(2, 2, 3);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid use command: Invalid target " + splitInput[1] + " given.");
                                            }
                                            break;
                                        case "key":
                                            if (GetValue(4, 1) == 1)
                                            {
                                                if (GetValue(2, 2) == 0)
                                                {
                                                    Console.WriteLine("You try to put the key in the hole and it fits like a glove. Opening the box reveals a lever built into the wall. Before even " +
                                                        "thinking you pull it and hear a small grinding noise before it suddenly stops.");
                                                    OverwriteEntry(2, 2, 1);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("You've already opened the locked box.");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid use command: Invalid target " + splitInput[1] + " given.");
                                            }
                                            break;
                                        default:
                                            Console.WriteLine("Invalid use command: Invalid target " + splitInput[1] + " given.");
                                            break;
                                    }
                                    break;
                                default:
                                    switch (splitInput[1])
                                    {
                                        case "lever":
                                            if (GetValue(2, 2) != 0)
                                                Console.WriteLine("This is the wrong room to use this item.");
                                            else
                                                Console.WriteLine("Invalid use command: Invalid target " + splitInput[1] + " given.");

                                            break;
                                        case "key":
                                            if (GetValue(4, 1) == 1)
                                                Console.WriteLine("This is the wrong room to use this item.");
                                            else
                                                Console.WriteLine("Invalid use command: Invalid target " + splitInput[1] + " given.");

                                            break;
                                        default:
                                            Console.WriteLine("Invalid use command: Invalid target " + splitInput[1] + " given.");
                                            break;
                                    }
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid command given: Command not recognized.");
                            break;
                    }
                }
            }
            Run();
        }

        /// <summary>
        /// Gets the name of the player from storage
        /// </summary>
        /// <returns>Name of player as a string</returns>
        static string GetName()
        {
            string name = "";
            for (int i = 1; i < 6; i++)
            {
                uint letterCode = GetValue(32 - i * 5, 5);
                if (letterCode > 0 && letterCode <= 26)
                    name += (char)(letterCode + 96);
                else if (letterCode == 27)
                    name += '+';
                else if (letterCode == 28)
                    name += '-';
                else if (letterCode == 29)
                    name += '=';
                else if (letterCode == 30)
                    name += '_';
                else if (letterCode == 31)
                    name += '?';
            }
            return name;
        }

        /// <summary>
        /// Gets what room the player is currently in
        /// </summary>
        /// <returns>Name of the room they are in</returns>
        static string GetCurrentRoom()
        {
            switch (GetValue(5, 2))
            {
                case 0:
                    return "entryway";
                case 1:
                    return "dining hall";
                case 2:
                    return "kitchen";
                case 3:
                    return "vault";
            }
            return "broken";
        }

        /// <summary>
        /// Gets a value in base-10 from storage
        /// </summary>
        /// <param name="startIndex">The bit to start at (from 0)</param>
        /// <param name="length">The amount of bits to grab</param>
        /// <returns>The value of that chunk of bits converted to base-10</returns>
        static uint GetValue(int startIndex, int length)
        {
            return (uint)((state & (((2 << (length - 1)) - 1) << startIndex)) >> startIndex);
        }

        /// <summary>
        /// Overwrites a space in storage with a given base-10 value
        /// </summary>
        /// <param name="startIndex">The bit to start at (from 0)</param>
        /// <param name="length">The amount of bits to overwrite</param>
        /// <param name="newValue">The new value to put in that place (in base-10)</param>
        static void OverwriteEntry(int startIndex, int length, int newValue)
        {
            state &= (uint)~(((2 << (length - 1)) - 1) << startIndex);
            state += (uint)(newValue << startIndex);
        }
    }
}
