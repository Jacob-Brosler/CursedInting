use std::borrow::BorrowMut;

fn main() {
    println!("What is your starting state?");
    let mut input = String::new();
    std::io::stdin().read_line(&mut input).unwrap();
    let wrapped_state = input.trim().parse::<u32>();
    let mut state : u32 = 0;
    if wrapped_state.is_ok()
    {
        state = wrapped_state.unwrap();
    }
    println!("Now running on state: {:b}", state);
    state.run();
}

trait Game {
    fn run(&mut self);
    fn get_name(&mut self) -> String;
    fn get_current_room(&mut self) -> &str;
    fn get_value(&mut self, start_index: u32, length: u32) -> u32;
    fn overwrite_entry(&mut self, start_index: u32, length: u32, new_value: u32);
}

impl Game for u32 {
    fn run(&mut self)
    {
        //If just started, ask for name
        if *self == 0
        {
            println!("What is your name brave adventurer? (Valid characters: a-z, +, -, =, _, ?)");
            let mut input = String::new();
            std::io::stdin().read_line(&mut input).unwrap();

            let mut split_name = input.chars();
            //5 is the max number of supported characters
            for i in 1..6
                {
                    *self <<= 5;
                    if i < input.len()
                    {
                        //Question mark is the default for an unsupported character
                        let mut result = 31;
                        let name_piece : char = split_name.next().unwrap();
                        if name_piece >= 'a' && name_piece <= 'z' {
                            result = name_piece as u32 - 96;
                        } else if name_piece == '+' {
                            result = 27;
                        } else if name_piece == '-' {
                            result = 28;
                        } else if name_piece == '=' {
                            result = 29;
                        } else if name_piece == '_' {
                            result = 30;
                        }
                        *self += result;
                    }
                }
            *self <<= 7;
            println!("Welcome to the spooky scary mansion {}.", self.get_name());
            println!("You walk into the entryway of the mansion. The door creaks as it slowly shuts behind you. The room itself is somewhat in a self of disrepair as dust and spiders have taken over most of the available space. A lonely, broken chandelier hangs from the ceiling high above you. The exit lays closed behind you and the way to the dining hall lies ahead.");
        }
        //If there is self but no name, go away
        else if *self < 128
        {
            println!("You try to remember who you are, but you only scream for you cannot know.\nTry a valid self next time headass.");
            return;
        }
        //Main game loop
        else {
            println!("What would you like to do, {}?", self.get_name());
            let mut input = String::new();
            std::io::stdin().read_line(&mut input).unwrap();
            input.make_ascii_lowercase();
            input = String::from(input.get(0..(input.len() - 1)).unwrap());
            let split_input = if input.contains(" ") { [input.get(0..input.find(" ").unwrap()).unwrap(), input.get((input.find(" ").unwrap() + 1)..).unwrap()] } else { [input.as_str(), ""] };
            if split_input.len() >= 1
            {
                match split_input[0]
                    {
                        "help" =>
                            println!("Available commands: \nHelp: Prints out available commands with small descriptions.\nMove <Room>: Moves to the specified room if possible.\nInspect: Prints a description of the room you are in, noting any important items\nInspect <Object>: Prints a description of a specific item in the room if it exists.\nTake <Object>: Takes a valid item and puts it in your inventory.\nUse <Item>: Uses a given item if it can be used on anything in the room you are in."),
                        "move" => {
                            if split_input[1] == ""
                            {
                                println!("Invalid move command: No target given.");
                            } else {
                                match self.get_current_room()
                                    {
                                        "entryway" => {
                                            match split_input[1]
                                                {
                                                    "exit" => {
                                                        if self.get_value(0, 1) == 1
                                                        {
                                                            println!("You leave the mansion, Rusty loot in tow. You turn back for only a single second as your farewell to this great adversary escape your lips. \"Bet.\"\n\nCongratulations, you win Cursed Inting!");
                                                        } else {
                                                            println!("Having found nothing of real value you leave the mansion dejectedly. Coward.");
                                                        }
                                                        return;
                                                    }
                                                    "dining hall" => {
                                                        println!("As you move into the dining hall you notice a large ornate wooden table filling the center of the room. There are a handful of chairs in scattered disrepair around it, and it is completely clear of items. There is a path back to the entryway behind you and a path to the kitchen to your right.");
                                                        self.overwrite_entry(5, 2, 1);
                                                    }
                                                    _ =>
                                                        println!("Invalid move command: Invalid target {} given.", split_input[1]),
                                                }
                                        }
                                        "dining hall" => {
                                            match split_input[1]
                                                {
                                                    "entryway" => {
                                                        println!("The entryway is just as dusty as you remember, however most of the spiders have been scared off by your movements. Everything else is just as you remember. The exit is in front of you and the way to the dining hall is behind you.");
                                                        self.overwrite_entry(5, 2, 0);
                                                    }
                                                    "kitchen" => {
                                                        println!("The kitchen is somewhat barren, with dust and spiders just as bad as the entryway. You smell something you're pretty sure is the trash can but you definitely don't want to go over and find out. There is a propane fridge and an ice box to your left. Behind you is the way back to the dining hall, and to your right there is another door that you think leads back to the entryway.");
                                                        self.overwrite_entry(5, 2, 2);
                                                    }
                                                    _ =>
                                                        println!("Invalid move command: Invalid target {} given.", split_input[1]),
                                                }
                                        }

                                        "kitchen" => {
                                            match split_input[1]
                                                {
                                                    "entryway" => {
                                                        println!("As the door shuts behind you you notice that it is now flush with the wall. It is probably meant to only be one way. The entryway is just as dusty as you remember, however most of the spiders have been scared off by your movements. Everything else is just as you remember. The exit is to your left and the way to the dining hall is to your right.");
                                                        self.overwrite_entry(5, 2, 0);
                                                    }
                                                    "dining hall" => {
                                                        println!("As you move into the dining hall you notice a large ornate wooden table filling the center of the room. There are a handful of chairs in scattered disrepair around it, and it is completely clear of items. There is a path back to the kitchen behind you and a path to the entryway to your left.");
                                                        self.overwrite_entry(5, 2, 1);
                                                    }
                                                    "vault" |
                                                    "mysterious vault" => {
                                                        //If they've discovered the vault
                                                        if self.get_value(1, 1) == 1
                                                        {
                                                            //If they re-closed the door
                                                            if self.get_value(2, 2) == 2
                                                            {
                                                                println!("The passageway you found is no longer there. You think flipping the lever might have closed it back up.");
                                                            }
                                                            //If the door is open
                                                            else if self.get_value(2, 2) == 3
                                                            {
                                                                println!("Moving through the passageway and down a flight of stone steps leads you to a small room deep in the mansion. In it you find gold, jewels, and other useless baubles. Then you finally find it, what you came for: Rust! This place is so old and damp it has all the Rust you could ever want!");
                                                                self.overwrite_entry(5, 2, 3);
                                                            } else {
                                                                println!("Invalid state reached: Play through the game normally you lazy bum.");
                                                                return;
                                                            }
                                                        } else {
                                                            println!("Invalid move command: Invalid target {} given.", split_input[1]);
                                                        }
                                                    }
                                                    _ =>
                                                        println!("Invalid move command: Invalid target {} given.", split_input[1]),
                                                }
                                        }

                                        "vault" => {
                                            match split_input[1]
                                                {
                                                    "kitchen" => {
                                                        println!("The kitchen appears just as you left it. You still smell something you're pretty sure is the trash can but you still definitely don't want to go over and find out. There is a propane fridge and an ice box to your left. Behind you is the way back to the vault, to your right is the door to the dining hall, and ahead of you there is another door that you think leads back to the entryway.");
                                                        self.overwrite_entry(5, 2, 2);
                                                    }
                                                    _ =>
                                                        println!("Invalid move command: Invalid target {} given.", split_input[1]),
                                                }
                                        }

                                        _ => {
                                            println!("Unreachable state: Invalid room signature. Aborting.");
                                            return;
                                        }
                                    }
                            }
                        }
                        "inspect" => {
                            let mut global_inspect_used: bool = false;
                            if split_input.len() >= 2
                            {
                                match split_input[1]
                                    {
                                        "ceiling" => {
                                            println!("The ceiling is pretty high up and hard to see, but it seems to be made mostly out of wood. It still looks pretty structurally sound but you don't know how much longer that will last.");
                                            global_inspect_used = true;
                                        }
                                        "floor" => {
                                            println!("The floor is made of very worn wood. It seems very highly trafficked but it looks safe.");
                                            global_inspect_used = true;
                                        }
                                        "wall" |
                                        "walls" |
                                        "wallpaper" => {
                                            println!("The wallpaper is peeling and extremely faded but you think you can make out a faint flower pattern.");
                                            global_inspect_used = true;
                                        }
                                        "dust" => {
                                            println!("The dust is extremely dusty. A very nice brown color.");
                                            global_inspect_used = true;
                                        }
                                        "nail" |
                                        "nails" => {
                                            println!("The nails are also nailed down. Whoever did this was very thorough or very paranoid!");
                                            global_inspect_used = true;
                                        }
                                        "spider" |
                                        "spiders" => {
                                            println!("You try to get closer to inspect them better, but as you do it jumps up and bites you. You die.");
                                            return;
                                        }
                                        "key" => {
                                            if self.get_value(4, 1) == 1
                                            {
                                                println!("The key is very simple and unassuming in nature. There are no markings to suggest what it might be used for.");
                                                global_inspect_used = true;
                                            }
                                        }

                                        _ => {}
                                    }
                            }
                            if !global_inspect_used
                            {
                                match self.get_current_room()
                                    {
                                        "entryway" => {
                                            if split_input[1] == "" || split_input[1] == self.get_current_room()
                                            {
                                                println!("The room you're in is in somewhat of a state of disrepair as dust and spiders have taken over most of the available space. A lonely, broken chandelier hangs from the ceiling high above you. There is also a small locked box attached to the wall near you. The exit lays closed behind you and the way to the dining hall lies ahead.");
                                            } else {
                                                match split_input[1]
                                                    {
                                                        "chandelier" =>
                                                            println!("You can't get close to the chandelier due to its height but from where you are it looks very broken. It's missing one of its branches and all of the candles are either melted down or cut at extremely odd angles. You don't know how it hasn't fallen down already."),

                                                        "box" |
                                                        "lock box" |
                                                        "locked box" => {
                                                            print!("The box shows a few signs of rusting on the face but it extremely solidly built. It seems to be firmly attached to the wall. ");
                                                            if self.get_value(2, 2) == 0 {
                                                                println!("It has a keyhole in the front of it. {}", if self.get_value(4, 1) == 1 { "Maybe your key will fit?" } else { "" });
                                                            } else if self.get_value(2, 2) == 1 || self.get_value(2, 2) == 3 {
                                                                println!("The lever inside is in what you assume is the 'on' position.");
                                                            } else {
                                                                println!("The lever inside is in what you assume is the 'off' position.");
                                                            }
                                                        }
                                                        "door" |
                                                        "exit" => {
                                                            print!("The door has massive gouges all across its inner face but whatever caused them appears to have already left. It is unlocked ");
                                                            if self.get_value(0, 1) == 1
                                                            {
                                                                println!("and you can leave at any time, Rust loot in tow!");
                                                            } else {
                                                                println!("but you haven't completed your objective yet.");
                                                            }
                                                        }
                                                        "lever" => {
                                                            if self.get_value(2, 2) == 0 {
                                                                println!("Invalid inspect command: Invalid target {} given.", split_input[1]);
                                                            } else if self.get_value(2, 2) == 1 || self.get_value(2, 2) == 3 {
                                                                println!("The lever is in what you assume is the 'on' position.");
                                                            } else {
                                                                println!("The lever is in what you assume is the 'off' position.");
                                                            }
                                                        }
                                                        _ =>
                                                            println!("Invalid inspect command: Invalid target {} given.", split_input[1]),
                                                    }
                                            }
                                        }

                                        "dining hall" => {
                                            if
                                            split_input[1] == "" || split_input[1] == self.get_current_room()
                                            {
                                                println!("The dining hall you notice a large ornate wooden table filling the center of the room. There are a handful of chairs in scattered disrepair around it, and it is completely clear of items.  There is a path to the kitchen and a path to the entryway.");
                                            } else {
                                                match split_input[1]
                                                    {
                                                        "table" => {
                                                            println!("The table is a beautifully carved wooden table with lion's head shaped feet, a celtic pattern carved around the edge and a massive celtic knot in the middle. It looks very expensive and has survived the years surprisingly well, but it seems extremely heavy.");
                                                            if self.get_value(4, 1) == 0 {
                                                                println!("After further inspection you notice that there is a key attached to the underside of the table.");
                                                            }
                                                        }
                                                        "chair" |
                                                        "chairs" =>
                                                            println!("The chairs are all broken, with some missing legs and some having split backs. Interestingly, they all seem to be securely nailed to the floor."),

                                                        "key" =>
                                                            println!("The key is very simple and unassuming in nature. There are no markings to suggest what it might be used for."),

                                                        _ =>
                                                            println!("Invalid inspect command: Invalid target {} given.", split_input[1]),
                                                    }
                                            }
                                        }

                                        "kitchen" => {
                                            if split_input[1] == "" || split_input[1] == self.get_current_room()
                                            {
                                                println!("The kitchen is somewhat barren, with dust and spiders everywhere. You smell something you're pretty sure is the trash can but you definitely don't want to go over and find out. There is a propane fridge and an ice box along one of the walls. There is also the way back to the dining hall and another door that you think leads back to the entryway.");
                                            } else {
                                                match split_input[1]
                                                    {
                                                        "trash" |
                                                        "trash can" => {
                                                            println!("The smell just gets worse as you get closer to the trash can. You try to wave away the stench as best as you can as you approach but nothing can save you from that god-forsaken scent. It gets worse and worse as you get closer to the source. Just as you get close enough to almost see what's causing this sin against your senses the smell causes you to fall into a coma and you die of asphyxiation.");
                                                            return;
                                                        }
                                                        "ice box" => {
                                                            println!("As you open the lid to peer into the ice box it sudden stands up and clamps down on your body, cutting you in half. You are dead.");
                                                            return;
                                                        }
                                                        "fridge" |
                                                        "propane fridge" => {
                                                            print!("The fridge itself is a simple metal box meant to keep food cold through the use of a specially placed pilot light. However, it doesn't look like its had propane in it for a very long time. ");
                                                            if self.get_value(2, 2) == 0 || self.get_value(2, 2) == 2
                                                            {
                                                                if self.get_value(1, 1) == 0 {
                                                                    println!("Opening the fridge reveals... the wall of the room? That's weird.");
                                                                } else {
                                                                    println!("The passage you saw earlier is now covered up by the wall. Maybe something you did closed it?");
                                                                }
                                                            } else if self.get_value(2, 2) == 1 {
                                                                println!("Opening the fridge reveals... the wall of the room? That's weird. There's a thin break in the wall along the right wall of the fridge as a panel appears to be partially moved but you can't seem to get it to move any more.");
                                                            } else {
                                                                println!("Opening the fridge reveals... a stone staircase spiraling downwards into darkness. Could this be the mysterious vault you've heard rumors of?");
                                                                self.overwrite_entry(1, 1, 1);
                                                            }
                                                        }
                                                        _ =>
                                                            println!("Invalid inspect command: Invalid target {} given.", split_input[1]),
                                                    }
                                            }
                                        }

                                        "vault" => {
                                            if split_input[1] == "" || split_input[1] == self.get_current_room()
                                            {
                                                print!("You are currently in a small room deep in the mansion. Most of the floor is taken up by gold, jewels, and other useless baubles. ");
                                                //If you already took the rust
                                                if self.get_value(0, 1) == 1 {
                                                    println!("However, you've already got all you could ever need from this room.");
                                                } else {
                                                    println!("However, why would you care about that? You've already got all the Rust you could ever need.");
                                                }
                                            } else {
                                                match split_input[1]
                                                    {
                                                        "gold" |
                                                        "silver" |
                                                        "jewel" |
                                                        "jewels" |
                                                        "bauble" |
                                                        "baubles" |
                                                        "useless bauble" |
                                                        "useless baubles" =>
                                                            println!("Yea yea yea, they're shiny and all but why would you need them when you have Rust?"),
                                                        "ruby" |
                                                        "rubies" =>
                                                            println!("Rust is just better than Ruby, why would you ever want to use that?"),
                                                        "rwby" |
                                                        "rwbies" =>
                                                            println!("It's also a gun?"),
                                                        "emerald" |
                                                        "emeralds" =>
                                                            println!("Huh, that's weird. The emeralds disappeared as soon as you tried to get a better look. Well who needs them anyways when you have Rust!"),
                                                        "rust" =>
                                                            println!("It's so plentiful and beautiful! It's everything you could have ever asked for!"),
                                                        _ =>
                                                            println!("Invalid inspect command: Invalid target {} given.", split_input[1]),
                                                    }
                                            }
                                        }

                                        _ => {
                                            println!("Unreachable state: Invalid room signature. Aborting.");
                                            return;
                                        }
                                    }
                            }
                        }

                        "take" => {
                            if split_input[1] == ""
                            {
                                println!("Invalid move command: No target given.");
                            } else {
                                let mut global_take_used: bool = false;
                                match split_input[1]
                                    {
                                        "ceiling" |
                                        "floor" |
                                        "wall" |
                                        "walls" => {
                                            println!("No. Just no.");
                                            global_take_used = true;
                                        }
                                        "wallpaper" => {
                                            println!("You try to peel some of the wallpaper off of the wall but the small amount that isn't still firmly adhered just breaks apart to dust as you try to take it.");
                                            global_take_used = true;
                                        }
                                        "dust" => {
                                            println!("As you try to pick up the dust it flies out of your hand and disperses into the air.");
                                            global_take_used = true;
                                        }
                                        "nail" |
                                        "nails" => {
                                            println!("The nails are also nailed down. Whoever did this was very thorough or very paranoid!");
                                            global_take_used = true;
                                        }
                                        "spider" |
                                        "spiders" => {
                                            println!("You try to get closer to grab them, but as you do it jumps up and bites you. You die.");
                                            return;
                                        }

                                        _ => {}
                                    }

                                if !global_take_used
                                {
                                    match self.get_current_room()
                                        {
                                            "entryway" => {
                                                match split_input[1]
                                                    {
                                                        "chandelier" =>
                                                            println!("You can't get close enough to take the chandelier due to how high up it is."),
                                                        "box" |
                                                        "lock box" |
                                                        "locked box" =>
                                                            print!("The box is solidly nailed to the wall and you can't seem to get a good enough grip on it to rip it off."),
                                                        "door" |
                                                        "exit" => {
                                                            print!("As you try to pull the door of its hinges it suddenly swings up from the bottom and you're launched into the ceiling. You splat into a nice red puddle and die.");
                                                            return;
                                                        }
                                                        "lever" => {
                                                            if self.get_value(2, 2) == 0 {
                                                                println!("Invalid inspect command: Invalid target {} given.", split_input[1]);
                                                            } else {
                                                                print!("You pull as hard as you can on the lever but can't get it out of the wall. As you struggle with it you accidentally flip it. ");
                                                                if self.get_value(2, 2) == 1 || self.get_value(2, 2) == 3
                                                                {
                                                                    println!("The lever is now in what you assume is the 'off' position.");
                                                                    self.overwrite_entry(2, 2, 2);
                                                                } else {
                                                                    println!("The lever is now in what you assume is the 'on' position. You hear a faint sound of stone scraping on stone coming from another part of the house");
                                                                    self.overwrite_entry(2, 2, 3);
                                                                }
                                                            }
                                                        }
                                                        _ =>
                                                            println!("Invalid take command: Invalid target {} given.", split_input[1]),
                                                    }
                                            }
                                            "dining hall" => {
                                                match split_input[1]
                                                    {
                                                        "table" =>
                                                            println!("The table is solidly nailed down to the floor and you can't seem to move it at all."),
                                                        "chair" |
                                                        "chairs" =>
                                                            println!("The chairs are all solidly nailed down to the floor and you can't seem to move them at all."),
                                                        "key" => {
                                                            if self.get_value(4, 1) == 1 {
                                                                println!("You already have the key.");
                                                            } else {
                                                                println!("You take the small key. This could be useful later.");
                                                                self.overwrite_entry(4, 1, 1);
                                                            }
                                                        }
                                                        _ =>
                                                            println!("Invalid take command: Invalid target {} given.", split_input[1]),
                                                    }
                                            }
                                            "kitchen" => {
                                                match split_input[1]
                                                    {
                                                        "trash" |
                                                        "trash can" => {
                                                            println!("The smell just gets worse as you get closer to the trash can. You try to wave away the stench as best as you can as you approach but nothing can save you from that god-forsaken scent. It gets worse and worse as you get closer to the source. Just as you get close enough to almost see what's causing this sin against your senses the smell causes you to fall into a coma and you die of asphyxiation.");
                                                            return;
                                                        }
                                                        "ice box" => {
                                                            println!("As you move to grab the ice box it sudden stands up and clamps down on your body, cutting you in half. You are dead.");
                                                            return;
                                                        }
                                                        "fridge" |
                                                        "propane fridge" =>
                                                            println!("The fridge is very solidly adhered to the wall and floor, but by what you can't tell."),
                                                        _ =>
                                                            println!("Invalid take command: Invalid target {} given.", split_input[1]),
                                                    }
                                            }
                                            "vault" => {
                                                match split_input[1]
                                                    {
                                                        "gold" |
                                                        "silver" |
                                                        "jewel" |
                                                        "jewels" |
                                                        "bauble" |
                                                        "baubles" |
                                                        "useless bauble" |
                                                        "useless baubles" =>
                                                            println!("Yea yea yea, they're shiny and all but why would you need them when you have Rust?"),
                                                        "ruby" |
                                                        "rubies" =>
                                                            println!("Rust is just better than Ruby, why would you ever want to take that?"),
                                                        "rwby" |
                                                        "rwbies" => {
                                                            println!("You get shot by a customizable high impact sniper rifle, punched and shotgunned in the face, and get nailed by title IX all in the span of one breath. You die.");
                                                            return;
                                                        }
                                                        "emerald" |
                                                        "emeralds" =>
                                                            println!("Huh, that's weird. The emeralds disappeared as soon as you tried to take them. Well who needs them anyways when you have Rust!"),
                                                        "rust" => {
                                                            //If you already took the rust
                                                            if self.get_value(0, 1) == 1 {
                                                                println!("Sadly, you've already scrubbed every inch of this place for all the Rust it had. There's no more left to grab.");
                                                            } else {
                                                                println!("It's so plentiful and beautiful! It's everything you could have ever asked for! Just holding it in your hand gives you an overwhelming sense of safety like nothing ever had before.");
                                                                self.overwrite_entry(0, 1, 1);
                                                            }
                                                        }
                                                        _ =>
                                                            println!("Invalid take command: Invalid target {} given.", split_input[1]),
                                                    }
                                            }

                                            _ => {
                                                println!("Unreachable state: Invalid room signature. Aborting.");
                                                return;
                                            }
                                        }
                                }
                            }
                        }
                        "use" => {
                            if split_input[1] == ""
                            {
                                println!("Invalid use command: No target given.");
                            } else {
                                match self.get_current_room()
                                    {
                                        "entryway" => {
                                            match split_input[1]
                                                {
                                                    "lever" => {
                                                        if self.get_value(2, 2) == 1 || self.get_value(2, 2) == 3
                                                        {
                                                            println!("The lever is now in what you assume is the 'off' position.");
                                                            self.overwrite_entry(2, 2, 2);
                                                        } else if self.get_value(2, 2) == 2
                                                        {
                                                            println!("The lever is now in what you assume is the 'on' position. You hear a faint sound of stone scraping on stone coming from another part of the house");
                                                            self.overwrite_entry(2, 2, 3);
                                                        } else {
                                                            println!("Invalid use command: Invalid target {} given.", split_input[1]);
                                                        }
                                                    }
                                                    "key" => {
                                                        if self.get_value(4, 1) == 1
                                                        {
                                                            if self.get_value(2, 2) == 0
                                                            {
                                                                println!("You try to put the key in the hole and it fits like a glove. Opening the box reveals a lever built into the wall. Before even thinking you pull it and hear a small grinding noise before it suddenly stops.");
                                                                self.overwrite_entry(2, 2, 1);
                                                            } else {
                                                                println!("You've already opened the locked box.");
                                                            }
                                                        } else {
                                                            println!("Invalid use command: Invalid target {} given.", split_input[1]);
                                                        }
                                                    }
                                                    _ =>
                                                        println!("Invalid use command: Invalid target {} given.", split_input[1]),
                                                }
                                        }
                                        _ => {
                                            match split_input[1]
                                                {
                                                    "lever" => {
                                                        if self.get_value(2, 2) != 0 {
                                                            println!("This is the wrong room to use this item.");
                                                        } else {
                                                            println!("Invalid use command: Invalid target {} given.", split_input[1]);
                                                        }
                                                    }
                                                    "key" => {
                                                        if self.get_value(4, 1) == 1 {
                                                            println!("This is the wrong room to use this item.");
                                                        } else {
                                                            println!("Invalid use command: Invalid target {} given.", split_input[1]);
                                                        }
                                                    }
                                                    _ =>
                                                        println!("Invalid use command: Invalid target {} given.", split_input[1]),
                                                }
                                        }
                                    }
                            }
                        }
                        _ =>
                            println!("Invalid command given: Command {} not recognized.", split_input[0]),
                    }
            }
        }
        self.run();
    }

    fn get_name(&mut self) -> String {
        let mut name = String::from("");
        for i in 1..6
            {
                let letter_code = self.get_value(32 - i * 5, 5);
                if letter_code > 0 && letter_code <= 26 {
                    name.push((letter_code + 96) as u8 as char);
                } else if letter_code == 27 {
                    name.push('+');
                } else if letter_code == 28 {
                    name.push('-');
                } else if letter_code == 29 {
                    name.push('=');
                } else if letter_code == 30 {
                    name.push('_');
                } else if letter_code == 31 {
                    name.push('?');
                }
            }
        return name;
    }

    fn get_current_room(&mut self) -> &str {
        match self.get_value(5, 2)
            {
                0 => return "entryway",
                1 => return "dining hall",
                2 => return "kitchen",
                3 => return "vault",
                _ => return "Invalid Room Return",
            }
    }

    fn get_value(&mut self, start_index: u32, length: u32) -> u32 {

        return (*self & (((2 << (length - 1)) - 1) << start_index)) >> start_index;
    }

    fn overwrite_entry(&mut self, start_index: u32, length: u32, new_value: u32) {
        *self &= !(((2 << (length - 1)) - 1) << start_index);
        *self += new_value << start_index;
    }
}